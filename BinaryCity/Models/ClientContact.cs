using System.ComponentModel.DataAnnotations;

namespace BinaryCity.Models
{
    public class ClientContact
    {
        public int ClientId { get; set; }
        public int ContactId { get; set; }
        // Navigation properties
        public Client Client { get; set; }
        public Contact Contact { get; set; }
    }
}
