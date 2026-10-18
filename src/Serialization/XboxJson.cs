using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace XboxAuthNet;

internal static class XboxJson
{
    internal static JsonSerializerOptions Options { get; } = new()
    {
        TypeInfoResolver = XboxJsonContext.Default,
    };

    internal static JsonTypeInfo<T> TypeInfo<T>()
    {
        return (JsonTypeInfo<T>)Options.GetTypeInfo(typeof(T));
    }
}
