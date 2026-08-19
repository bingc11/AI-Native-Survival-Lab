using System;

namespace P04.Core.State
{
    /// <summary>
    /// 时间状态：第几天/第几小时/白天黑夜。
    /// 1 tick = 1 游戏小时。
    /// </summary>
    public sealed class TimeState
    {
        public int Day { get; private set; } = 1;
        public int Hour { get; private set; } = 8; // 从早上 8 点开始

        public bool IsDaytime => Hour >= 6 && Hour < 20;

        /// <summary>推进 1 tick（1 小时）。</summary>
        public void Advance()
        {
            Hour++;
            if (Hour >= 24)
            {
                Hour = 0;
                Day++;
            }
        }
    }
}
