
namespace PokemonReviewApp.Repository;

public class PokemonRepository : IPokemonRepository
{

	private readonly ApplicationDbContext _context;

	public PokemonRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<bool> CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
	{
		var pokemonOwnerEntity = await _context.Owners.Where(o => o.Id == ownerId).FirstOrDefaultAsync();
		var pokemonCategoryEntity = await _context.Categories.Where(o => o.Id == categoryId).FirstOrDefaultAsync();

		var pokemonOwner = new PokemonOwner//cuz the many to many relationship
		{
			Owner = pokemonOwnerEntity,//we can use automapper too here but i think it is clear as exist
			Pokemon = pokemon,
		};
		await _context.AddAsync(pokemonOwner);

		var pokemonCategory = new PokemonCategory
		{
			Category = pokemonCategoryEntity,//this category is in pokemon we will put in it the category of parameter
			Pokemon = pokemon
		};
		await _context.AddAsync(pokemonOwner);
		await _context.AddAsync(pokemon);
		return await save();
		
	}

	public Pokemon DeletePokemon(Pokemon pokemon)
	{
		_context.Pokemon.Remove(pokemon);
		_context.SaveChanges();
		return pokemon;
	}

	public async Task<Pokemon> GetPokemon(int id)
	{
		return await _context.Pokemon.Where(p => p.Id == id).FirstOrDefaultAsync();
		//return await _context.Pokemon.FindAsync(id);
	}

	public async Task<Pokemon> GetPokemonByName(string name)
	{
		return await _context.Pokemon.FirstAsync(p => p.Name == name);
	}

	public  decimal GetPokemonRating(int pokeId)
	{
		var review= _context.Reviews.Where(p=>p.Pokemon.Id ==pokeId);
		if (review.Count() <= 0)
			return 0;

		return ((decimal)review.Sum(r => r.Rating) / review.Count());
	}

	public async Task<IEnumerable<Pokemon>> GetPokemons()
	{
		return await _context.Pokemon.OrderBy(p => p.Id).ToListAsync();  //manipulate the data
	}

	public bool PokemonExists(int pokeId)
	{
		return _context.Pokemon.Any(p => p.Id == pokeId); 
	}

	public async Task<bool> save()
	{
		var saved = await _context.SaveChangesAsync();
		return saved > 0 ? true : false;
	}

	public bool UpdatePokemon(int categoryId, int ownerId,Pokemon pokemon)
	{
		_context.Pokemon.Update(pokemon);
		_context.SaveChanges();
		return true;
		
	}
}
