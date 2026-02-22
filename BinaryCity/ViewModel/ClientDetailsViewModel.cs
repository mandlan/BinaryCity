namespace BinaryCity.ViewModel
{
    public class ClientDetailsViewModel
    {
            public int ClientId { get; set; }

            public string Name { get; set; }

            public string ClientCode { get; set; }

            // Contacts already linked to this client
            public List<ContactListItemViewModel> LinkedContacts { get; set; } = new();

            // Contacts available to be linked
            public List<ContactListItemViewModel> AvailableContacts { get; set; } = new();
        }
    }
