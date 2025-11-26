using Microsoft.AspNetCore.Mvc;
using RoadWorkClub.API.Models.Domain;
using RoadWorkClub.API.Models.DTOs;

namespace RoadWorkClub.API.Interfaces
{
    public interface IPathwayRepository
    {
        Task<List<Pathway>> GetAll();

        // to fetch stopovers via their IDs
        Task<Stopover?> GetStopover(Guid stopoverId);

        Task AddPathToDb(Pathway newpath);

        Task SavePath();

        Task<Pathway?> FindPathwayDomain(Guid id);

        Task DeletePath(Pathway);
    }
}
