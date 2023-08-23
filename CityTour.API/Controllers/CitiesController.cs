using AutoMapper;
using CityTour.API.Data;
using CityTour.API.Dtos;
using CityTour.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityTour.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		private IAppRepository _appRepository;
		private IMapper _mapper;

		public CitiesController(IAppRepository appRepository, IMapper mapper)
		{
			_appRepository = appRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("cities")]
		public ActionResult GetCities()
		{
			var cities = _appRepository.GetCities();
			//var cities = _appRepository.GetCities().Select(c=>c.Name);
			//var cities = _appRepository.GetCities().Select(c=> new CityForListDto {Id = c.Id, Name = c.Name,Description = c.Description, PhotoUrl = c.Photos.FirstOrDefault(p=>p.IsMain==true).Url }).ToList();//bunun yerine automapper ist.olunur
			var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities); 
			return Ok(citiesToReturn);
		}

		[HttpGet]
		[Route("detail")]
		public ActionResult GetCityById(int cityId)
		{
			var city = _appRepository.GetCityById(cityId);
			var cityToReturn = _mapper.Map<CityForDetailDto>(city);
			return Ok(city);
		}

		[HttpPost]
		[Route("add")]
		public ActionResult Add([FromBody]City city)
		{
			_appRepository.Add(city);
			_appRepository.SaveAll();
			return Ok(city);
		}

		[HttpGet]
		[Route("photos")]
		public ActionResult GetPhotosByCity(int cityId)
		{
			var photos = _appRepository.GetPhotosByCity(cityId);
			return Ok(photos);
		}

	}
}
