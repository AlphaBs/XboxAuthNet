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
        // 표준 JSON 기능 스위치는 AOT 게시 시 false로 고정되지만 분석기는 래퍼의 계약을 자동 추론하지 못한다.
        [UnconditionalSuppressMessage("Trimming", "IL4000", Justification = "System.Text.Json의 리플렉션 기능 스위치가 false일 때만 생성 메타데이터 경로를 사용하며 두 모드 모두 검증한다.")]
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
        // JIT 이용자의 기존 사용자 정의 모델은 유지하되, AOT에서는 생성된 메타데이터만 사용한다.
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

    internal static string Serialize(object body)
    {
        return JsonSerializer.Serialize(body, Options.GetTypeInfo(body.GetType()));
    }
}
