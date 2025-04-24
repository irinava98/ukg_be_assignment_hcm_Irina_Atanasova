namespace People_Management
{
   public class Person
   {
      public int Id { get; set; }

      public string FirstName { get; set; } = null!;

      public string LastName { get; set;} = null!;

      public string Email { get; set; } = null!;

      public string JobTitle { get; set; } = null!;

      public decimal Salary { get; set; }

      public int Age { get; set; }

      public string Department { get; set; } = null!;

      public string HouseAddress { get; set; } = null!;


   }
}
