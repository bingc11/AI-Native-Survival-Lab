using System;
using System.Collections.Generic;

namespace P04.Core.Event
{
    /// <summary>
    /// 事件总线：广播 WorldEvent，供响应者监听。
    /// </summary>
    public sealed class EventBus
    {
        private readonly List<Action<WorldEvent>> _listeners = new List<Action<WorldEvent>>();

        /// <summary>订阅事件。</summary>
        public void Subscribe(Action<WorldEvent> listener)
        {
            _listeners.Add(listener ?? throw new ArgumentNullException(nameof(listener)));
        }

        /// <summary>广播事件。</summary>
        public void Publish(WorldEvent evt)
        {
            foreach (var listener in _listeners)
            {
                listener(evt);
            }
        }
    }
}
