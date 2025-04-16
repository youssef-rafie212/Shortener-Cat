namespace Core.ServicesContracts
{
    public interface IShortenerService
    {
        string Encode(int id);
        int Decode(string encoded);
    }
}
