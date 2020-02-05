using System;
using System.Linq;
using System.Threading.Tasks;

namespace Otomatik.BlazorBin.Data
{
    public class BinService
    {
        public Task<string> GenerateCode()
        {
            return GenerateRandomText(10);
        }

        private Task<string> GenerateRandomText(int length)
        {
            var rng = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            return Task.FromResult(new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rng.Next(s.Length)]).ToArray()));
        }
    }
}
