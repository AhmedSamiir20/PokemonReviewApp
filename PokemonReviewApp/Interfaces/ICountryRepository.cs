namespace PokemonReviewApp.Interfaces;

public interface ICountryRepository
{
	Task<IEnumerable<Country>> GetCountries();
	Task<Country> GetCountry(int id);
	Task<Country> GetCountryByOwner(int ownerId);
	Task<IEnumerable<Owner>> GetOwnersFromCountry(int countryId);
	bool CountryExists(int id);
	Country DeleteCountry(Country country);
	bool UpdateCountry(Country country);
	Task<bool> Save();
	Task<bool> CreateCountry(Country country);
}
