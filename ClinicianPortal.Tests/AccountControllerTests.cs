using ClinicianPortal.Controllers;
using ClinicianPortal.Models;
using ClinicianPortal.Models.EntityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicianPortal.Controllers;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ClinicianPortal.Tests
{
    public class AccountControllerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB for each test
                .Options;

            return new AppDbContext(options);
        }      
        //[Fact]
        //public void SignUp_ValidModel_Redirects()
        //{
        //    var context = GetInMemoryDbContext();
        //    var controller = new AccountController(context);

        //    var model = new SignUpModel
        //    {
        //        Username = "john",
        //        Password = "123",
        //        Name = "john",
        //        LastName = "Willson"
        //    };

        //    var result = controller.SignUp(model);

        //    var redirect = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirect.ActionName);
        //    Assert.Equal("Login", redirect.ControllerName);
        //}
        [Fact]
        public void SignUp_ValidModel_SavesUser()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "john",
                Password = "123",
                Name = "John",
                LastName = "willson"
            };

            controller.SignUp(model);

            var user = context.Clinician.Find(1); // first inserted user
            Assert.NotNull(user);
            Assert.Equal("john", user.username);
        }
        [Fact]
        public void SignUp_InvalidModel_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);
            controller.ModelState.AddModelError("Username", "Required");

            var model = new SignUpModel(); // empty model
            var result = controller.SignUp(model);

            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void SignUp_EmptyUsername_ModelStateError()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);
            controller.ModelState.AddModelError("Username", "Required");

            var model = new SignUpModel
            {
                Password = "123",
                Name = "John",
                LastName = "Willson"
            };

            var result = controller.SignUp(model);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void SignUp_EmptyPassword_ModelStateError()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);
            controller.ModelState.AddModelError("Password", "Required");

            var model = new SignUpModel
            {
                Username = "John",
                Name = "John",
                LastName = "Willson"
            };

            var result = controller.SignUp(model);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void SignUp_EmptyForm_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);
            controller.ModelState.AddModelError("Username", "Required");
            controller.ModelState.AddModelError("Password", "Required");

            var model = new SignUpModel();
            var result = controller.SignUp(model);

            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void SignUp_ValidModel_SetsRole()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "John",
                Password = "123",
                Name = "john",
                LastName = "willson"
            };

            controller.SignUp(model);

            var user = context.Clinician.Find(1);
            Assert.Equal("CLINICIAN", user.role);
        }
        [Fact]
        public void SignUp_ValidModel_SetsCreateDate()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "john",
                Password = "123",
                Name = "john",
                LastName = "willson"
            };

            controller.SignUp(model);

            var user = context.Clinician.Find(1);
            Assert.True(user.create_date <= DateTime.Now);
        }
        [Fact]
        public void SignUp_ValidModel_SetsDOJ()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "john",
                Password = "123",
                Name = "john",
                LastName = "willson"
            };

            controller.SignUp(model);

            var user = context.Clinician.Find(1);
            Assert.True(user.DOJ <= DateTime.Now);
        }

        [Fact]
        public void SignUp_ValidModel_IncreasesUserCount()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "alice",
                Password = "password",
                Name = "Alice",
                LastName = "Smith"
            };

            controller.SignUp(model);

            var userCount = context.Clinician.CountAsync().Result;
            Assert.Equal(1, userCount);
        }
        [Fact]
        public void SignUp_ValidModel_UsernameSavedCorrectly()
        {
            var context = GetInMemoryDbContext();
            var controller = new AccountController(context);

            var model = new SignUpModel
            {
                Username = "carol",
                Password = "1234",
                Name = "Carol",
                LastName = "Williams"
            };

            controller.SignUp(model);

            var user = context.Clinician.FirstOrDefaultAsync().Result;
            Assert.NotNull(user);
            Assert.Equal("carol", user.username);
        }

    }
}
