using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorkClub.API.Data;
using RoadWorkClub.API.Interfaces;
using RoadWorkClub.API.Models.Domain;
using RoadWorkClub.API.Models.DTOs;

namespace RoadWorkClub.API.Repositories
{
    public class PathwayRepository : IPathwayRepository
    {
        private readonly RoadWorkClubDbContext dbContext;

        public PathwayRepository(RoadWorkClubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Pathway>> GetAll()
        {

            var res = await dbContext.Path.ToListAsync(); // return list always. If no values found, returns empty list

            return res;

           
            

            
        }
};
