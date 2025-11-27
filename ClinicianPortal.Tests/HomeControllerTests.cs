using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicianPortal.Controllers;
using ClinicianPortal.Models;
using ClinicianPortal.Models.EntityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


namespace ClinicianPortal.Tests
{
    public class HomeControllerTests
    {
        // Helper to create InMemory database with test data
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
        .Options;

            var context = new AppDbContext(options);

            // Seed test data
            context.Clinician.Add(new Clinician
            {
                Id = 1,
                username = "admin",
                password = "password123",
                create_date = DateTime.Now,
                DOJ = DateTime.Now,
                lastName="test",
                Name="test",
                role = "test"

            }); ;
            context.SaveChanges();

            return context;
        }
        [Fact]
        public void Index_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<HomeController>>().Object;
            var controller = new HomeController(logger, context);

            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Index_Post_ValidCredentials_RedirectsToDashboard()
        {
            var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<HomeController>>().Object;
            var controller = new HomeController(logger, context);

            var loginModel = new LoginModel
            {

                Username = "admin",
                Password = "password123"
            };

            var result = controller.Index(loginModel) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Dashboard", result.ControllerName);
        }
        [Fact]
        public void Index_Post_InvalidCredentials_ReturnsViewWithError()
        {
            var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<HomeController>>().Object;
            var controller = new HomeController(logger, context);

            var loginModel = new LoginModel
            {
                Username = "wronguser",
                Password = "wrongpassword"
            };

            var result = controller.Index(loginModel) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(string.Empty));
            Assert.Equal("Invalid username or password", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }



    }
}
