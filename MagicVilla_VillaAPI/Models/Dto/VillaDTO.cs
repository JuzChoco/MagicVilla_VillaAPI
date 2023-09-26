using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public String Name { get; set; }

        public int Sqft { get; set; }
        public int Occupancy { get; set; }
    }
}
