using RoadWorkClub.API.Models.DTOs;

namespace RoadWorkClub.API.Interfaces
{
    public interface IPathwayRepository
    {
        Task<PathwayDto> GetAll();
    }
}
