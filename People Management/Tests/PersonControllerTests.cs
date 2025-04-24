using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using People_Management.Controllers;
using People_Management.Services;
using System.Security.Claims;

namespace People_Management.Tests
{
   [TestFixture]
   public class PersonControllerTests
   {
      private Mock<IPersonService> _mockService;
      private PersonController _controller;

      [SetUp]
      public void SetUp()
      {
         _mockService = new Mock<IPersonService>();
         _controller = new PersonController(_mockService.Object);

         var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Manager")
         }, "mock"));

         _controller.ControllerContext = new ControllerContext
         {
            HttpContext = new DefaultHttpContext { User = user }
         };
      }

      private Person CreatePerson(int id, string department)
      {
         return new Person
         {
            Id = id,
            FirstName = $"First{id}",
            LastName = $"Last{id}",
            Email = $"user{id}@example.com",
            JobTitle = "Developer",
            Salary = 50000 + id,
            Age = 30,
            Department = department,
            HouseAddress = $"123{id} Main St"
         };
      }

      [Test]
      public async Task Get_WhenManagerLoggedIn_ReturnsOnlyPeopleInSameDepartment()
      {
         var manager = CreatePerson(1, "HR");
         var people = new List<Person>
         {
            CreatePerson(2, "HR"),
            CreatePerson(3, "IT")
         };

         _mockService.Setup(s => s.GetPersonById(1)).ReturnsAsync(manager);
         _mockService.Setup(s => s.GetAllPeople()).ReturnsAsync(people);

         var result = await _controller.Get();

         var okResult = result as OkObjectResult;
         Assert.IsNotNull(okResult);

         var returnedPeople = okResult.Value as IEnumerable<Person>;
         Assert.That(returnedPeople, Has.Exactly(1).Items);
      }

      [Test]
      public async Task Post_WhenHRAdminLoggedIn_CreatesPersonSuccessfully()
      {
         _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "HRAdmin")
         }));

         var newPerson = CreatePerson(10, "Finance");

         _mockService.Setup(s => s.AddPerson(It.IsAny<Person>())).ReturnsAsync(newPerson);

         var result = await _controller.Post(newPerson);

         var createdResult = result as CreatedAtActionResult;
         Assert.IsNotNull(createdResult);

         var person = createdResult.Value as Person;
         Assert.IsNotNull(person);
         Assert.AreEqual(10, person.Id);
         Assert.AreEqual("Finance", person.Department);
      }

      [Test]
      public async Task GetById_WhenEmployeeAccessesOtherProfile_ReturnsForbid()
      {
         _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, "2"),
            new Claim(ClaimTypes.Role, "Employee")
         }));

         var result = await _controller.GetById(3);

         Assert.IsInstanceOf<ForbidResult>(result);
      }

      [Test]
      public async Task Delete_WhenHRAdminDeletesExistingPerson_ReturnsNoContent()
      {
         _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
         {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "HRAdmin")
         }));

         _mockService.Setup(s => s.DeletePerson(5)).ReturnsAsync(true);

         var result = await _controller.Delete(5);

         Assert.IsInstanceOf<NoContentResult>(result);
      }
   }
}
