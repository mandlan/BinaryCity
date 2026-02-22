namespace BinaryCity.ViewModel
{
    public class ContactListItemViewModel
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Surname}{Name}";
        public string Email { get; set; }
        public int LinkedClientsCount { get; set; }
    }
}
