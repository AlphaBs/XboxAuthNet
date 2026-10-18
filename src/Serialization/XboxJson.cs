using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace XboxAuthNet;

internal static class XboxJson
{
    internal static JsonSerializerOptions Options { get; } = CreateOptions(false);
    internal static JsonSerializerOptions RequestOptions { get; } = CreateOptions(true);

    private static JsonSerializerOptions CreateOptions(bool ignoreNull)
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = XboxJsonContext.Default,
        };
        if (ignoreNull)
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        return options;
    }

    internal static JsonTypeInfo<T> TypeInfo<T>()
    {
        return (JsonTypeInfo<T>)Options.GetTypeInfo(typeof(T));
    }
}
