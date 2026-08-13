using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace P00.Core
{
    /// <summary>
    /// 确定性执行边界（补全 LLMUnity 缺失的一环，见 READING_GUIDE 第 5 节）：
    /// 包装一个 provider，对其输出做 JSON 解析 + 必需字段校验，
    /// 失败则有限重试（最多 _maxRetries 次），最终回退到固定模板响应（非 AI 基线）。
    /// 保证模型输出即便完全失控，游戏层也只会收到「合法 JSON 或兜底 JSON」。
    /// </summary>
    public sealed class JsonOutputValidator : IGameAIProvider
    {
        private readonly IGameAIProvider _inner;
        private readonly IReadOnlyList<string> _requiredFields;
        private readonly int _maxRetries;
        private readonly string _fallbackJson;

        public JsonOutputValidator(
            IGameAIProvider inner,
            IEnumerable<string> requiredFields,
            int maxRetries = 2,
            string? fallbackJson = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _requiredFields = (requiredFields ?? throw new ArgumentNullException(nameof(requiredFields))).ToList();
            _maxRetries = Math.Max(0, maxRetries);
            _fallbackJson = fallbackJson ?? DefaultFallback(_requiredFields);
        }

        public async Task<string> CompleteAsync(string query, CancellationToken cancellationToken)
        {
            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string output = await _inner.CompleteAsync(query, cancellationToken);
                if (TryValidate(output)) return output;
            }
            return _fallbackJson;
        }

        /// <summary>
        /// 校验输出是否为合法 JSON 对象且包含全部必需字段。
        /// 失败时通过 <paramref name="error"/> 给出原因。
        /// </summary>
        public bool TryValidate(string output, out string? error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(output))
            {
                error = "output is null or empty";
                return false;
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(output);
                JsonElement root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    error = $"root is {root.ValueKind}, expected Object";
                    return false;
                }

                foreach (string field in _requiredFields)
                {
                    if (!root.TryGetProperty(field, out JsonElement value))
                    {
                        error = $"missing required field: '{field}'";
                        return false;
                    }

                    if (value.ValueKind == JsonValueKind.Null)
                    {
                        error = $"required field '{field}' cannot be null";
                        return false;
                    }
                }

                return true;
            }
            catch (JsonException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private bool TryValidate(string output) => TryValidate(output, out _);

        /// <summary>构造一个满足 schema 的固定兜底响应（非 AI 基线）。</summary>
        private static string DefaultFallback(IReadOnlyCollection<string> requiredFields)
        {
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                foreach (string field in requiredFields)
                {
                    writer.WriteString(field, "fallback");
                }
                writer.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
