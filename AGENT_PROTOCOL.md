# Agent 工作协议
Agent 助手在 TASK 约束下工作。主控台负责创建/更新任务，Agent 只读取任务文件及最小必要上下文。

## 启动流程
1. 确认唯一活动任务存在
2. 读取任务文件
3. 只读取与任务直接相关的源码
4. 产出任务指定文件
5. 不承诺超出任务范围的工作
6. commander 审查后决定下一步

## SCOUT 产出
- docs/source-studies/<repo>/READING_GUIDE.md
- docs/source-studies/<repo>/SOURCE_CARD.md
- docs/source-studies/<repo>/MY_NOTES.md（留给用户）

## 禁止行为
- 不读取整个仓库
- 不替用户填写 MY_NOTES
- 不承诺进入实现阶段
- 不修改非任务允许范围的文件

## 交付格式
简洁工程汇报：
- 当前判断
- 已完成
- 验证结果
- 阻塞
- 下一步
