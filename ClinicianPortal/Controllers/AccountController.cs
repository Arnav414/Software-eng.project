using ClinicianPortal.Models;
using Microsoft.AspNetCore.Mvc;
using ClinicianPortal.Models.EntityModel;

namespace ClinicianPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        //public AccountController(int a=10,int b=20) { }
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                // Insert using LINQ
                var user = new Clinician()
                {
                    username = model.Username,
                    password = model.Password,
                    Name = model.Name,
                    lastName = model.LastName,
                    role= "CLINICIAN",
                    create_date= DateTime.Now,
                    DOJ=DateTime.Now
                };
                _context.Clinician.Add(user);
                _context.SaveChanges(); // <- Saves to table
                // Redirect to login page
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
