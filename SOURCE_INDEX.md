# SOURCE_INDEX.md
第三方源码审计状态。列为参考的仓库所在地遵循总控协议：位于项目外部相邻目录（AI-Native-Survival-Workspace/）。

| 仓库 | 本地路径 | Git URL | Commit/Tag | License | 运行环境 | 研究内容 | 允许复用 | 与生存 Demo 的关系 | 审计状态 |
|------|---------|---------|------------|---------|---------|---------|---------|-----------------|---------|
| LLMUnity | ../references/LLMUnity | https://github.com/undreamai/LLMUnity | 2c30b44 / v3.0.3 | Apache-2.0 | Unity 2022.3.16f1+，本地 llama.cpp / 远程 HTTP | LLM Unity 集成；结构化输出（grammar：GBNF/JSON schema）；扩展点（LLMAgent.Chat override、SetupCallerObject 换适配器） | 是（Apache-2.0） | P00 Provider | SCOUT 完成（2026-08-07），READING_GUIDE 待用户确认 |
| GOAP | ../references/GOAP | - | - | - | - | GOAP | - | P01/P02 | 待审计 |
| Voyager | ../references/Voyager | - | - | - | - | 自动导航 Agent | - | 参考 | 待审计 |
| Chop-Chop | ../references/Chop-Chop | - | - | - | - | 简单生存机制 | - | P01 参考 | 待审计 |

(占位条目，实际路径将在首次检查后由 Agent 更新)
