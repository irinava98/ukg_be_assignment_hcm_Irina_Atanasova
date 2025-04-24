namespace People_Management
{
   public class AppUser
   {
      public int Id { get; set; }

      public string Username { get; set; } = null!;

      public string Password { get; set; } = null!;

      public string Role { get; set; } = null!;

      public int? PersonId { get; set; }

      public Person? Person { get; set; }
   }
}
