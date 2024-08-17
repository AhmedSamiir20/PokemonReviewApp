namespace PokemonReviewApp.Repository;

public class CategoryRepository : ICategoryRepository
{
	private readonly ApplicationDbContext _context;

	public CategoryRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public bool CategoryExists(int id)
	{
		return _context.Categories.Any(c => c.Id == id);
	}

	public async Task<bool> CreateCategory(Category category)
	{
		await _context.Categories.AddAsync(category);
		return await Save();
	}

	public Category DeleteCategory(Category category)
	{
		_context.Categories.Remove(category);
		_context.SaveChanges();
		return category;
	}

	public async Task<IEnumerable<Category>> GetCategories()
	{
		return await _context.Categories.ToListAsync();
	}

	public async Task<Category> GetCategory(int id)
	{
		return await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
	}

	public async Task<IEnumerable<Pokemon>> GetPokemonsByCategory(int id)
	{
		return await _context.PokemonCategories.Where(c=>c.CategoryId == id).Select(p=>p.Pokemon).ToListAsync();//first i bring the category and select all the pokemon in this category
	}

	public async Task<bool> Save()
	{
		var saved =  await _context.SaveChangesAsync();
		return saved>0? true : false;
	}

	public  bool UpdateCategory(Category category)
	{
		_context.Categories.Update(category);
		_context.SaveChanges();
		return true;
	}
}
