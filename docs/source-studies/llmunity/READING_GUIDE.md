# LLMUnity READING_GUIDE

审计日期：2026-08-07（接续 AGENT_BRIEF 2026-08-04）
审计对象：LLMUnity v3.0.3（commit `2c30b44`，本地路径 `E:\references\LLMUnity`）
用途：为 P00 Provider Probe 设计"游戏层与模型层的隔离边界"，理解结构化输出调用链与扩展点。

> 注意：TASK-0001 简报中记录的 Git URL（`https://github.com/ilciro/LLMUnity`）已失效（404）。
> 官方仓库已迁移至 **https://github.com/undreamai/LLMUnity**，本次按新地址 clone。

---

## 0. 仓库布局速览（读之前先知道这些）

```
Runtime/          核心运行时（Unity 侧全部代码都在这）
├── LLM.cs        模型持有者（MonoBehaviour），负责加载/就绪/llmService
├── LLMClient.cs  LLM 客户端基类（MonoBehaviour）：本地/远程切换、grammar、补全参数、Completion
├── LLMAgent.cs   对话代理（MonoBehaviour）：聊天历史、systemPrompt、Chat()
├── LLMCharacter.cs 已弃用，仅保留别名（LLMAgent 的子类）
├── LLMManager.cs / LLMBuilder.cs / LLMGGUF.cs / LLMUnitySetup.cs  模型下载/构建/安装
└── LlamaLib/     与原生 llama 库的互操作层（无 MonoBehaviour，纯 C# 包装）
    ├── LlamaLib.cs      所有 P/Invoke 声明（delegate + DllImport + 动态符号加载）
    ├── LLMService.cs    模型服务工厂（LLMService_Construct）
    ├── LLMClient.cs     底层客户端（Completion/Tokenize/Embeddings 的持有者）
    ├── LLMAgent.cs      底层对话代理（Chat 的持有者）
    └── LLM.cs           底层 LLMLocal 基类（SetGrammar 等）
Samples~/        使用示例（FunctionCalling 是最相关的一个）
Editor/          仅 Inspector/构建管线，运行时不执行
Resources/       原生库二进制与模型下载配置
Tests/           单元测试
```

命名约定：`LLMUnity.*` 是 Unity 侧类；`UndreamAI.LlamaLib.*` 是原生互操作层类。两者同名（如 `LLMAgent`）时以命名空间区分。

---

## 1. 关键调用链（按 P00 关注度排序）

### 路径 A —— Chat 对话（结构化输出主路径，P00 最相关）

```
1. 游戏层调用（入口）
   LLMUnity.LLMAgent.Chat(string query, Action<string> callback,
                           Action completionCallback, bool addToHistory)
   Runtime/LLMAgent.cs:269
   → 游戏脚本只需持有 LLMAgent（或 LLMCharacter，但已弃用）

2. 同一 MonoBehaviour 的内部 llmAgent
   llmAgent.ChatAsync(query, addToHistory, wrappedCallback, false, debugPrompt)
   Runtime/LLMAgent.cs:290
   → wrappedCallback 把流式文本回调包装回 Unity 主线程（Utils.WrapCallbackForAsync / IL2CPP 分支）

3. 互操作层异步
   UndreamAI.LlamaLib.LLMAgent.ChatAsync → Task.Run(() => Chat(...))
   Runtime/LlamaLib/LLMAgent.cs:269-272

4. 互操作层原生调用
   llamaLib.LLMAgent_Chat(llm, userPrompt, addToHistory, callback,
                          returnResponseJson=false, debugPrompt)
   Runtime/LlamaLib/LLMAgent.cs:262-267

5. P/Invoke 边界
   LLMAgent_Chat（delegate：Runtime/LlamaLib/LlamaLib.cs 约 136 行；
                 DllImport：约 508 行；Windows 静态绑定 / 其他平台动态符号 GetSymbolDelegate）
   → 原生 C++ 服务（预编译 DLL，代码不在本仓库）执行：
     prompt 模板 → 采样（受 grammar 约束）→ 流式回调 → 返回完整文本
```

游戏侧得到的是 `Task<string>`，即最终生成的**纯文本**。

### 路径 B —— Completion 原始补全（无聊天历史、无代理状态）

