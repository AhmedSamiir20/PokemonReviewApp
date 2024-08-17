namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
	private readonly ICountryRepository _countryRepository;
	private readonly IOwnerRepository _ownerRepository;
	private readonly IMapper _mapper;

	public CountriesController(ICountryRepository countryRepository, IMapper mapper, IOwnerRepository ownerRepository)
	{
		_countryRepository = countryRepository;
		_mapper = mapper;
		_ownerRepository = ownerRepository;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
	public async Task<IActionResult> GetCountries()
	{
		var countries = _mapper.Map<IEnumerable<CountryDto>>(await _countryRepository.GetCountries());

		if (!ModelState.IsValid)
			return NotFound();

		return Ok(countries);
	}

	[HttpGet("{countryId}")]
	[ProducesResponseType(200, Type = typeof(Country))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetCountry(int id)
	{
		if (!_countryRepository.CountryExists(id))
			return NotFound();

		var country = _mapper.Map<CountryDto>(await _countryRepository.GetCountry(id));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(country);
	}

	[HttpGet("/owner/{ownerId}")]
	[ProducesResponseType(200, Type = typeof(Country))]

	public async Task<IActionResult> GetCountryByOwnerid(int ownerId)
	{
		if(! _ownerRepository.OwnerExists(ownerId))
			return NotFound();

		var countries = _mapper.Map<CountryDto>
			(await _countryRepository.GetCountryByOwner(ownerId));

		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(countries);

	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreateCountry([FromForm] CountryDto countryCreate)
	{
		if (countryCreate is null)
			return BadRequest(ModelState);

		var countries = (await _countryRepository.GetCountries())
			.Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (countries is not null)
		{
			ModelState.AddModelError("", "country of this Name is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var countryMap = _mapper.Map<Country>(countryCreate);

		if (!await _countryRepository.CreateCountry(countryMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Successfully Created");
	}


	[HttpPut("{countryId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateCountry(int countryId, [FromForm] CountryDto updateCountry)
	{
		updateCountry.Id = countryId;
		if (updateCountry == null)
			return BadRequest(ModelState);
		if (!_countryRepository.CountryExists(countryId))
			return NotFound("There is no Country for this id");
		if (!ModelState.IsValid)
			return BadRequest();

		var countryMap = _mapper.Map<Country>(updateCountry);

		if (!_countryRepository.UpdateCountry(countryMap))
		{
			ModelState.AddModelError("", "Something went wrong while updating");
			return StatusCode(500, ModelState);
		}

		return Ok(countryMap);
	}

	[HttpDelete("{countryId}")]
	public async Task<IActionResult> DeleteCountry(int countryId)
	{
		if (!_countryRepository.CountryExists(countryId))
			return NotFound("There is no Country for this id");

		var country = await _countryRepository.GetCountry(countryId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		_countryRepository.DeleteCountry(country);

		return Ok(country);
	}


}
