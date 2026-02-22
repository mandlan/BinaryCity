using System.ComponentModel.DataAnnotations;
namespace BinaryCity.Models

{
    public class Client
    {
        public int ClientId { get; set; }
        [Required]
        public string Name { get; set; }
        public string ClientCode { get; set; }
        public ICollection<ClientContact> ClientContacts { get; set; }

    }
}
