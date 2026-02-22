using BinaryCity.Data;
using BinaryCity.Models;
using BinaryCity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class ClientController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClientController> _logger;

    public ClientController(AppDbContext context, ILogger<ClientController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // LIST
    public IActionResult Index()
    {
        var clients = _context.Clients
            .Include(c => c.ClientContacts)
            .ThenInclude(cc => cc.Contact)
            .OrderBy(c => c.Name)
            .Select(c => new ClientListItemViewModel
            {
            ClientId = c.ClientId,
            Name = c.Name,
            ClientCode = c.ClientCode,
            LinkedContactsCount = c.ClientContacts.Count
        })
            .ToList();

      

        return View(clients);
    }

    // CREATE (GET)
    [HttpGet]
    public IActionResult Create()
    {
        return View(new ClientCreateViewModel());
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ClientCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return IsAjaxRequest()
                ? BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) })
                : View(model);
        }

        var name = model.Name.Trim();

        var client = new Client
        {
            Name = name,
            ClientCode = GenerateClientCode(name)
        };

        try
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");

            if (IsAjaxRequest())
                return StatusCode(500, new { success = false, message = "Server error saving client" });

            ModelState.AddModelError("", "An unexpected server error occurred.");
            return View(model);
        }

        if (IsAjaxRequest())
            return Json(new { success = true, redirectUrl = Url.Action("Index") });

        return RedirectToAction(nameof(Index));
    }

    // DETAILS
    public IActionResult Details(int id)
    {
        var client = _context.Clients
            .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Contact)
            .FirstOrDefault(c => c.ClientId == id);

        if (client == null)
            return NotFound();

        var linkedContacts = client.ClientContacts
            .Select(cc => cc.Contact)
            .ToList();

        var availableContacts = _context.Contacts
            .Where(c => !linkedContacts.Select(l => l.ContactId).Contains(c.ContactId))
            .ToList();

        var vm = new ClientDetailsViewModel
        {
            ClientId = client.ClientId,
            Name = client.Name,
            ClientCode = client.ClientCode,

            LinkedContacts = linkedContacts.Select(c => new ContactListItemViewModel
            {
                ContactId = c.ContactId,
                Name = c.Name,
                Surname = c.Surname,
                Email = c.Email
            }).OrderBy(c => c.Name).ToList(),

            AvailableContacts = availableContacts.Select(c => new ContactListItemViewModel
            {
                ContactId = c.ContactId,
                Name = c.Name,
                Surname = c.Surname,
                Email = c.Email
            }).OrderBy(c => c.FullName).ToList()
        };

        return View(vm);
    }

    // LINK / UNLINK
    [HttpPost]
    public IActionResult LinkContact(int clientId, int contactId)
    {
        if (!_context.ClientContacts.Any(cc => cc.ClientId == clientId && cc.ContactId == contactId))
        {
            _context.ClientContacts.Add(new ClientContact { ClientId = clientId, ContactId = contactId });
            _context.SaveChanges();
        }

        return RedirectToAction("Details", new { id = clientId });
    }

    [HttpPost]
    public IActionResult Unlink(int clientId, int contactId)
    {
        var link = _context.ClientContacts.FirstOrDefault(cc => cc.ClientId == clientId && cc.ContactId == contactId);
        if (link != null)
        {
            _context.ClientContacts.Remove(link);
            _context.SaveChanges();
        }

        return RedirectToAction("Details", new { id = clientId });
    }

    // CLIENT CODE GENERATION
    private string GenerateClientCode(string clientName)
    {
        var lettersOnly = Regex.Replace(clientName, "[^A-Za-z]", "").ToUpperInvariant();
        var alpha = (lettersOnly + "ABCDEFGHIJKLMNOPQRSTUVWXYZ").Substring(0, 3);

        for (int i = 1; i <= 999; i++)
        {
            var candidate = $"{alpha}{i:D3}";
            if (!_context.Clients.Any(c => c.ClientCode == candidate))
                return candidate;
        }

        throw new InvalidOperationException("Unable to generate unique client code.");
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}