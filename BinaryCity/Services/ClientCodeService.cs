using BinaryCity.Data;
using System.Text.RegularExpressions;

namespace BinaryCity.Services
{
    public interface IClientCodeService
    {
        string GenerateClientCode(string clientName);
        }
    public class ClientCodeService : IClientCodeService
    {
        private readonly AppDbContext _context;

        public ClientCodeService(AppDbContext context)
        {
            _context = context;
        }

        public string GenerateClientCode(string clientName)
        {
            // Step 1: Clean the name (letters only, uppercase)
            var lettersOnly = Regex.Replace(clientName, "[^A-Za-z ]", "").ToUpperInvariant();

            // Step 2: Split into words
            var words = lettersOnly.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Step 3: Build alpha prefix
            string alpha;
            if (words.Length >= 3)
            {
                // Take first letter of first 3 words
                alpha = string.Concat(words.Take(3).Select(w => w[0]));
            }
            else if (words.Length == 2)
            {
                // First letter of first word + first two letters of second word
                var first = words[0][0].ToString();
                var second = words[1].Length >= 2 ? words[1].Substring(0, 2) : words[1][0] + "A";
                alpha = first + second;
            }
            else
            {



                alpha = (words[0] + "ABCDEFGHIJKLMNOPQRSTUVWXYZ").Substring(0, 3);
            }

            for (int i = 1; i <= 999; i++)
            {
                var candidate = $"{alpha}{i:D3}";

                if (!_context.Clients.Any(c => c.ClientCode == candidate))
                    return candidate;
            }

            throw new InvalidOperationException("Unable to generate unique client code.");
        }

    }
}
