using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

public class ClientsController : Controller
{
    private readonly ClinicService _service = new();

    public IActionResult Index(string? search)
    {
        ViewBag.Search = search;
        var clients = string.IsNullOrWhiteSpace(search)
            ? _service.GetAllClients()
            : _service.SearchClients(search);
        return View(clients);
    }

    public IActionResult Create() => View(new Client());

    [HttpPost]
    public IActionResult Create(Client client)
    {
        if (!ModelState.IsValid) return View(client);
        _service.AddClient(client);
        TempData["Success"] = "Client added successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var clients = _service.GetAllClients();
        var client = clients.FirstOrDefault(c => c.Id == id);
        if (client == null) return NotFound();
        return View(client);
    }

    [HttpPost]
    public IActionResult Edit(Client client)
    {
        if (!ModelState.IsValid) return View(client);
        _service.UpdateClient(client);
        TempData["Success"] = "Client updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        _service.DeleteClient(id);
        TempData["Success"] = "Client deleted.";
        return RedirectToAction(nameof(Index));
    }
}
