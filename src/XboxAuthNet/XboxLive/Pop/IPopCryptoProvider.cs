namespace XboxAuthNet.XboxLive.Pop
{
    public interface IPopCryptoProvider
    {
        object ProofKey { get; }
        byte[] Sign(byte[] data);
    }
}