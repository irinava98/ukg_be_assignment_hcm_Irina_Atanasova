namespace People_Management
{
   public class DbInitializer
   {
      public static void Seed(ApplicationDbContext context)
      {
         if (!context.People.Any())
         {
            context.People.AddRange(
                  new Person
                  {
                     FirstName = "Alice",
                     LastName = "Johnson",
                     Email = "alice.johnson@yahoo.com",
                     JobTitle = "Software Engineer",
                     Salary = 75000,
                     Age = 29,  
                     Department = "IT",
                     HouseAddress = "123 Maple Street, Springfield"  
                  },
                    new Person
                    {
                       FirstName = "Bob",
                       LastName = "Smith",
                       Email = "bob.smith@yahoo.com",
                       JobTitle = "Project Manager",
                       Salary = 85000,
                       Age = 45,  
                       Department = "Management",
                       HouseAddress = "456 Oak Avenue, Springfield"  
                    },
                    new Person
                    {
                       FirstName = "Charlie",
                       LastName = "Davis",
                       Email = "charlie.davis@yahoo.com",
                       JobTitle = "HR Specialist",
                       Salary = 60000,
                       Age = 38,  
                       Department = "HR",
                       HouseAddress = "789 Pine Road, Springfield"  
                    },
                    new Person
                    {
                       FirstName = "Dana",
                       LastName = "Evans",
                       Email = "dana.evans@yahoo.com",
                       JobTitle = "Software Engineer",
                       Salary = 75000,
                       Age = 30,  
                       Department = "IT",
                       HouseAddress = "101 Birch Street, Springfield"  
                    },
                    new Person
                    {
                       FirstName = "Eve",
                       LastName = "Miller",
                       Email = "eve.miller@yahoo.com",
                       JobTitle = "HR Specialist",
                       Salary = 60000,
                       Age = 32,  
                       Department = "HR",
                       HouseAddress = "202 Cedar Lane, Springfield"  
                    }
               );

            context.SaveChanges();

            Console.WriteLine("Database seeded with the following people:");
            foreach (var person in context.People)
            {
               Console.WriteLine($"Id: {person.Id}, Name: {person.FirstName} {person.LastName}, Email: {person.Email}, Job Title: {person.JobTitle}, Department: {person.Department}, Age: {person.Age}, House Address: {person.HouseAddress}");
            }
         }
      }
   }
}
