# HANDOFF.md
用于新主控会话接续。最新更新：2026-08-07

## 当前目标
- TASK-0002（P00 Provider Probe）实现完成，等主控台 REVIEW。

## 已完成
- TASK-0001（LLMUnity SCOUT）归档 tasks/completed/（commit 8278b9a）。
- TASK-0002 实现（commit 4bb6c20）：prototypes/P00_ProviderProbe/
  - P00.Core：IGameAIProvider、StubProvider（非 AI 基线）、JsonOutputValidator（校验/重试/兜底）
  - P00.Tests：12 个测试全绿（确定性 / schema 校验 / 重试 / 兜底自洽 / 取消传播 / 不阻塞）
  - UnityAdapter/LLMUnityAdapter.cs：代码形态（不编译），按 READING_GUIDE 契约包装 LLMAgent.Chat
  - 依赖：P00.Core netstandard2.1（可被 Unity 引用）+ System.Text.Json 8.0.5；Tests net10.0 + xunit

## 未完成
- 主控台 REVIEW（TASK-0002 暂停点）。
- Unity 工程内实际加载模型验证（需 Unity 编辑器）。

## 工作区和分支状态
- E:\Unity-AIProjects，main 分支，与 origin/main 同步（4bb6c20）。
- git 直连 GitHub 不可达：push/clone 需 `git -c http.proxy=http://127.0.0.1:7897 ...`。
- NuGet 直连也有 SSL 抖动：dotnet restore/test 前设 `$env:HTTPS_PROXY` 更稳。
- 环境：dotnet 10.0.200 可用；无 Unity 编辑器。

## 测试状态
- `dotnet test prototypes/P00_ProviderProbe/P00.slnx` → 12/12 通过，零警告。

## 已知问题
- 勿用 PowerShell 的 Set-Content/Get-Content 改写带中文的源文件（会破坏 UTF-8 编码），用编辑器工具。
- Git Credential Manager 曾报"页文件太小"崩溃；走代理后 push 正常。
- native llama 库为预编译 DLL，JSON schema→GBNF 转换在原生层，仓库内不可见。

## 不应重复的调查
- 不再重新 clone LLMUnity、不重读整个仓库、不重写 P00 核心逻辑（除非 REVIEW 要求调整接口形状）。

## 下一步第一项动作
1. 主控台 REVIEW P00：接口形状（IGameAIProvider 是否够用）、JsonOutputValidator 语义（重试次数/兜底策略/取消）、测试覆盖是否满足退出条件。
2. REVIEW 通过后：TASK-0002 关闭 → P00 进 Unity 工程（需 Unity 编辑器，可能切换到有 Unity 的机器）。
3. 若 REVIEW 要求改动：以新 BUILDER/REVIEW 任务形式下达，不直接扩大范围。
