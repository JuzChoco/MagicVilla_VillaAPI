using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        //Temporary Data Storage so that the Controller class can call from this file instead of hard coding all the villas in that file
        public static List<VillaDTO> GetAllVillaDTOs = new List<VillaDTO>
        {
                new VillaDTO{Id = 1, Name = "Pool View", Sqft = 100, Occupancy = 4},
                new VillaDTO{Id = 2, Name = "Beach View", Sqft = 100, Occupancy = 4}
        };
    }
}
