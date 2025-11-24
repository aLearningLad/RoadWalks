using Microsoft.AspNetCore.Mvc;
using RoadWorkClub.API.Models.Domain;
using RoadWorkClub.API.Models.DTOs;

namespace RoadWorkClub.API.Interfaces
{
    public interface IPathwayRepository
    {
        Task<List<Pathway>> GetAll();
    }
}
