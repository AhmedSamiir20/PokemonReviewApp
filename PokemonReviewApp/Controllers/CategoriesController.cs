namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IMapper _mapper;

	public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
	{
		_categoryRepository = categoryRepository;
		_mapper = mapper;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
	public async Task<IActionResult> GetCategories()
	{
		var categories = _mapper.Map<IEnumerable<CategoryDto>>(await _categoryRepository.GetCategories());

		if (!ModelState.IsValid)
			return NotFound();
		return Ok(categories);
	}

	[HttpGet("{catgoryId}")]
	[ProducesResponseType(200, Type = typeof(Category))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetCategory(int id)
	{
		if (!_categoryRepository.CategoryExists(id))
			return NotFound();

		var category = _mapper.Map<CategoryDto>(await _categoryRepository.GetCategory(id));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(category);
	}

	[HttpGet("pokemon/{categoryId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetPokemonByCategoryid(int id)
	{
		if (!_categoryRepository.CategoryExists(id))
			return NotFound();

		var pokemons = _mapper.Map<IEnumerable<PokemonDto>>
			(await _categoryRepository.GetPokemonsByCategory(id));

		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(pokemons);

	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreateCategory([FromForm] CategoryDto categoryCreate)
	{
		if (categoryCreate is null)
			return BadRequest(ModelState);

		var categories = (await _categoryRepository.GetCategories())
			.Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (categories is not null)
		{
			ModelState.AddModelError("", "category of this Name is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var categoryMap = _mapper.Map<Category>(categoryCreate);

		if (!await _categoryRepository.CreateCategory(categoryMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Successfully Created");
	}

	[HttpPut("{categoryId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateCategory(int categoryId, [FromForm]CategoryDto updateCategory)
	{
		updateCategory.Id = categoryId;	
		if(updateCategory == null)
			return BadRequest(ModelState);
		if(!_categoryRepository.CategoryExists(categoryId))
			return NotFound("There is no Category for this id");
		if (!ModelState.IsValid) 
			return BadRequest();

		var categoryMap = _mapper.Map<Category>(updateCategory);

		if(!_categoryRepository.UpdateCategory(categoryMap))
		{
			ModelState.AddModelError("","Something went wrong while updating");
			return StatusCode(500, ModelState); 
		}
			
		return Ok(categoryMap); 
	}

	[HttpDelete("{catId}")]
	public async Task<IActionResult> DeleteCategory(int categoryId)
	{
		if(! _categoryRepository.CategoryExists(categoryId))
			return NotFound("There is no Category for this id");
		
		var category = await _categoryRepository.GetCategory(categoryId);
		
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		
		_categoryRepository.DeleteCategory(category);
		
		return Ok(category);
	}
}
