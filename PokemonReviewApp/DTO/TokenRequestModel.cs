namespace PokemonReviewApp.DTO;

public class TokenRequestModel
{
	[Required,EmailAddress]
	public string Email { get; set; }
	
	[Required,PasswordPropertyText]
	public string Password { get; set; }
}