```
1. LLMUnity.LLMClient.Completion(string prompt, Action<string> callback,
                                 Action completionCallback, int id_slot)
   Runtime/LLMClient.cs:579
2. SetCompletionParameters() 把 temperature/topK/... 组装成 JObject
   Runtime/LLMClient.cs:464
3. llmClient.CompletionAsync(prompt, wrappedCallback, id_slot)
   → UndreamAI.LlamaLib.LLMLocal.CompletionAsync → 原生 LLM_Completion
   Runtime/LlamaLib/LlamaLib.cs（LLM_Completion delegate + DllImport）
```

与路径 A 的区别：B 不维护 chat 历史、不带 systemPrompt 模板、不经过 LLMAgent_Chat。

### 路径 C —— 初始化 / 模型加载（理解"谁先就绪"用）

```
1. LLM（模型持有者，场景中通常一个）：Awake 创建 llmService
   Runtime/LLM.cs:143（属性 llmService、WaitUntilReady()）
   → LLMService_Construct 由原生层加载 GGUF 模型
   Runtime/LlamaLib/LLMService.cs:60-124
2. LLMClient.Awake：AssignLLM() 自动绑定场景中的 LLM
   Runtime/LLMClient.cs:359-387
3. LLMClient.Start：SetupCaller() → SetupCallerObject()
   Runtime/LLMClient.cs:270-304
   → 本地模式：new UndreamAI.LlamaLib.LLMClient(llm.llmService)（共享原生模型）
   → 远程模式：new UndreamAI.LlamaLib.LLMClient(host, port, APIKey, numRetries)（HTTP 连接）
4. LLMAgent.SetupCallerObject：追加创建 llmAgent = new UndreamAI.LlamaLib.LLMAgent(llmClient, systemPrompt)
   Runtime/LLMAgent.cs:118-137
5. LLMAgent.PostSetupCallerObject：SetGrammar、slot、overflow 策略、InitHistory
   Runtime/LLMAgent.cs:142-152
```

阅读重点：本地模式下 `llmClient`/`llmAgent` 共享同一个原生 `llm` 句柄，多个 Agent 通过 slot 并发。

---

## 2. 结构化输出（JSON/Mode）现状 —— 回答 TASK-0001 第 5 问

- **机制**：grammar（语法约束）。不是 post-hoc 解析，是采样期约束。
  - `LLMAgent.grammar` 属性 → `LLMClient.SetGrammar(string)` → `GetCaller()?.SetGrammar(_grammar)` → 原生 `LLM_Set_Grammar`
  - Runtime/LLMClient.cs:189-194、425-428；Runtime/LlamaLib/LlamaLib.cs（LLM_Set_Grammar）
- **格式**：GBNF 或 JSON schema 均可（LLMClient.cs:51 注释 + CHANGELOG v2.5.1 "Allow JSON schema grammars (PR: #333)"）。JSON schema → GBNF 的转换在**原生层**完成（本仓库无此代码）。
- **官方示例**：`Samples~/FunctionCalling/FunctionCalling.cs`。用 `llmAgent.grammar = "root ::= (\"Weather\" | \"Time\" | \"Emotion\")"` 把输出约束为三个函数名之一，然后直接 `CallFunction(functionName)`。
- **辅助参数**：原生 `LLM_Completion` 还有 `return_response_json`（返回 `{"prompt", "content"}` 包装），但 `LLMAgent.ChatAsync` 调用时固定传 `false`，这条路径的 JSON 包装未启用。
- **结论**：v3.0.3 提供"语法级"结构化输出（strong），但**没有**"schema 定义 → 自动校验/重试"的游戏层工具（weak）——校验、失败重试需 P00 自己实现。

---

## 3. 扩展点（回答 TASK-0001 第 4 问）—— 格式：类 → 方法 → 用途

