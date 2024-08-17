namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
	private readonly IReviewRepository _reviewRepository;
	private readonly IPokemonRepository _pokeRepository;
	private readonly IReviewerRepository _reviewerRepository;
	private readonly IMapper _mapper;

	public ReviewsController(IReviewRepository reviewRepository, IMapper mapper, IPokemonRepository pokeRepository, IReviewerRepository reviewerRepository)
	{
		_reviewRepository = reviewRepository;
		_mapper = mapper;
		_pokeRepository = pokeRepository;
		_reviewerRepository = reviewerRepository;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
	public async Task<IActionResult> GetReviews()
	{
		var reviews = _mapper.Map<IEnumerable<ReviewDto>>(await _reviewRepository.GetReviews());
		if (!ModelState.IsValid)
			return NotFound();
		return Ok(reviews);
	}

	[HttpGet("{reviewId}")]
	[ProducesResponseType(200, Type = typeof(Review))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetReview(int id)
	{
		if (!_reviewRepository.ReviewExists(id))
			return NotFound();

		var review = _mapper.Map<ReviewDto>(await _reviewRepository.GetReview(id));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(review);
	}

	[HttpGet("pokemon/{pokeId}")]
	[ProducesResponseType(200, Type = typeof(List<Review>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetReviewsByPokemon(int pokeId)
	{
		if (!_pokeRepository.PokemonExists(pokeId))
			return NotFound();
		var reviews = _mapper.Map<IEnumerable<ReviewDto>>(await _reviewRepository.GetReviewsOfPokemon(pokeId));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(reviews);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreateReview([FromQuery] int pokeId, [FromQuery] int reviewerId, [FromForm] ReviewDto reviewCreate)
	{
		if (reviewCreate is null)
			return BadRequest(ModelState);

		if (!_pokeRepository.PokemonExists(pokeId))
			return NotFound("You Entered wrong Pokemon Id");

		if (!_reviewerRepository.ReviewerExists(reviewerId))
			return NotFound("You Entered wrong Reviewer Id");


		var reviews = (await _reviewRepository.GetReviews())
			.Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (reviews is not null)
		{
			ModelState.AddModelError("", "review of this title is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var reviewMap = _mapper.Map<Review>(reviewCreate);


		reviewMap.Pokemon = await _pokeRepository.GetPokemon(pokeId);
		reviewMap.Reviewer = await _reviewerRepository.GetReviewer(reviewerId);


		if (!await _reviewRepository.CreateReview(reviewMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Successfully Created");
	}

	[HttpPut("{reviewId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateReview(int reviewId, [FromForm] ReviewDto updateReview)
	{
		updateReview.Id = reviewId;
		if (updateReview == null)
			return BadRequest(ModelState);
		if (!_reviewRepository.ReviewExists(reviewId))
			return NotFound("There is no Review for this id");
		if (!ModelState.IsValid)
			return BadRequest();

		var reviewMap = _mapper.Map<Review>(updateReview);

		if (!_reviewRepository.UpdateReview(reviewMap))
		{
			ModelState.AddModelError("", "Something went wrong while updating");
			return StatusCode(500, ModelState);
		}

		return Ok(reviewMap);
	}


	[HttpDelete("{reviewId}")]
	public async Task<IActionResult> DeleteReview(int reviewId)
	{
		if (!_reviewRepository.ReviewExists(reviewId))
			return NotFound("There is no Review for this id");

		var review = await _reviewRepository.GetReview(reviewId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		_reviewRepository.DeleteReview(review);

		return Ok(review);
	}
}
