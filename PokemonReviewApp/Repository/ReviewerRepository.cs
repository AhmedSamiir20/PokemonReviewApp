namespace PokemonReviewApp.Repository;

public class ReviewerRepository : IReviewerRepository
{
	private readonly ApplicationDbContext _context;

	public ReviewerRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<bool> CreateReviewer(Reviewer reviewer)
	{
		await _context.AddAsync(reviewer);
		return await Save();
	}

	public Reviewer DeleteReviewer(Reviewer reviewer)
	{
		_context.Reviewers.Remove(reviewer);
		_context.SaveChanges();
		return reviewer;
	}

	public async Task<Reviewer> GetReviewer(int id)
	{
		return await _context.Reviewers.Where(r=>r.Id==id).Include(e=>e.Reviews).ThenInclude(p=>p.Pokemon).FirstOrDefaultAsync();
	}

	public async Task<IEnumerable<Reviewer>> GetReviewers()
	{
		return await _context.Reviewers.ToListAsync();
	}

	public async Task<IEnumerable<Review>> GetReviewsByReviewer(int id)
	{
		return await _context.Reviews.Where(r=>r.Reviewer.Id==id).ToListAsync();
	}

	public bool ReviewerExists(int id)
	{
		return _context.Reviewers.Any(r => r.Id==id);
	}

	public async Task<bool> Save()
	{
		var saved=await _context.SaveChangesAsync();
		return saved > 0 ? true : false;
	}

	public bool UpdateReviewer(Reviewer reviewer)
	{
		_context.Reviewers.Update(reviewer);
		_context.SaveChanges();
		return true;
	}
}
