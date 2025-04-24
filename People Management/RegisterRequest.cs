namespace People_Management
{
   public class RegisterRequest
   {
         public string Username { get; set; } = null!;
         public string Password { get; set; } = null!;
         public string Role { get; set; } = null!;
         public int? PersonId { get; set; }  
      
   }

}

