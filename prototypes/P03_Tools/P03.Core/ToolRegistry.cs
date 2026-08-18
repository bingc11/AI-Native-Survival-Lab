using System;
using System.Collections.Generic;
using System.Linq;

namespace P03.Core
{
    /// <summary>
    /// 工具白名单（"围墙"）：AI 只能调用这里注册过的工具。
    /// 和 P01 的 EventRegistry 是同一个模式——AI 可以自由决定，
    /// 但只能从白名单里选，不能发明工具。
    /// </summary>
    public sealed class ToolRegistry
    {
        private readonly Dictionary<string, AITool> _byName;

        public ToolRegistry(IEnumerable<AITool> tools)
        {
            _byName = (tools ?? throw new ArgumentNullException(nameof(tools)))
                .ToDictionary(t => t.Name, t => t); // 重复名会抛异常（防呆）
        }

        public IReadOnlyList<AITool> All => _byName.Values.ToList();

        public bool Contains(string name) => name != null && _byName.ContainsKey(name);

        public AITool? Get(string name)
            => name != null && _byName.TryGetValue(name, out AITool tool) ? tool : null;
    }
}
