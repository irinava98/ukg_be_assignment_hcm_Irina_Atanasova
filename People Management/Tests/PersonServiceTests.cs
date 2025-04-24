using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using People_Management.Services;


namespace People_Management.Tests
{
   [TestFixture]
   public class PersonServiceCrudTests
   {
      private ApplicationDbContext _context;
      private PersonService _service;

      [SetUp]
      public void Setup()
      {
         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
             .UseInMemoryDatabase(databaseName: "PeopleManagementTestDb") 
             .Options;

         _context = new ApplicationDbContext(options);
         _context.Database.EnsureCreated(); 
         _service = new PersonService(_context);
      }

      [TearDown]
      public void TearDown()
      {
         _context.Database.EnsureDeleted(); 
         _context.Dispose();
      }

      [Test]
      public async Task CreatePerson_AddsPersonToDatabase()
      {
         // Arrange
         var person = new Person
         {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",  
            JobTitle = "Dev",           
            Age = 25,
            Department = "IT",          
            Salary = 50000,
            HouseAddress = "123 Test St"  
         };

         // Act
         var result = await _service.AddPerson(person);

         // Assert
         Assert.That(await _context.People.CountAsync(), Is.EqualTo(1));  // Expecting 1 person in DB
         Assert.That(result.FirstName, Is.EqualTo(person.FirstName));  
      }

      [Test]
      public async Task ReadPerson_ReturnsCorrectPerson()
      {
         // Arrange
         var person = new Person
         {
            FirstName = "Read",
            LastName = "Test",
            Email = "readtest@example.com", 
            JobTitle = "Tester",             
            Department = "QA",               
            Age = 30,
            Salary = 40000,
            HouseAddress = "456 Read St"    
         };
         _context.People.Add(person);
         await _context.SaveChangesAsync();

         // Act
         var result = await _service.GetPersonById(person.Id);

         // Assert
         Assert.That(result, Is.Not.Null);
         Assert.That(result!.FirstName, Is.EqualTo("Read"));  
      }

      [Test]
      public async Task ReadPerson_ReturnsNullIfPersonNotFound()
      {
         // Act
         var result = await _service.GetPersonById(999);  

         // Assert
         Assert.That(result, Is.Null);  
      }

      [Test]
      public async Task UpdatePerson_ModifiesExistingData()
      {
         // Arrange
         var person = new Person
         {
            FirstName = "Old",
            LastName = "Name",
            Email = "oldname@example.com", 
            JobTitle = "Junior",           
            Department = "Dev",            
            Age = 22,
            Salary = 35000,
            HouseAddress = "123 Old St"    
         };
         _context.People.Add(person);
         await _context.SaveChangesAsync();

         person.FirstName = "New";
         person.LastName = "Name";

         // Act
         var result = await _service.UpdatePerson(person);

         // Assert
         Assert.That(result, Is.Not.Null);  
         Assert.That(result!.FirstName, Is.EqualTo("New"));  
      }

      [Test]
      public async Task UpdatePerson_ReturnsNullIfPersonNotFound()
      {
         // Arrange
         var person = new Person
         {
            Id = 999,
            FirstName = "NonExistent",
            LastName = "Person",
            Email = "nonexistent@example.com", 
            JobTitle = "Unknown",              
            Department = "None",               
            Age = 0,
            Salary = 0,
            HouseAddress = "Nowhere"           
         };

         // Act
         var result = await _service.UpdatePerson(person);

         // Assert
         Assert.That(result, Is.Null);  
      }

      [Test]
      public async Task DeletePerson_RemovesPerson()
      {
         // Arrange
         var person = new Person
         {
            FirstName = "Delete",
            LastName = "Me",
            Email = "delete@example.com", 
            JobTitle = "Dev",             
            Department = "IT",            
            Age = 25,
            Salary = 50000,
            HouseAddress = "123 Delete St"  
         };
         _context.People.Add(person);
         await _context.SaveChangesAsync();

         // Act
         var success = await _service.DeletePerson(person.Id);
         var deleted = await _context.People.FindAsync(person.Id);

         // Assert
         Assert.That(success, Is.True);  
         Assert.That(deleted, Is.Null);  
      }

      [Test]
      public async Task DeletePerson_ReturnsFalseIfPersonNotFound()
      {
         // Act
         var success = await _service.DeletePerson(999);  

         // Assert
         Assert.That(success, Is.False);  
      }
   }
}
