using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using at_aspnet.Models;

namespace at_aspnet.Controllers
{
    public class PersonController : Controller
    {
        public static List<Person> People { get; set; } = new List<Person>();
        public static IEnumerable<Person> ResultList;

        public IActionResult Index(string? message, string? type)
        {
            ViewBag.message = message;
            ViewBag.type = type;
            return View(People);
        }

        public IActionResult New()
        {
            return View();
        }

        public IActionResult Update([FromQuery] Guid id)
        {
            var person = People.Where(x => x.Id == id).FirstOrDefault();
            return View(person);
        }

        [HttpPost]
        public IActionResult Save(Person model)
        {
            if (!ModelState.IsValid) { return View(); }
            model.Id = Guid.NewGuid();
            People.Add(model);
            return RedirectToAction("Index", "Person", new { message = "Person added.", type = "alert-success" });
        }

        [HttpPost]
        public IActionResult SaveUpdate(Guid id, Person model)
        {
            if (!ModelState.IsValid) { return View(); }

            var updatedPerson = People.Where(x => x.Id == id).FirstOrDefault();
            updatedPerson.FirstName = model.FirstName;
            updatedPerson.Surname = model.Surname;
            updatedPerson.Birthday = model.Birthday;

            People.Remove(updatedPerson);
            People.Add(updatedPerson);

            return RedirectToAction("Index", "Person", new { message = "Person edited.", type = "alert-warning" });
        }

        public IActionResult Delete([FromQuery] Guid id)
        {
            var person = People.Where(x => x.Id == id).FirstOrDefault();
            People.Remove(person);
            return RedirectToAction("Index", "Person", new { message = "Person deleted.", type = "alert-danger" });
        }

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchPeople(Person model)
        {
            ResultList = SearchFor(model.FirstName, model.Surname);
            return RedirectToAction("Result", "Person", new { message = $"Found {ResultList.Count()} contact(s)", type = "alert-dark" });
        }

        public IEnumerable<Person> SearchFor(string termFirstName, string termSurname)
        {
            return People.Where(person =>
                person.FirstName.Contains(termFirstName, StringComparison.InvariantCultureIgnoreCase) ||
                person.Surname.Contains(termSurname, StringComparison.InvariantCultureIgnoreCase));
        }

        public IActionResult Result(string? message, string? type)
        {
            ViewBag.message = message;
            ViewBag.type = type;
            return View(ResultList);
        }
    }
}