namespace P01.Core
{
    /// <summary>
    /// 一次导演决策的最终分类（收口人的五种结论）。
    /// 这就是"确定性执行边界"的落地：不管导演给了什么，最终必须归到这五类里的一类。
    /// </summary>
    public enum DirectorOutcome
    {
        /// <summary>合法事件，已通过围墙验收，可以执行。</summary>
        EventSelected,

        /// <summary>导演放弃选择（返回 null）——无事发生。</summary>
        NothingHappened,

        /// <summary>导演给了墙外 ID（典型：LLM 乱编）——拒绝执行。</summary>
        InvalidId,

        /// <summary>超时或外部取消——中止，不执行。</summary>
        TimeoutOrCancelled,

        /// <summary>未知异常——记录错误，不执行。</summary>
        Error,
    }
}