| # | 类（文件） | 方法 | 用途 |
|---|-----------|------|------|
| 1 | `LLMUnity.LLMAgent`（Runtime/LLMAgent.cs） | `Chat()`（virtual） | P00 的 Guard/Validator 注入点：override 后在收到文本后做结构化校验与重试 |
| 2 | `LLMUnity.LLMClient`（Runtime/LLMClient.cs） | `SetGrammar()` / `LoadGrammar()`（virtual） | 锁定输出契约：强制 JSON schema，禁止模型输出自由文本 |
| 3 | `LLMUnity.LLMClient`（Runtime/LLMClient.cs） | `SetupCallerObject()`（virtual） | 替换底层适配器：自建 LLMLocal 派生类，或换远程服务协议 |
| 4 | `LLMUnity.LLMClient`（Runtime/LLMClient.cs） | `GetCaller()`（virtual） | 返回自定义 LLMLocal；所有统一入口（SetGrammar/SetCompletionParameters/Completion）都会走它 |
| 5 | `LLMUnity.LLMClient`（Runtime/LLMClient.cs） | `IsAutoAssignableLLM()`（virtual） | 控制自动绑定逻辑：同一场景多模型时选谁 |
| 6 | `LLMUnity.LLMAgent`（Runtime/LLMAgent.cs） | `Warmup()` | 预处理 systemPrompt，缩小首响应延迟（对"确定性执行边界"有意义的优化） |

注意：没有现成的"validator"或"output schema"类型。要注入校验器，正确位置是扩展点 1（派生 LLMAgent + override Chat）。

---

## 4. 可跳过目录（回答 TASK-0001 第 6 问）—— 格式：目录 + 理由

| 目录/文件 | 理由（与 P00 关系） |
|----------|-------------------|
| `Editor/` | 仅 Inspector 绘制与构建注入（LLMBuildProcessor 等），运行时不执行 |
| `Samples~/` | 使用示例；除 FunctionCalling 外与 P00 设计无直接关系 |
| `Tests/` | 单元测试，无架构信息 |
| `Runtime/RAG/` | 检索增强系统，P00 不需要（注意其内 usearch 目录有独立 LICENSE） |
| `Runtime/Helpers/EventSystemAutoSetup.cs` | UI 事件系统辅助 |
| `Runtime/IL2CPP.cs` | IL2CPP 平台回调包装；P00 初期 Mono 调试可跳过 |
| `Resources/` | 原生库二进制/下载配置，运行时需要但不读代码 |
| `CHANGELOG.md`、`.github/`、`Migration.md` | 元信息 |

---

## 5. P00 最小修改建议（仅接口契约，回答 TASK-0001 第 7 问）

1. **隔离边界**：游戏层依赖 `LLMUnity.LLMAgent` 派生类（如 `P00Agent`），不直接触碰 `UndreamAI.LlamaLib.*`。这样底层适配器可整体替换。
2. **保持签名**：`P00Agent.Chat(query, callback, completionCallback, addToHistory)` 签名与基类一致，保证现有行为不破坏。
3. **输出契约**：构造时设置 `grammar`（JSON schema 或 GBNF），Chat 返回仍是 `Task<string>`；游戏层对字符串反序列化为目标类型。
4. **确定性执行边界（AI 方案必须有非 AI 基线）**：对 grammar 约束后的输出做模式匹配校验（如 `JObject.Parse` + 字段存在性），失败则有限重试，最终回退到固定模板响应（非 AI 基线），避免模型输出侵入游戏状态机。
5. **不修改第三方源码**：所有改动都在本仓库侧（派生类），`../references/LLMUnity` 保持只读。

---

## 6. 阅读时需回答的问题（留给用户确认理解）

- Q1：为什么 `Chat` 走 `LLMAgent_Chat` 而不是 `LLM_Completion`？两者在 prompt 处理上的差异是什么？（提示：systemPrompt + 历史模板在原生侧拼接）
- Q2：`return_response_json` 参数在哪条路径被置为 true？（提示：搜索 `returnResponseJson` / `return_response_json`）
- Q3：`grammar` 字符串传给原生层后，JSON schema 到 GBNF 的转换发生在哪一层？本仓库能看到实现吗？（提示：预编译 DLL，看不到）
- Q4：流式回调 `wrappedCallback` 如何从原生线程回到 Unity 主线程？（提示：Utils.WrapCallbackForAsync）
- Q5：本地与远程两条路径的适配点分别在哪两个构造函数？（提示：`Runtime/LlamaLib/LLMClient.cs:7` 与 `:16`）
- Q6：如果要注入"JSON 输出校验器"，最合适的方法是哪个类的哪个 override？（答案：LLMAgent.Chat override，见扩展点 1）
- Q7：P00 的隔离边界选型：派生 `LLMAgent`（保留聊天历史）还是直接用 `LLMClient.Completion`（无状态）？各自的代价是什么？
