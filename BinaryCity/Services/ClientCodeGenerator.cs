using BinaryCity.Data;

namespace BinaryCity.Services
{
    public class ClientCodeGenerator
    {
        private readonly AppDbContext _context;

        public ClientCodeGenerator(AppDbContext context)
        {
            _context = context;
        }

        public string GenerateCode(string clientName)
        {
            string alphaPart = clientName.Length >= 3
                ? new string(clientName.Take(3).ToArray()).ToUpper()
                : clientName.ToUpper().PadRight(3, 'A');

            int counter = 1;
            string code;
            do
            {
                code = $"{alphaPart}{counter.ToString("D3")}";
                counter++;
            } while (_context.Clients.Any(c => c.ClientCode == code));

            return code;
        }
    }
}
