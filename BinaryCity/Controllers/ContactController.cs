using BinaryCity.Data;
using BinaryCity.Models;
using BinaryCity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ContactController : Controller
{
    private readonly AppDbContext _context;

    public ContactController(AppDbContext context)
    {
        _context = context;
    }

    // List all contacts
    public IActionResult Index()
    {
        var contacts = _context.Contacts
            .Include(c => c.ClientContacts)
            .ThenInclude(cc => cc.Client)
            .OrderBy(c => c.Surname)
            .ThenBy(c => c.Name)
            .Select(c => new ContactListItemViewModel
            {
                ContactId = c.ContactId,
                Name = c.Name,
                Surname = c.Surname,
                Email = c.Email,
                LinkedClientsCount = c.ClientContacts.Count

            })
            .ToList();


        return View(contacts);
    }

    // Show create form
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Clients"] = _context.Clients.OrderBy(c => c.Name).ToList();
        return View();
    }

    // Handle form submission
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ContactCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Clients"] = _context.Clients.OrderBy(c => c.Name).ToList();
            return View(model);
        }

        if (_context.Contacts.Any(c => c.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email must be unique.");
            ViewData["Clients"] = _context.Clients.OrderBy(c => c.Name).ToList();
            return View(model);
        }

        var contact = new Contact
        {
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email
        };

        _context.Contacts.Add(contact);
        _context.SaveChanges();

        // Link clients
        foreach (var clientId in model.SelectedClientIds)
        {
            _context.ClientContacts.Add(new ClientContact
            {
                ContactId = contact.ContactId,
                ClientId = clientId
            });
        }

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // Existing non-AJAX link/unlink (kept for progressive enhancement)
    [HttpPost]
    public IActionResult LinkClient(int contactId, int clientId)
    {
        if (!_context.Contacts.Any(c => c.ContactId == contactId)) return NotFound();
        if (!_context.Clients.Any(c => c.ClientId == clientId)) return NotFound();

        var exists = _context.ClientContacts.Any(cc => cc.ContactId == contactId && cc.ClientId == clientId);
        if (!exists)
        {
            _context.ClientContacts.Add(new ClientContact { ContactId = contactId, ClientId = clientId });
            _context.SaveChanges();
        }

        return RedirectToAction("Details", new { id = contactId });
    }

    public IActionResult Unlink(int contactId, int clientId)
    {
        var link = _context.ClientContacts
            .FirstOrDefault(cc => cc.ContactId == contactId && cc.ClientId == clientId);

        if (link != null)
        {
            _context.ClientContacts.Remove(link);
            _context.SaveChanges();
        }

        return RedirectToAction("Details", new { id = contactId });
    }

    public IActionResult Details(int id)
    {
        var contact = _context.Contacts
            .Include(c => c.ClientContacts)
            .ThenInclude(cc => cc.Client)
            .FirstOrDefault(c => c.ContactId == id);

        if (contact == null)
            return NotFound();

        var linkedClients = contact.ClientContacts
            .Select(cc => cc.Client)
            .OrderBy(c => c.Name)
            .Select(c => new ClientListItemViewModel
            {
                ClientId = c.ClientId,
                Name = c.Name,
                ClientCode = c.ClientCode
            })
            .ToList();

        var allClients = _context.Clients
            .OrderBy(c => c.Name)
            .ToList();

        var availableClients = allClients
            .Where(c => !linkedClients.Any(l => l.ClientId == c.ClientId))
            .Select(c => new ClientListItemViewModel
            {
                ClientId = c.ClientId,
                Name = c.Name,
                ClientCode = c.ClientCode
            })
            .ToList();

        var vm = new ContactDetailsViewModel
        {
            ContactId = contact.ContactId,
            Name = contact.Name,
            Surname = contact.Surname,
            Email = contact.Email,
            LinkedClients = linkedClients,
            AvailableClients = availableClients
        };

        return View(vm);
    }
    // AJAX endpoints for bonus points
    [HttpPost]
    public JsonResult LinkClientAjax([FromBody] ContactLinkRequestViewModel model)
    {
        if (!_context.Contacts.Any(c => c.ContactId ==  model.ContactId) ||
                !_context.Clients.Any(c => c.ClientId == model.ClientId))
            return Json(new { success = false, message = "Not found" });

        var exists = _context.ClientContacts.Any(cc => cc.ContactId == model.ContactId && cc.ClientId == model.ClientId);
        if (!exists)
        {
            _context.ClientContacts.Add(new ClientContact { ContactId = model.ContactId, ClientId = model.ClientId });
            _context.SaveChanges();
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public JsonResult UnlinkClientAjax([FromBody] ContactLinkRequestViewModel model)
    {
        var link = _context.ClientContacts.FirstOrDefault(cc => cc.ContactId == model.ContactId && cc.ClientId == model.ClientId);
        if (link != null)
        {
            _context.ClientContacts.Remove(link);
            _context.SaveChanges();
        }

        return Json(new { success = true });
    }

    // Email uniqueness check for client-side validation
    [HttpGet]
    public JsonResult CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return Json(new { isUnique = false });
        var exists = _context.Contacts.Any(c => c.Email.ToLower() == email.ToLower());
        return Json(new { isUnique = !exists });
    }
}