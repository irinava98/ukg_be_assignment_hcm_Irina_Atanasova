using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using People_Management.Services;
using System.Security.Claims;

namespace People_Management.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class PersonController : ControllerBase
   {
      private readonly IPersonService _personService;

      public PersonController(IPersonService personService)
      {
         _personService = personService;
      }

      [Authorize(Roles = "HRAdmin,Manager")]
      [HttpGet]
      public async Task<IActionResult> Get()
      {
         var allPeople = await _personService.GetAllPeople();
         var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
         var userPersonId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

         if (userRole == "Manager")
         {
            var manager = await _personService.GetPersonById(userPersonId);
            if (manager == null)
            {
               return Forbid();
            }

            var dept = manager.Department;
            var filtered = allPeople.Where(p => p.Department == dept);
            return Ok(filtered);
         }

         return Ok(allPeople);
      }

      [Authorize]
      [HttpGet("{id}")]
      public async Task<IActionResult> GetById(int id)
      {
         var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
         var userPersonId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

         if (userRole == "Employee" && userPersonId != id)
         {
            return Forbid("Employees can only access their own profile.");
         }

         if (userRole == "Manager")
         {
            var person = await _personService.GetPersonById(id);
            var manager = await _personService.GetPersonById(userPersonId);

            if (person == null || person.Department != manager?.Department)
            {
               return Forbid("Managers can only access profiles in their department.");
            }

            return Ok(person);
         }

         var result = await _personService.GetPersonById(id);
         return result == null ? NotFound() : Ok(result);
      }

      [Authorize(Roles = "HRAdmin")]
      [HttpPost]
      public async Task<IActionResult> Post([FromBody] Person person)
      {
         if (person == null)
            return BadRequest("Person object is null.");

         var addedPerson = await _personService.AddPerson(person);
         return CreatedAtAction(nameof(GetById), new { id = addedPerson.Id }, addedPerson);
      }

      [Authorize(Roles = "HRAdmin,Manager")]
      [HttpPut]
      public async Task<IActionResult> Put([FromBody] Person person)
      {
         if (person == null || person.Id <= 0)
            return BadRequest("Invalid person data.");

         var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
         var userPersonId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

         if (userRole == "Manager")
         {
            var manager = await _personService.GetPersonById(userPersonId);
            if (manager?.Department != person.Department)
            {
               return Forbid("Managers can only update people in their department.");
            }
         }

         var updatedPerson = await _personService.UpdatePerson(person);
         if (updatedPerson == null)
         {
            return NotFound("Person not found for update.");
         }

         return Ok(updatedPerson);
      }

      [Authorize(Roles = "HRAdmin")]
      [HttpDelete("{id}")]
      public async Task<IActionResult> Delete(int id)
      {
         var deleted = await _personService.DeletePerson(id);
         return deleted ? NoContent() : NotFound("Person not found.");
      }
   }
}
