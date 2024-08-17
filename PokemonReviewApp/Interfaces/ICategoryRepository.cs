namespace PokemonReviewApp.Interfaces;

public interface ICategoryRepository
{
	Task<IEnumerable<Category>> GetCategories();
	Task<Category> GetCategory(int id);
	Task<IEnumerable<Pokemon>> GetPokemonsByCategory(int id);
	bool CategoryExists(int id);
	Category DeleteCategory(Category category);
	bool UpdateCategory(Category category);
	Task<bool> Save();
	Task<bool> CreateCategory(Category category);
}
