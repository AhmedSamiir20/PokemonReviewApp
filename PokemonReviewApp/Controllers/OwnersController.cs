namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OwnersController : ControllerBase
{
	private readonly IOwnerRepository _ownerRepository;
	private readonly ICountryRepository _countryRepository;
	private readonly IMapper _mapper;

	public OwnersController(IOwnerRepository ownerRepository, IMapper mapper, ICountryRepository countryRepository)
	{
		_ownerRepository = ownerRepository;
		_mapper = mapper;
		_countryRepository = countryRepository;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
	public async Task<IActionResult> GetOwners()
	{
		var owners = _mapper.Map<IEnumerable<OwnerDto>>(await _ownerRepository.GetOwners());
		if(!ModelState.IsValid)
			return NotFound();
		return Ok(owners);
	}

	[HttpGet("{ownerId}")]
	[ProducesResponseType(200, Type = typeof(Owner))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetOwner(int id)
	{
		if (!_ownerRepository.OwnerExists(id))
			return NotFound();

		var owner = _mapper.Map<OwnerDto>(await _ownerRepository.GetOwner(id));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(owner);
	}

	[HttpGet("{ownerId}/pokemon")]
	[ProducesResponseType(200, Type = typeof(List<Pokemon>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetPokemonByOwner(int ownerId)
	{
		if(!_ownerRepository.OwnerExists(ownerId))
			return NotFound();
		var pokemons = _mapper.Map<IEnumerable<PokemonDto>>(await _ownerRepository.GetPokemonByOwner(ownerId));
		if(!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(pokemons);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreateOwner([FromQuery]int countryId,[FromForm] OwnerDto ownerCreate)
	{
		if (ownerCreate is null)
			return BadRequest(ModelState);

		if(!_countryRepository.CountryExists(countryId))
			return BadRequest(ModelState);

		

		var owners = (await _ownerRepository.GetOwners())
			.Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (owners is not null)
		{
			ModelState.AddModelError("", "Owner of this Name is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var ownerMap = _mapper.Map<Owner>(ownerCreate);

		ownerMap.Country=await _countryRepository.GetCountry(countryId);

		if (!await _ownerRepository.CreateOwner(ownerMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Successfully Created");
	}

	[HttpPut("{ownerId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateOwner(int ownerId, [FromForm] OwnerDto updateOwner)
	{
		updateOwner.Id = ownerId;
		if (updateOwner == null)
			return BadRequest(ModelState);
		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound("There is no Owner for this id");
		if (!ModelState.IsValid)
			return BadRequest();

		var ownerMap = _mapper.Map<Owner>(updateOwner);

		if (!_ownerRepository.UpdateOwner(ownerMap))
		{
			ModelState.AddModelError("", "Something went wrong while updating");
			return StatusCode(500, ModelState);
		}

		return Ok(ownerMap);
	}

	[HttpDelete("{ownerId}")]
	public async Task<IActionResult> DeleteOwner(int ownerId)
	{
		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound("There is no Owner for this id");

		var owner = await _ownerRepository.GetOwner(ownerId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		_ownerRepository.DeleteOwner(owner);

		return Ok(owner);
	}
}
