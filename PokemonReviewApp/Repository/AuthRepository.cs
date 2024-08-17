namespace PokemonReviewApp.Repository;

public class AuthRepository : IAuthRepository
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly JWT _jwt;

	public AuthRepository(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_jwt = jwt.Value;
	}

	public async Task<AuthModel> Register(RegisterModel registerModel)
	{
		if (await _userManager.FindByEmailAsync(registerModel.Email) is not null)
			return new AuthModel { Message="This Email is already registerd!"};
		
		if (await _userManager.FindByNameAsync(registerModel.UserName) is not null)
			return new AuthModel { Message="This UserName is already registerd!"};

		//we can do it by mapper
		var user = new ApplicationUser
		{
			UserName = registerModel.UserName,
			Email = registerModel.Email,
			FirstName = registerModel.FirstName,
			LastName = registerModel.LastName,
		};

		var result=await _userManager.CreateAsync(user,registerModel.Password);//to make hashing to the password
		if (!result.Succeeded)
		{
			var errors = string.Empty;
			foreach(var error in result.Errors)
			{
				errors += $"{error.Description},";
			}
			return new AuthModel { Message = errors };
		}
		await _userManager.AddToRoleAsync(user, "Admin");
		//then we will create jwt token by separate function

		var jwtSecurityToken = await CreateJwtToken(user);

		return new AuthModel
		{
			Email = user.Email,
			ExpiresOn = jwtSecurityToken.ValidTo,
			IsAuthenticated = true,
			Roles = new List<string> { "Admin" },
			Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
			UserName = user.UserName
		};
	}


	public async Task<AuthModel> GetToken(TokenRequestModel model)
	{
		var authModel = new AuthModel(); //because we return auth model,,,,,create object from AuthModel empty

		var user = await _userManager.FindByEmailAsync(model.Email);

		if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
		{
			authModel.Message = "Email or Password is incorrect!";//we must return annoynmous message to didn't know the real error because of the hacking
			return authModel;
		}

		var jwtSecurityToken = await CreateJwtToken(user);
		var rolesList = await _userManager.GetRolesAsync(user);

		authModel.IsAuthenticated = true;
		authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
		authModel.Email = user.Email;
		authModel.UserName = user.UserName;
		authModel.ExpiresOn = jwtSecurityToken.ValidTo;
		authModel.Roles = rolesList.ToList();

		return authModel;
	}

	public async Task<string> AddRole(AddRoleModel model)
	{
		var user = await _userManager.FindByIdAsync(model.UserId);

		if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
			return "Invalid user ID or Role";

		if (await _userManager.IsInRoleAsync(user, model.Role))
			return "User already assigned to this role";

		var result = await _userManager.AddToRoleAsync(user, model.Role);

		return result.Succeeded ? string.Empty : "Something went wrong";
	}

	private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
	{
		var userClaims = await _userManager.GetClaimsAsync(user);//we get claims of the user
		var roles = await _userManager.GetRolesAsync(user);//we get the role of user
		var roleClaims = new List<Claim>();

		foreach (var role in roles)
			roleClaims.Add(new Claim("roles", role));//we add all the roles of the user to claims

		var claims = new[]
		{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//add date in claims
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("uid", user.Id)
			}
		.Union(userClaims)
		.Union(roleClaims);

		var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
		var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

		var jwtSecurityToken = new JwtSecurityToken(
			issuer: _jwt.Issuer,//pass the data in appsetting
			audience: _jwt.Audience,
			claims: claims,//the claims that we have added it recently
			expires: DateTime.Now.AddDays(_jwt.DurationInDays),
			signingCredentials: signingCredentials);

		return jwtSecurityToken;
	}
}

