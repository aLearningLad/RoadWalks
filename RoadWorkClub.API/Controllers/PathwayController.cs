using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.IdentityModel.Tokens;
using RoadWorkClub.API.Data;
using RoadWorkClub.API.Interfaces;
using RoadWorkClub.API.Models.Domain;
using RoadWorkClub.API.Models.DTOs;
using RoadWorkClub.API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RoadWorkClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathwayController : ControllerBase
    {
        private readonly RoadWorkClubDbContext rwcdbContext;
        private readonly IPathwayRepository pathwayRepository;

        public PathwayController(RoadWorkClubDbContext rwcdbContext, IPathwayRepository pathwayRepository)
        {
            this.rwcdbContext = rwcdbContext;
            this.pathwayRepository = pathwayRepository;
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
                        var stopoverEntry = await pathwayRepository.GetStopover(eachId);
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

                await pathwayRepository.AddPathToDb(dataToDb);

                try
                {

                    await pathwayRepository.SavePath();
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
        public async Task<IActionResult> GetAll()
        {

            var allpathways = await pathwayRepository.GetAll();
            var listOfPaths = new List<PathwayDto>();

            foreach (var pathway in allpathways)
            {
                var thisPath = new PathwayDto
                {
                    AvgDuration = pathway.AvgDuration,
                    Description = pathway.Description,
                };
                listOfPaths.Add(thisPath);
            }
            

            return Ok(listOfPaths);
        }

        // get a single pathway by it's ID
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id) {


            // get Id from params
            Guid pathwayId = id;

            // try getting it from database
            var res = await rwcdbContext.Path.FindAsync(pathwayId);
            


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
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateById([FromRoute]Guid id, updatePathwayDto updatePathwayDto)
        {

            // get the pathway from database if it exists
            var pathwayDomain = await pathwayRepository.FindPathwayDomain(id);

            if (pathwayDomain == null)
            {
                return NotFound();
            }

            // update values 
            pathwayDomain.AvgDuration = updatePathwayDto.AvgDuration;
            pathwayDomain.Name = updatePathwayDto.Name;
            pathwayDomain.IsLoop = updatePathwayDto.IsLoop;
            pathwayDomain.RecommendedStartTime = updatePathwayDto.RecommendedStartTime;

           await rwcdbContext.SaveChangesAsync();

            // convert to a dto to return to client with success code
            var responseDto = new PathwayDto
            {
               Name = pathwayDomain.Name,
               Description = pathwayDomain.Description,
               AvgDuration = pathwayDomain.AvgDuration,
               Id = pathwayDomain.Id,
               RecommendedStartTime = pathwayDomain.RecommendedStartTime,
               DistanceInKm = pathwayDomain.DistanceInKm,
               IsLoop = pathwayDomain.IsLoop
            };

            return Ok(responseDto);
        }



        // delete a method via it's ID
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id) {

            var toDelete = await rwcdbContext.Path.FirstOrDefaultAsync(x => x.Id == id);

            if (toDelete == null)
            {
                return NotFound();
            }
            try
            {
                var deletedDto = new PathwayDto()
                {
                    Name = toDelete.Name,
                    Description = toDelete.Description,
                    AvgDuration = toDelete.AvgDuration,
                    DifficultyRatingValue = Convert.ToString(toDelete.DifficultyRating),
                    Id = toDelete.Id,
                };


             rwcdbContext.Path.Remove(toDelete);
            rwcdbContext.SaveChanges(); // can't make this async bc .Remove is synchronous
            return Ok(deletedDto); 
            }      
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    
    }
}
