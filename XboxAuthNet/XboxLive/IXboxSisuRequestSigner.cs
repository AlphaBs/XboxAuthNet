namespace XboxAuthNet.XboxLive
{
    public interface IXboxSisuRequestSigner
    {
        object ProofKey { get; }
        string SignRequest(string reqUri, string token, string body);
    }
}
