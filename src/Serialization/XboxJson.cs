using System.Text.Json;
#if NET10_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace XboxAuthNet;

internal static class XboxJson
{
    internal static JsonSerializerOptions Options { get; } = CreateOptions(false);
    internal static JsonSerializerOptions RequestOptions { get; } = CreateOptions(true);

#if NET10_0_OR_GREATER
    [FeatureGuard(typeof(RequiresDynamicCodeAttribute))]
    [FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
#endif
    internal static bool ReflectionEnabled
    {
#if NET10_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL4000", Justification = "Reflection fallback is disabled when dynamic code or the System.Text.Json reflection feature switch is unavailable.")]
#endif
        get
        {
#if NET5_0_OR_GREATER
            if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
                return false;
#endif
            return JsonSerializer.IsReflectionEnabledByDefault;
        }
    }

    private static JsonSerializerOptions CreateOptions(bool ignoreNull)
    {
        IJsonTypeInfoResolver resolver = XboxJsonContext.Default;
        if (ReflectionEnabled)
            resolver = JsonTypeInfoResolver.Combine(resolver, new DefaultJsonTypeInfoResolver());

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = resolver,
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
