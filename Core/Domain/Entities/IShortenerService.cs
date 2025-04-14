namespace Core.Domain.Entities
{
    public interface IShortenerService
    {
        string Encode(int id);
        int Decode(string encoded);
    }
}
