using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")] // Can also do this and it will use VillaAPI but not preferred as might have drawbacks in the future
    [Route("api/VillaAPI")]
    //[ApiController ]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogging _logger; //For custom logging
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper) //(ILogging logger)
        {
            _db = db;
            _mapper = mapper;
            //_logger = logger; //For custom logging   

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            //_logger.Log("Getting all villas!", ""); //For custom logging
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
               return Ok(_mapper.Map<VillaDTO>(villaList));

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponsesType(200, Type = typeof(VillaDTO))] //Can do this too if you just put public ActionResult GetVilla(int id); just more tedious
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("Error getting villa with Id: " + id, "error"); //For custom logging
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            
            if (villa == null)
            {
                //_logger.Log("Error getting villa with Id: " + id, "error"); //For custom logging
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createVilla)
        {

            //Can be used if you wish to create your own custom validation or if you did not include [ApiController] at the top
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _db.Villas.FirstOrDefaultAsync(u=>u.Name.ToLower() == createVilla.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error:", "The villa name already exists!");
                return BadRequest(ModelState);
            }

            if (createVilla == null)
            {
                return BadRequest(createVilla);
            }

            //if (villa.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            Villa model = _mapper.Map<Villa>(createVilla);

            // _mapper.Map<Villa>(createVilla) basically does the job of mapping
            //Villa model = new()
            //{
            //    Amenity = createVilla.Amenity,
            //    Details = createVilla.Details,
            //    ImageUrl = createVilla.ImageUrl,
            //    Name = createVilla.Name,
            //    Occupancy = createVilla.Occupancy,
            //    Rate = createVilla.Rate,
            //    Sqft = createVilla.Sqft,
            //    CreatedDate = DateTime.Now

            //};

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new {id = model.Id}, model);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) //IActionResult or ActionResult is viable for this
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateVilla) //If you do not put [FromBody] all the fields will have to be inputted, instead of just the Id
        {
            if (updateVilla == null || id != updateVilla.Id)
            {
                return BadRequest();
            }

            //When VillaDataStore was used
            //var actualVilla = VillaStore.GetAllVillaDTOs.FirstOrDefault(u => u.Id == id);
            //actualVilla.Name = villa.Name;
            //actualVilla.Sqft = villa.Sqft;
            //actualVilla.Occupancy = villa.Occupancy;

            //*** When using DbContext ***//
            Villa model = _mapper.Map<Villa>(updateVilla);

            //Villa model = new()
            //{
            //    Amenity = updateVilla.Amenity,
            //    Details = updateVilla.Details,
            //    Id = updateVilla.Id,
            //    ImageUrl = updateVilla.ImageUrl,
            //    Name = updateVilla.Name,
            //    Occupancy = updateVilla.Occupancy,
            //    Rate = updateVilla.Rate,
            //    Sqft = updateVilla.Sqft,
            //    UpdatedDate = updateVilla.UpdatedDate
            //};
            
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        //Can exclude operationType and from in Swagger
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchVilla(int id, JsonPatchDocument<VillaUpdateDTO> patch)
        {
            if (patch == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            VillaUpdateDTO villaUpdateDTO= _mapper.Map<VillaUpdateDTO>(villa);

            //VillaUpdateDTO villaDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft

            //};

            if (villa == null)
            {
                return BadRequest();
            }

            patch.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            //Villa model = new()
            //{
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    Id = villaDTO.Id,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Name = villaDTO.Name,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft

            //};

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            if (!ModelState.IsValid) {
                ModelState.AddModelError("Custom Error", "An error has occured.");
            }

            return NoContent();
        }
    }
}
