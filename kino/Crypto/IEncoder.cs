namespace kino.Crypto
{

    public interface IEncoder
    {
        string Encode(string data);
        string Decode(string data);
    }
}
