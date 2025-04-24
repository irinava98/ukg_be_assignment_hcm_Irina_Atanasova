using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using People_Management;
using People_Management.Services;
using System.Text;

namespace People_Management
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);

         builder.WebHost.UseUrls("https://localhost:7201");

         builder.Services.AddScoped<IPersonService, PersonService>();
         builder.Services.AddScoped<JWTService>();

         builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = builder.Configuration["Jwt:Issuer"],
                  ValidAudience = builder.Configuration["Jwt:Issuer"],
                  IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                  )
               };
            });

         builder.Services.AddControllers();
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
         builder.Services.AddCors(options =>
         {
            options.AddPolicy(name: MyAllowSpecificOrigins,
               policy =>
               {
                  policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
               });
         });

         builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseInMemoryDatabase("PeopleDb"));

         var app = builder.Build();

         // Ensure Swagger UI is setup correctly
         if (app.Environment.IsDevelopment())
         {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
               options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
               options.RoutePrefix = "";  // Swagger UI should be at the root
            });
         }

         // Middleware pipeline
         app.UseHttpsRedirection();
         app.UseCors(MyAllowSpecificOrigins);
         app.UseAuthentication();
         app.UseAuthorization();

         app.MapControllers();

         using (var scope = app.Services.CreateScope())
         {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            DbInitializer.Seed(context);
         }

         
         app.Run();
      }
   }
}
