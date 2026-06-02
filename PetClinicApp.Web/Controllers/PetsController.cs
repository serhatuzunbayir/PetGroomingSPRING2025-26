using Microsoft.AspNetCore.Mvc;
using PetClinicApp.Core.Models;
using PetClinicApp.Core.Services;

namespace PetClinicApp.Web.Controllers;

public class PetsController : Controller
{
    private readonly ClinicService _service = new();

    public IActionResult Index()
    {
        var pets = _service.GetAllPets();
        return View(pets);
    }

    public IActionResult Create()
    {
        ViewBag.Clients = _service.GetAllClients();
        return View(new Pet());
    }

    [HttpPost]
    public IActionResult Create(Pet pet)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clients = _service.GetAllClients();
            return View(pet);
        }
        _service.AddPet(pet);
        TempData["Success"] = "Pet added successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var pets = _service.GetAllPets();
        var pet = pets.FirstOrDefault(p => p.Id == id);
        if (pet == null) return NotFound();
        ViewBag.Clients = _service.GetAllClients();
        return View(pet);
    }

    [HttpPost]
    public IActionResult Edit(Pet pet)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clients = _service.GetAllClients();
            return View(pet);
        }
        _service.UpdatePet(pet);
        TempData["Success"] = "Pet updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        _service.DeletePet(id);
        TempData["Success"] = "Pet deleted.";
        return RedirectToAction(nameof(Index));
    }
}
