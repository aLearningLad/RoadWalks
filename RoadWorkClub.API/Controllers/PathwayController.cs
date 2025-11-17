using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.IdentityModel.Tokens;
using RoadWorkClub.API.Data;
using RoadWorkClub.API.Models.Domain;
using RoadWorkClub.API.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace RoadWorkClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathwayController : ControllerBase
    {
        private readonly RoadWorkClubDbContext rwcdbContext;

        public PathwayController(RoadWorkClubDbContext rwcdbContext)
        {
            this.rwcdbContext = rwcdbContext;
        }


        // create a pathway
        [HttpPost]
        public async Task<IActionResult> Newpathway([FromBody]PathwayDto newPath)
        {
            if (newPath != null)

            {
                // check for stopover IDs --> add them to stopovers list in domain model later
                 var stopoverIds = new List<Guid>(newPath.StopoverIds);

                    var stopoverNames = new List<Stopover>() { };
                if (stopoverIds.Any())
                {
                    foreach (var eachId in stopoverIds)
                    {
                        var stopoverEntry = rwcdbContext.Stopovers.Find(eachId);
                        if (stopoverEntry != null) {
                            stopoverNames.Add(stopoverEntry);
                        }
                    }
                }



                // compare this to values inside enum --> return error if not correct
                 var difficulty = newPath.DifficultyRatingValue;
                if (!Enum.IsDefined(typeof(Enums.Difficulty), difficulty)) { // weird syntax --> I'm checking if my enums file DOES NOT contain this value
                    return BadRequest("Difficulty rating is invalid. Please consult the Github ReadME");
                }
                 


                // map all to domain model
                var dataToDb = new Pathway() {
                    Name = newPath.Name,
                    AvgDuration = newPath.AvgDuration,
                    Description = newPath.Description,
                    DifficultyRating = Enum.Parse<Enums.Difficulty>(difficulty), // look inside my enums, fetch the value I want
                    CreatedAt = DateTime.UtcNow, // standard way to do it
                    DistanceInKm = newPath.DistanceInKm,
                    Id = new Guid(Guid.NewGuid().ToString()),
                    Stopovers = stopoverNames,
                };

                var res = rwcdbContext.Add(dataToDb);

                try
                {

               await rwcdbContext.SaveChangesAsync();
                } catch (DbUpdateException ex)
                {
                    return BadRequest(ex.Message);
                } catch (ValidationException ex)
                {
                    return Unauthorized(ex.Message);
                } catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            };
            return Ok(new { data=newPath, message= "New path saved successfully" });
        }

        // get all pathways
        [HttpGet]
        public IActionResult GetAll()
        {

            var allpathways = rwcdbContext.Path.ToList();

            // if I have paths, map to dto & return it
            if(allpathways.Any())
            {
                return Ok(allpathways);
            }

            return Ok("The database is empty, bruv");

            
       
        
        
        }

        // get a single pathway by it's ID
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute]Guid id) {


            // get Id from params
            Guid pathwayId = id;

            // try getting it from database
            var res = rwcdbContext.Path.Find(pathwayId);
            


            // if it exists, cool. Return it
            if (res != null)
            {
                return Ok($"The path you requested: {res}");
            }

            // if it doesn't exist, return error code and a short descriptive message
            return NotFound("No pathway with that ID currently exists");
        
        }

        // update a route via it's ID
        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult UpdateById([FromRoute]Guid id)
        {

            //var res = rwcdbContext
            return StatusCode(200); // test this, check it out
        }
    
    
    }
}
