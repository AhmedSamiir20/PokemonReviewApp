namespace PokemonReviewApp.Helper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Pokemon, PokemonDto>().ReverseMap();
		CreateMap<Category, CategoryDto>().ReverseMap();
		CreateMap<Country, CountryDto>().ReverseMap();
		CreateMap<Owner, OwnerDto>().ReverseMap();
		CreateMap<Review, ReviewDto>().ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(sorc => sorc.Reviewer.FirstName));
		CreateMap<ReviewDto, Review>();
		CreateMap<Reviewer, ReviewerDto>().ReverseMap();

	}
}
