using System.ComponentModel.DataAnnotations;

namespace BinaryCity.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]

        public string Email { get; set; }

        public ICollection<ClientContact> ClientContacts { get; set; }

    }
}
