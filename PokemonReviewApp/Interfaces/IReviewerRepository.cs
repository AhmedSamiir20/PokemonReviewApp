namespace PokemonReviewApp.Interfaces;

public interface IReviewerRepository
{
	Task<IEnumerable<Reviewer>> GetReviewers();
	Task<Reviewer> GetReviewer(int id);
	Task<IEnumerable<Review>> GetReviewsByReviewer(int id);
	bool ReviewerExists(int id);
	Task<bool> CreateReviewer(Reviewer reviewer);
	Task<bool> Save();
	Reviewer DeleteReviewer(Reviewer reviewer);
	bool UpdateReviewer(Reviewer reviewer);
}
