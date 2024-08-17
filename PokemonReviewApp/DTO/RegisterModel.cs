namespace PokemonReviewApp.DTO;

public class RegisterModel
{
	[Required,StringLength(50)]
	public string FirstName { get; set; }
	
	[Required,StringLength(50)]
	public string LastName { get; set; }
	
	[Required,StringLength(50)]
	public string UserName { get; set; }
	
	[Required,StringLength(128),EmailAddress]
	public string Email { get; set; }
	
	[Required,StringLength(256),PasswordPropertyText]
	public string Password { get; set; }
}
