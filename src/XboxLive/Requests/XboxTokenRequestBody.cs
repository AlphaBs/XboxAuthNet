namespace XboxAuthNet.XboxLive.Requests
{
    // 서명에 사용하는 JSON 본문이 달라지지 않도록 속성 선언 순서를 유지한다.
    internal sealed class XboxTokenRequestBody<T>
    {
        public T? Properties { get; set; }

        public string? RelyingParty { get; set; }

        public string TokenType { get; set; } = "JWT";
    }
}
