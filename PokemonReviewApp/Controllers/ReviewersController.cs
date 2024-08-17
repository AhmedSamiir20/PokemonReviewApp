namespace PokemonReviewApp.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewersController : ControllerBase
{
	private readonly IReviewerRepository _reviewerRepository;
	private readonly IMapper _mapper;

	public ReviewersController(IReviewerRepository reviewerRepository, IMapper mapper)
	{
		_reviewerRepository = reviewerRepository;
		_mapper = mapper;
	}

	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
	public async Task<IActionResult> GetReviewers()
	{
		var reviewers = _mapper.Map<IEnumerable<ReviewerDto>>(await _reviewerRepository.GetReviewers());
		if (!ModelState.IsValid)
			return NotFound();
		return Ok(reviewers);
	}

	[HttpGet("{reviewerId}")]
	[ProducesResponseType(200, Type = typeof(Reviewer))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetReviewer(int id)
	{
		if (!_reviewerRepository.ReviewerExists(id))
			return NotFound();

		var reviewer = _mapper.Map<ReviewerDto>(await _reviewerRepository.GetReviewer(id));
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		return Ok(reviewer);
	}

	[HttpGet("{reviewerId}/reviews")]
	public async Task<IActionResult> GetReviewsByReviewer(int id)
	{
		if (!_reviewerRepository.ReviewerExists(id))
			return NotFound();

		var reviews = _mapper.Map<IEnumerable<ReviewDto>>
			(await _reviewerRepository.GetReviewsByReviewer(id));
		
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		
		return Ok(reviews);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> CreateReviewer([FromForm] ReviewerDto reviewerCreate)
	{
		if (reviewerCreate is null)
			return BadRequest(ModelState);


		var reviewers = (await _reviewerRepository.GetReviewers())
			.Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper())
			.FirstOrDefault();


		if (reviewers is not null)
		{
			ModelState.AddModelError("", "reviewer of this title is already Exists!");
			return StatusCode(422, ModelState);
		}

		if (!ModelState.IsValid) return BadRequest(ModelState);

		var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);


		if (!await _reviewerRepository.CreateReviewer(reviewerMap))
		{
			ModelState.AddModelError("", "Something went wrong while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Successfully Created");
	}

	[HttpPut("{reviewerId}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> UpdateReviewer(int reviewerId, [FromForm] ReviewerDto updateReviewer)
	{
		updateReviewer.Id = reviewerId;
		if (updateReviewer == null)
			return BadRequest(ModelState);
		if (!_reviewerRepository.ReviewerExists(reviewerId))
			return NotFound("There is no Reviewer for this id");
		if (!ModelState.IsValid)
			return BadRequest();

		var reviewerMap = _mapper.Map<Reviewer>(updateReviewer);

		if (!_reviewerRepository.UpdateReviewer(reviewerMap))
		{
			ModelState.AddModelError("", "Something went wrong while updating");
			return StatusCode(500, ModelState);
		}

		return Ok(reviewerMap);
	}

	[HttpDelete("{reviewerId}")]
	public async Task<IActionResult> DeleteReviewer(int reviewerId)
	{
		if (!_reviewerRepository.ReviewerExists(reviewerId))
			return NotFound("There is no Reviewer for this id");

		var reviewer = await _reviewerRepository.GetReviewer(reviewerId);

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		_reviewerRepository.DeleteReviewer(reviewer);

		return Ok(reviewer);
	}
}


