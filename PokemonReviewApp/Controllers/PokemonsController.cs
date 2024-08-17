namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PokemonsController : ControllerBase
{
	private readonly IPokemonRepository _pokemonRepository;
	private readonly IOwnerRepository _ownerRepository;
	private readonly ICategoryRepository _categoryRepository;
	private readonly IReviewerRepository _reviewerRepository;
	private readonly IMapper _mapper;

	public PokemonsController(IPokemonRepository pokemonRepository, IMapper mapper, IOwnerRepository ownerRepository, ICategoryRepository categoryRepository, IReviewerRepository reviewerRepository)
	{
		_pokemonRepository = pokemonRepository;
		_mapper = mapper;
		_ownerRepository = ownerRepository;
		_categoryRepository = categoryRepository;
		_reviewerRepository = reviewerRepository;
	}

	[HttpGet]
	[ProducesResponseType(200,Type=typeof(IEnumerable<Pokemon>))]
	public async Task<IActionResult> GetPokemons()
	{
		var pokemon= _mapper.Map<List<PokemonDto>>(await _pokemonRepository.GetPokemons());

		if(!ModelState.IsValid)
			BadRequest(ModelState);
		return Ok(pokemon);
	}

	[HttpGet("{pokeId}")]
	[ProducesResponseType(200, Type = typeof(Pokemon))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetPokemon(int id)
	{
		if (! _pokemonRepository.PokemonExists(id))
			return NotFound();

		var pokemon= _mapper.Map<PokemonDto>( await _pokemonRepository.GetPokemon(id));
		if(!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(pokemon);
	}

	[HttpGet("{pokeId}/rating")]
	[ProducesResponseType(200, Type = typeof(decimal))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetPokemonRating(int pokeId)
	{
		if (!_pokemonRepository.PokemonExists(pokeId))
			return NotFound();

		var rating=  _pokemonRepository.GetPokemonRating(pokeId);
		if (!ModelState.IsValid)  //Model State Check about the validation of models
			return BadRequest(ModelState); 
		
		return Ok(rating);
	}

	[HttpGet("{pokeName}")]
	public async Task<IActionResult> GetPokemonByName(string name)
	{
		var pokemon=await _pokemonRepository.GetPokemonByName(name);
		if(pokemon is null)
			return NotFound();
		return Ok(pokemon);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreatePokemon([FromQuery]int ownerId,[FromQuery]int catId,[FromForm]PokemonDto pokemonCreate)
	{
		if (pokemonCreate is null)
			return BadRequest(ModelState);

		if (!_ownerRepository.OwnerExists(ownerId))
			return NotFound("This Owner is invalid");
		
		if (!_categoryRepository.CategoryExists(catId))
			return NotFound("This Category is invalid");

		var pokemons = (await _pokemonRepository.GetPokemons())
			.Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (pokemons is not null)
		{
			ModelState.AddModelError("", "Pokemon is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

		if(!await _pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500,ModelState);
		}
		return Ok("Successfully Created");
	}

	[HttpPut("{pokemonId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdatePokemon([FromQuery]int pokemonId, [FromQuery] int categoryId, [FromQuery] int ownerId, [FromForm] PokemonDto updatePokemon)
	{
		updatePokemon.Id = pokemonId;
		if (updatePokemon == null)
			return BadRequest(ModelState);
		if (!_pokemonRepository.PokemonExists(pokemonId))
			return NotFound("There is no Pokemon for this id");
		if (!ModelState.IsValid)
			return BadRequest();

		var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);
		if (!_pokemonRepository.UpdatePokemon(categoryId,ownerId,pokemonMap))
		{
			ModelState.AddModelError("", "Something went wrong while updating");
			return StatusCode(500, ModelState);
		}

		return Ok(pokemonMap);
	}

	[HttpDelete("{pokeId}")]
	public async Task<IActionResult> DeletePokemon(int pokemonId)
	{
		if (!_pokemonRepository.PokemonExists(pokemonId))
			return NotFound("There is no Pokemon for this id");

		var pokemon = await _pokemonRepository.GetPokemon(pokemonId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		_pokemonRepository.DeletePokemon(pokemon);

		return Ok(pokemon);
		
	}
}
