using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")] // Can also do this and it will use VillaAPI but not preferred as might have drawbacks in the future
    [Route("api/VillaAPI")]
    //[ApiController ]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogging _logger; //For custom logging
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db) //(ILogging logger)
        {
            _db = db;
            //_logger = logger; //For custom logging   

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            //_logger.Log("Getting all villas!", ""); //For custom logging
               return Ok(_db.Villas);

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponsesType(200, Type = typeof(VillaDTO))] //Can do this too if you just put public ActionResult GetVilla(int id); just more tedious
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("Error getting villa with Id: " + id, "error"); //For custom logging
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            
            if (villa == null)
            {
                //_logger.Log("Error getting villa with Id: " + id, "error"); //For custom logging
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villa)
        {

            //Can be used if you wish to create your own custom validation or if you did not include [ApiController] at the top
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (_db.Villas.FirstOrDefault(u=>u.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error:", "The villa name already exists!");
                return BadRequest(ModelState);
            }

            if (villa == null)
            {
                return BadRequest(villa);
            }

            if (villa.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa model = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };

            _db.Villas.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new {id = villa.Id}, villa);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id) //IActionResult or ActionResult is viable for this
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villa) //If you do not put [FromBody] all the fields will have to be inputted, instead of just the Id
        {
            if (villa == null || id != villa.Id)
            {
                return BadRequest();
            }

            //When VillaDataStore was used
            //var actualVilla = VillaStore.GetAllVillaDTOs.FirstOrDefault(u => u.Id == id);
            //actualVilla.Name = villa.Name;
            //actualVilla.Sqft = villa.Sqft;
            //actualVilla.Occupancy = villa.Occupancy;

            //*** When using DbContext ***//
            Villa model = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };
            
            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();
        }

        //Can exclude operationType and from in Swagger
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDTO> patch)
        {
            if (patch == null || id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };

            if (villa == null)
            {
                return BadRequest();
            }

            patch.ApplyTo(villaDTO, ModelState);

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft

            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid) {
                ModelState.AddModelError("Custom Error", "An error has occured.");
            }

            return NoContent();
        }
    }
}
