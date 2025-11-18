namespace RoadWorkClub.API.Models.DTOs
{
    public class updatePathwayDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double DistanceInKm { get; set; }
        public TimeSpan AvgDuration { get; set; } // this is in minutes
        public DateTime CreatedAt { get; set; }

        public bool IsLoop { get; set; } // does route start & end at same point??
        public TimeSpan RecommendedStartTime { get; set; }
    }
}
