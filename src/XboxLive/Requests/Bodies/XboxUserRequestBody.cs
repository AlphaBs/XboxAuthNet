namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    // 서명에 사용하는 JSON 본문이 달라지지 않도록 속성 선언 순서를 유지한다.
    internal sealed class XboxUserRequestBody<T>
    {
        public string? RelyingParty { get; set; }

        public string TokenType { get; set; } = "JWT";

        public T? Properties { get; set; }
    }
}
