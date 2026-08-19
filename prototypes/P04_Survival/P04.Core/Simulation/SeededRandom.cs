using System;

namespace P04.Core.Simulation
{
    /// <summary>
    /// 确定性随机：固定 seed，保证重放/实验可复现。
    /// </summary>
    public sealed class SeededRandom
    {
        private readonly System.Random _rng;

        /// <summary>种子值（用于 replay 记录）。</summary>
        public int Seed { get; }

        public SeededRandom(int seed)
        {
            Seed = seed;
            _rng = new System.Random(seed);
        }

        /// <summary>[minInclusive, maxExclusive)</summary>
        public int Next(int minInclusive, int maxExclusive)
        {
            return _rng.Next(minInclusive, maxExclusive);
        }

        /// <summary>[0f, 1f)</summary>
        public float NextFloat()
        {
            return (float)_rng.NextDouble();
        }

        /// <summary>从选项里随机取一个。</summary>
        public T Pick<T>(System.Collections.Generic.IReadOnlyList<T> options)
        {
            return options[Next(0, options.Count)];
        }
    }
}
