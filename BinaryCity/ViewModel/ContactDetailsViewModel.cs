namespace BinaryCity.ViewModel
{
    public class ContactDetailsViewModel
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        // Clients already linked to this contact
        public List<ClientListItemViewModel> LinkedClients { get; set; } = new();
        // Clients available to be linked
        public List<ClientListItemViewModel> AvailableClients { get; set; } = new();
    }
}
