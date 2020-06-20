using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using at_aspnet.Models;
using at_aspnet.Repository;

namespace at_aspnet.Controllers
{
    public class PersonController : Controller
    {

        private PeopleRepository PeopleRepository { get; set; }

        public PersonController(PeopleRepository peopleRepository) { this.PeopleRepository = peopleRepository; }

        public static List<Person> People { get; set; } = new List<Person>();
        public static List<Person> ResultList;

        public IActionResult Index(string? message, string? type)
        {
            // READ
            var peopleList = this.PeopleRepository.GetAll();
            List<Person> todayBirthdays = new List<Person>();
            List<Person> nextBirthdays = new List<Person>();

            TodayBirthdays(this.PeopleRepository.GetAll());
            nextBirthdays = NextBirthdays(this.PeopleRepository.GetAll());
            nextBirthdays.Sort((x, y) => x.Birthday.CompareTo(y.Birthday));

            void TodayBirthdays(List<Person> peopleList)
            {
                foreach (var p in peopleList)
                {
                    if (p.Birthday.Day == DateTime.Now.Day && p.Birthday.Month == DateTime.Now.Month) { todayBirthdays.Add(p); }
                }
            }

            List<Person> NextBirthdays(List<Person> peopleList)
            {
                List<Person> result = new List<Person>();
                foreach (var p in peopleList)
                {
                    var date = p.Birthday;
                    var year = date.Month < DateTime.Now.Month ||
                               (date.Month == DateTime.Now.Month && date.Day <= DateTime.Now.Day) ?
                                    DateTime.Now.Year + 1 : DateTime.Now.Year;
                    p.Birthday = new DateTime(year, date.Month, date.Day);
                    p.Age = p.Age + 1;
                    result.Add(p);
                }
                return result;
            }

            ViewBag.todayBirthdays = todayBirthdays;
            ViewBag.nextBirthdays = nextBirthdays;
            ViewBag.message = message;
            ViewBag.type = type;
            return View(peopleList);
        }

        public IActionResult New()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Result(string? message, string? type)
        {
            ViewBag.message = message;
            ViewBag.type = type;
            return View(ResultList);
        }

        public IActionResult List(string? message, string? type) 
        {
            ViewBag.message = message;
            ViewBag.type = type;
            return View(this.PeopleRepository.GetAll());
        }

        // CREATE
        [HttpPost]
        public IActionResult Save(Person model)
        {
            if (!ModelState.IsValid) { return View(); }
            var p = model;
            model.Id = Guid.NewGuid();
            model.Age = CalculateAge(p);
            PeopleRepository.Save(model);
            return RedirectToAction("List", "Person", new { message = "Person added.", type = "alert-success" });
        }

        // Save contact and UPDATE
        [HttpPost]
        public IActionResult SaveUpdate(Guid id, Person model)
        {
            if (!ModelState.IsValid) { return View(); }

            var updatedPerson = PeopleRepository.GetById(id);
            updatedPerson.FirstName = model.FirstName;
            updatedPerson.Surname = model.Surname;
            updatedPerson.Birthday = model.Birthday;
            updatedPerson.Age = CalculateAge(model);

            PeopleRepository.Update(updatedPerson);
            return RedirectToAction("List", "Person", new { message = "Person edited.", type = "alert-warning" });
        }

        // UPDATE - Search for specific person object
        public IActionResult Update([FromQuery] Guid id)
        {
            var people = PeopleRepository.GetById(id);
            return View(people);
        }

        // DELETE PERSON
        public IActionResult Delete([FromQuery] Guid id)
        {
            if (!ModelState.IsValid) { return View(); }
            PeopleRepository.Delete(id);
            return RedirectToAction("List", "Person", new { message = "Person deleted.", type = "alert-danger" });
        }

        private static int CalculateAge(Person person)
        {
            var difference = DateTime.Now.Year - person.Birthday.Year;
            var temp = new DateTime(DateTime.Now.Year, person.Birthday.Month, person.Birthday.Day).Date;
            if (temp.Date > DateTime.Now.Date) { return (difference - 1); }
            else { return difference; }
        }

        // Search for person  
        [HttpPost]
        public IActionResult SearchPeople(Person model)
        {
            ResultList = SearchFor(model.FirstName, model.Surname);
            return RedirectToAction("Result", "Person", new { message = $"Found {ResultList.Count()} contact(s)", type = "alert-dark" });
        }

        public List<Person> SearchFor(string termFirstName, string termSurname)
        {
            return PeopleRepository.SearchPeopleDatabase(termFirstName, termSurname);
        }
    }
}