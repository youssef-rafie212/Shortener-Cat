using Core.Domain.Entities;
using System.Text;

namespace Core.Services
{
    public class ShortenerService : IShortenerService
    {
        private const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string Encode(int id)
        {
            if (id == 0) return "0";

            var res = new StringBuilder();
            while (id > 0)
            {
                res.Insert(0, chars[id % 62]);
                id /= 62;
            }
            return res.ToString();
        }

        public int Decode(string encoded)
        {
            int res = 0;
            foreach (char c in encoded)
            {
                res = res * 62 + chars.IndexOf(c);
            }

            return res;
        }

    }
}
