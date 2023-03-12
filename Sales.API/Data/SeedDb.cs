using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sales.API.Services;
using Sales.API.Data;
using Sales.Shared.Entities;
using Sales.Shared.Responses;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Sales.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;
        public SeedDb(DataContext context, IApiService apiService) 
        {
            _context = context;
            _apiService = apiService;
        } 

        public async Task SeedAsync ()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                Response responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSucess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        Country country = (await _context.Countries.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!))!;
                        if (country == null)
                        {
                            country = new() { Name = countryResponse.Name!, States = new List<State>() };
                            Response responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");

                            if (responseStates.IsSucess)
                            {
                                List<StateResponse> states = (List<StateResponse>)responseStates.Result!;
                                foreach (StateResponse stateResponse in states!)
                                {
                                    State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                    if (state == null)
                                    {
                                        state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
                                        Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                        if(responseCities.IsSucess)
                                        {
                                            List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;
                                            if (cities is not null || state.Cities.Count > 0)
                                            {                                            
                                                foreach (CityResponse cityResponse in cities)
                                                {
                                                    if(cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița" || cityResponse is null)
                                                    {
                                                        continue;
                                                    }
                                                    City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                                    if(city == null)
                                                    {
                                                        state.Cities.Add(new City() { Name = cityResponse.Name!});
                                                    }
                                                }
                                            }
                                        }
                                        if(state.CitiesNumber > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }
                                }
                            }
                            if(country.StatesNumber> 0) 
                            {
                                _context.Countries.Add(country);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Calzado" });
            }
            await _context.SaveChangesAsync();
        }

    }
}
