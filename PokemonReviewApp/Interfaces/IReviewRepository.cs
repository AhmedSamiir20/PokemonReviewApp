namespace PokemonReviewApp.Interfaces;

public interface IReviewRepository
{
	Task<IEnumerable<Review>> GetReviews();
	Task<Review> GetReview(int id);
	Task<IEnumerable<Review>> GetReviewsOfPokemon(int pokeId);
	bool ReviewExists(int id);
	Task<bool> CreateReview(Review review);
	Task<bool> Save();
	Review DeleteReview(Review review);
	bool UpdateReview(Review review);
}
