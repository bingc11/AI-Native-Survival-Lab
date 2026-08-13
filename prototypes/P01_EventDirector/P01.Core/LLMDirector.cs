using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using P00.Core;

namespace P01.Core
{
    /// <summary>
    /// LLM 导演：策略三兄弟之三（压轴）。
    /// 通过 P00 的 IGameAIProvider 调模型，让模型从事件表里选一个。
    ///
    /// 这是 P00 中间层第一次真正干活：
    ///   P01 的游戏层问题 → IGameAIProvider → (Stub/LLMUnity) → 模型
    /// 模型可能乱编 ID —— 接口只保证给一个字符串，合法性由调用方用 EventRegistry 验收。
    /// </summary>
    public sealed class LLMDirector : IEventDirector
    {
        private readonly IGameAIProvider _provider;
        private readonly IReadOnlyList<GameEvent> _availableEvents;
        private readonly string _systemDirective;

        public LLMDirector(
            IGameAIProvider provider,
            IEnumerable<GameEvent> availableEvents,
            string? systemDirective = null)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _availableEvents = (availableEvents ?? throw new ArgumentNullException(nameof(availableEvents)))
                is IReadOnlyList<GameEvent> list ? list : new List<GameEvent>(availableEvents);
            _systemDirective = systemDirective ?? "Choose exactly one event id from the list. Reply only with JSON: {\"event\":\"<id>\"}";
        }

        public async Task<string?> ChooseEventAsync(string gameState, CancellationToken cancellationToken)
        {
            string prompt = BuildPrompt(gameState);
            string raw = await _provider.CompleteAsync(prompt, cancellationToken);
            return ExtractEventId(raw);
        }

        /// <summary>把"游戏状态 + 可选事件清单"拼成给模型的提示词。</summary>
        private string BuildPrompt(string gameState)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_systemDirective);
            sb.AppendLine();
            sb.AppendLine("Game state: " + (gameState ?? "(empty)"));
            sb.AppendLine();
            sb.AppendLine("Available events:");
            foreach (GameEvent e in _availableEvents)
            {
                sb.AppendLine($"- id={e.Id}, title={e.Title}, requirement={e.Requirement ?? "none"}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从模型回复里抠出事件 ID。容错：允许回复带多余文本，只取 JSON 里 event 字段。
        /// 解析失败返回 null（无事发生）——不会让乱回复变成事件。
        /// </summary>
        private static string? ExtractEventId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string candidate = raw;
            int start = candidate.IndexOf('{');
            int end = candidate.LastIndexOf('}');
            if (start >= 0 && end > start)
            {
                candidate = candidate.Substring(start, end - start + 1);
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(candidate);
                if (doc.RootElement.ValueKind == JsonValueKind.Object
                    && doc.RootElement.TryGetProperty("event", out JsonElement ev)
                    && ev.ValueKind == JsonValueKind.String)
                {
                    string? id = ev.GetString();
                    return string.IsNullOrWhiteSpace(id) ? null : id;
                }
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
