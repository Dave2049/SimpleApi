using SimpleApi.Models;

namespace SimpleApi.Persistence.Entities
{
    public record SmallGroupDTO
    {
        public int Id { get; init; }
        public string GeoCode { get; init; }
        public Adress Adress { get; init; }
        public int MaxCapacity { get; init; }
    }
}