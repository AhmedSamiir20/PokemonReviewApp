var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//JWT here we will map the values in appsetting to the class we had been created

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));



builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));//get the data from jwt that in appsetting to the class in helper that name jwt

builder.Services.AddAuthorization();

//------Identity Identity---------

builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


//------Identity Identity---------

builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped < ICountryRepository, CountryRepository>();
builder.Services.AddScoped < IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped < IReviewRepository, ReviewRepository>();
builder.Services.AddScoped < IReviewerRepository, ReviewerRepository>();
builder.Services.AddScoped < IAuthRepository, AuthRepository>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version= "v1",
		Title="PokemonReviewApp",
		Contact=new OpenApiContact
		{
			Name="Ahmed Samir",
			Email="ahmedsamiirr20@gmail.com", 
		}
		
	});
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme="Bearer",
		BearerFormat="JWT",
		In=ParameterLocation.Header,
		Description="Enter your JWT Key"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference=new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				},
				Name="Bearer",
				In=ParameterLocation.Header
			},
			new List<string>()
		}
	});
});

builder.Services.AddCors();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//here i told to him that every time i will use Bearer
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(o =>
	{
		o.RequireHttpsMetadata = false;
		o.SaveToken = false;
		o.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,//i told to him to validate the issuer and key and Audience
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidIssuer = builder.Configuration["JWT:Issuer"],//i know him where will get the values
			ValidAudience = builder.Configuration["JWT:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
			//then we will build the endpoint that check about the role and give u token to start use api
		};
	});


builder.Services.AddControllers().AddJsonOptions(x=>
x.JsonSerializerOptions.ReferenceHandler=System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);//it happen in many to many relationship because the entity get stuck in aloop in order for it 




var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();//is user allow to use this application first or no? Authentications then Authorization

app.UseAuthorization();

app.MapControllers();

app.Run();
