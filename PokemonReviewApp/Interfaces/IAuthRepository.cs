namespace PokemonReviewApp.Interfaces;

public interface IAuthRepository
{
	Task<AuthModel> Register(RegisterModel registerModel);
	Task<AuthModel> GetToken(TokenRequestModel model);
	Task<string> AddRole(AddRoleModel model);
}
