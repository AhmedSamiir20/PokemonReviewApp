

namespace PokemonReviewApp.Repository;

public class CountryRepository : ICountryRepository
{
	private readonly ApplicationDbContext _context;

	public CountryRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public bool CountryExists(int id)
	{
		return _context.Countries.Any(c => c.Id == id);
	}

	public async Task<bool> CreateCountry(Country country)
	{
		await _context.Countries.AddAsync(country);
		return await Save();
	}

	public Country DeleteCountry(Country country)
	{
		_context.Countries.Remove(country);
		_context.SaveChanges();
		return country;
	}

	public async Task<IEnumerable<Country>> GetCountries()
	{
		return await _context.Countries.ToListAsync();
	}

	public async Task<Country> GetCountry(int id)
	{
		return await _context.Countries.Where(c => c.Id == id).FirstOrDefaultAsync();
	}

	public async Task<Country> GetCountryByOwner(int ownerId)
	{
		return await _context.Owners.Where(o=>o.Id==ownerId).Select(c=>c.Country).FirstOrDefaultAsync();//i have owner id i will take it and go to this owner and select the country of this owner.
	}

	public async Task<IEnumerable<Owner>> GetOwnersFromCountry(int countryId)
	{
		return await _context.Owners.Where(c => c.Country.Id == countryId).ToListAsync(); //i have countryid i will go to the owner table and compare countryid row row and find the owner that have countryid same.

	}

	public async Task<bool> Save()
	{
		var saved= await _context.SaveChangesAsync();
		return saved > 0 ? true : false;
	}

	public bool UpdateCountry(Country country)
	{
		_context.Update(country);
		_context.SaveChanges();
		return true;
	}
}
