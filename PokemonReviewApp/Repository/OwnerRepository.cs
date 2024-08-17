namespace PokemonReviewApp.Repository;

public class OwnerRepository : IOwnerRepository
{
	private readonly ApplicationDbContext _context;

	public OwnerRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<bool> CreateOwner(Owner owner)
	{
		await _context.Owners.AddAsync(owner);
		return await Save();
	}

	public Owner DeleteOwner(Owner owner)
	{
		_context.Owners.Remove(owner);
		_context.SaveChanges();
		return owner;
	}

	public async Task<Owner> GetOwner(int ownerId)
	{
		return await _context.Owners.SingleOrDefaultAsync(o=>o.Id==ownerId);
	}

	public async Task<IEnumerable<Owner>> GetOwnerOfPokemon(int pokeId)
	{
		return await _context.PokemonOwners.Where(o=>o.PokemonId==pokeId).Select(o=>o.Owner).ToListAsync();
	}

	public async Task<IEnumerable<Owner>> GetOwners()
	{
		return await _context.Owners.ToListAsync();
	}

	public async Task<IEnumerable<Pokemon>> GetPokemonByOwner(int ownerId)
	{
		return await _context.PokemonOwners.Where(p=>p.Owner.Id==ownerId).Select(p=>p.Pokemon).ToListAsync();
	}

	public bool OwnerExists(int ownerId)
	{
		return _context.Owners.Any(o => o.Id==ownerId);
	}

	public async Task<bool> Save()
	{
		var saved= await _context.SaveChangesAsync();
		return saved>0?true: false;
	}

	public bool UpdateOwner(Owner owner)
	{
		_context.Owners.Update(owner);
		_context.SaveChanges();
		return true;
	}
}
