using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using RoadWorkClub.API.Data;
using RoadWorkClub.API.Models.Domain;

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

        // get all pathways
        [HttpGet]
        public IActionResult GetAll()
        {

            var allpathways = rwcdbContext.Path.ToList();

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

            var res = rwcdbContext
            return StatusCode(200); // test this, check it out
        }
    
    
    }
}
