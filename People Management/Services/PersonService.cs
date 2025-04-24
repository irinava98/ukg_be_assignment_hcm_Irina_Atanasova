using Microsoft.EntityFrameworkCore;

namespace People_Management.Services
{
   public class PersonService : IPersonService
   {
      private readonly ApplicationDbContext _context;

      public PersonService(ApplicationDbContext context)
      {
         _context = context;
      }

      public async Task<IEnumerable<Person>> GetAllPeople()
      {
         return await _context.People.ToListAsync();
      }

      public async Task<Person?> GetPersonById(int id)
      {
         return await _context.People.FindAsync(id);
      }

      public async Task<Person> AddPerson(Person person)
      {
         await _context.People.AddAsync(person);
         await _context.SaveChangesAsync();
         return person;
      }

      public async Task<Person?> UpdatePerson(Person person)
      {
         var existingPerson = await _context.People.FirstOrDefaultAsync(p => p.Id == person.Id);

         if (existingPerson == null)
            return null;

         existingPerson.FirstName = person.FirstName;
         existingPerson.LastName = person.LastName;
         existingPerson.Email = person.Email;
         existingPerson.JobTitle = person.JobTitle;
         existingPerson.Department = person.Department;
         existingPerson.Age = person.Age;
         existingPerson.Salary = person.Salary;
         existingPerson.HouseAddress = person.HouseAddress;

         await _context.SaveChangesAsync();
         return existingPerson;
      }

      public async Task<bool> DeletePerson(int id)
      {
         var personToDelete = await _context.People.FindAsync(id);

         if (personToDelete != null)
         {
            _context.People.Remove(personToDelete);
            await _context.SaveChangesAsync();
            return true;
         }

         return false;
      }
   }
}
