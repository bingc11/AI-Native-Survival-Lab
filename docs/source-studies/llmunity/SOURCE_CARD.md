# LLMUnity SOURCE_CARD

## 基本信息

| 项 | 值 |
|----|----|
| 仓库名 | LLM for Unity（LLMUnity） |
| Git URL | https://github.com/undreamai/LLMUnity |
| 原记录 URL | https://github.com/ilciro/LLMUnity（**已 404，仓库迁移，需更正索引**） |
| 本地路径 | E:\references\LLMUnity（即 ../references/LLMUnity） |
| Clone 时间 | 2026-08-07 |
| Commit | `2c30b44` add CielChan to projects |
| Tag | v3.0.3-3-g2c30b44（v3.0.3 之后 3 个提交；package.json 版本 3.0.3） |
| License | Apache License 2.0（LICENSE.md）；RAG 内 usearch 目录另有独立 LICENSE |
| 包名 | ai.undream.llm |

## 运行环境

- Unity 2022.3.16f1 起（package.json 声明 unity 2022.3 / unityRelease 16f1）
- 依赖：com.unity.nuget.newtonsoft-json 3.0.2（JObject 用于参数与历史序列化）
- 平台：Windows 走静态 DllImport，其他平台走 LibraryLoader 动态符号加载；原生库为预编译 DLL（Resources/，源码不在本仓库）
- 支持本地（GGUF 模型，llama.cpp 内核）与远程（HTTP 服务，host/port/APIKey）两种模式

## 是否允许复用

- Apache 2.0：允许商用/修改/分发，需保留版权与许可声明，注明修改。
- 本仓库仅记录调用链与设计理解，**不提交第三方完整源码**（CLAUDE.md 协议）。
- 对 P00：允许以依赖方式引用包（如 Unity Package 或 asmdef 引用），并以派生类方式扩展；不 fork 修改其核心。

## 与生存 Demo 的关系（P00）

- P00 Provider Probe 需要"游戏层与模型层的隔离边界"，LLMUnity 是候选 Provider。
- 相关能力：`LLMAgent` 聊天式对话、`grammar`（GBNF / JSON schema）结构化输出、本地/远程切换。
- 缺失能力：无游戏层"输出校验/重试"工具，需 P00 自建（见 READING_GUIDE 第 5 节接口契约）。

## 审计状态

SCOUT 完成，已产出 READING_GUIDE.md。等待用户阅读确认后，方可进入 BUILDER/REVIEW 阶段。
