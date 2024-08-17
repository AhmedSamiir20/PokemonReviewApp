namespace PokemonReviewApp.Interfaces;

public interface IOwnerRepository
{
	Task<IEnumerable<Owner>> GetOwners();
	Task<Owner> GetOwner(int  ownerId);
	Task<IEnumerable<Owner>> GetOwnerOfPokemon(int pokeId);
	Task<IEnumerable<Pokemon>> GetPokemonByOwner(int ownerId);
	bool OwnerExists(int ownerId);
	Owner DeleteOwner(Owner owner);
	bool UpdateOwner(Owner owner);
	Task<bool> CreateOwner(Owner owner);
	Task<bool> Save();
}
