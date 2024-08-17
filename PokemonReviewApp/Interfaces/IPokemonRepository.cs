namespace PokemonReviewApp.Interfaces;

public interface IPokemonRepository
{
	Task<IEnumerable<Pokemon>> GetPokemons();
	Task<Pokemon> GetPokemon(int id);
	Task<Pokemon> GetPokemonByName(string name);
	decimal GetPokemonRating(int pokeId);
	bool PokemonExists(int pokeId);
	Pokemon DeletePokemon(Pokemon pokemon);
	Task<bool> CreatePokemon(int ownerId,int categoryId,Pokemon pokemon);
	bool UpdatePokemon(int categoryId,int ownerId,Pokemon pokemon);	
	Task<bool> save();
	//Task<Pokemon> UpdatePokemon(Pokemon pokemon);

}
