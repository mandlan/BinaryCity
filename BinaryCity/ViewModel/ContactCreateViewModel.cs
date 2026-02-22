using System.ComponentModel.DataAnnotations;

namespace BinaryCity.ViewModel
{
    public class ContactCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public List<int> SelectedClientIds { get; set; } = new();
    }
}
