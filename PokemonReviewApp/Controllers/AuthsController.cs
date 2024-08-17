namespace PokemonReviewApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthsController : ControllerBase
{
	private readonly IAuthRepository _authRepository;

	public AuthsController(IAuthRepository authRepository)
	{
		_authRepository = authRepository;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterModel model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result=await _authRepository.Register(model);
		if(!result.IsAuthenticated)
			return BadRequest(result.Message);

		return Ok(result);   //123456789As$ the password
	}
	
	[HttpPost("token")]
	public async Task<IActionResult> GetToken([FromBody] TokenRequestModel model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result=await _authRepository.GetToken(model);
		if(!result.IsAuthenticated)
			return BadRequest(result.Message);

		return Ok(result);   //123456789As$ the password
	}

	[HttpPost("addRole")]
	public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _authRepository.AddRole(model);
		if (!string.IsNullOrEmpty(result))
			return BadRequest(result); 

		return Ok(model);   //123456789As$ the password
	}
}
