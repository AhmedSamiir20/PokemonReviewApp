namespace PokemonReviewApp.Repository;

public class ReviewRepository : IReviewRepository
{
	private readonly ApplicationDbContext _context;

	public ReviewRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<bool> CreateReview(Review review)
	{
		await _context.AddAsync(review);
		return await Save();
	}

	public Review DeleteReview(Review review)
	{
		_context.Reviews.Remove(review);
		_context.SaveChanges();
		return review;
	}

	public async Task<Review> GetReview(int id)
	{
		return await _context.Reviews.FindAsync(id);
	}

	public async Task<IEnumerable<Review>> GetReviews()
	{
		return await _context.Reviews.Include(r => r.Reviewer).ToListAsync();
	}

	public async Task<IEnumerable<Review>> GetReviewsOfPokemon(int pokeId)
	{
		return await _context.Reviews.Where(p => p.Pokemon.Id == pokeId).ToListAsync();
	}

	public bool ReviewExists(int id)
	{
		return _context.Reviews.Any(r => r.Id == id);
	}

	public async Task<bool> Save()
	{
		var saved = await _context.SaveChangesAsync();
		return saved > 0 ? true : false;
	}

	public bool UpdateReview(Review review)
	{
		_context.Reviews.Update(review);
		_context.SaveChanges();
		return true;
	}
}
