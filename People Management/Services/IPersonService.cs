namespace People_Management.Services
{
   public interface IPersonService
   {
      Task<IEnumerable<Person>> GetAllPeople();
      Task<Person?> GetPersonById(int id);
      Task<Person> AddPerson(Person person);
      Task<Person?> UpdatePerson(Person person);
      Task<bool> DeletePerson(int id);
   }
}
