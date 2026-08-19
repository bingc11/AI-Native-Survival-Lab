using System;
using System.Collections.Generic;

namespace P04.Core.State
{
    /// <summary>
    /// 资源状态：各地点木材/食物/水源量。
    /// </summary>
    public sealed class ResourceState
    {
        private readonly Dictionary<string, Resources> _byLocation = new Dictionary<string, Resources>();

        public ResourceState()
        {
            // 初始化几个地点
            _byLocation["Forest"] = new Resources { Wood = 50, Food = 10, Water = 5 };
            _byLocation["River"] = new Resources { Wood = 5, Food = 5, Water = 50 };
            _byLocation["Cave"] = new Resources { Wood = 0, Food = 0, Water = 2 };
        }

        public Resources Get(string location)
        {
            return _byLocation.TryGetValue(location, out var res) ? res : new Resources();
        }

        public void Consume(string location, string resource, float amount)
        {
            if (!_byLocation.TryGetValue(location, out var res)) return;
            switch (resource)
            {
                case "Wood": res.Wood = Math.Max(0, res.Wood - amount); break;
                case "Food": res.Food = Math.Max(0, res.Food - amount); break;
                case "Water": res.Water = Math.Max(0, res.Water - amount); break;
            }
        }
    }

    public class Resources
    {
        public float Wood { get; set; }
        public float Food { get; set; }
        public float Water { get; set; }
    }
}
