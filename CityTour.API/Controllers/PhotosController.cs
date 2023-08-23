using System.Security.Claims;
using AutoMapper;
using CityTour.API.Data;
using CityTour.API.Dtos;
using CityTour.API.Helpers;
using CityTour.API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CityTour.API.Controllers
{
	[Route("api/cities/{cityId}/photos")]
	[ApiController]
	public class PhotosController : ControllerBase
	{
		private IAppRepository _appRepository;
		private IMapper _mapper;
		private IOptions<CloudinarySettings> _cloudinaryConfig;
		private Cloudinary _cloudinary;

		public PhotosController(IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper, IAppRepository appRepository)
		{
			_cloudinaryConfig = cloudinaryConfig;
			_mapper = mapper;
			_appRepository = appRepository;

			var account = new Account(_cloudinaryConfig.Value.CloudName,_cloudinaryConfig.Value.ApiKey,_cloudinaryConfig.Value.ApiSecret);
			_cloudinary = new Cloudinary(account);
		}

		[HttpPost]
		public ActionResult AddPhotoForCity(int cityId, [FromBody]PhotoForCreationDto photoForCreationDto)
		{
			var city = _appRepository.GetCityById(cityId);
			if (city == null)
			{
				return BadRequest("Could not find the city");
			}

			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			if (currentUserId!=city.UserId)
			{
				return Unauthorized();
			}

			var file = photoForCreationDto.File;
			var uploadResult = new ImageUploadResult();
			if (file.Length>0)
			{
				using(var stream = file.OpenReadStream())
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(file.Name, stream)
					};
					uploadResult = _cloudinary.Upload(uploadParams);
				}
			}

			photoForCreationDto.Url = uploadResult.Uri.ToString();
			photoForCreationDto.PublicId = uploadResult.PublicId;
			var photo = _mapper.Map<Photo>(photoForCreationDto);
			photo.City = city;
			if(!city.Photos.Any(p=>p.IsMain))
			{
				photo.IsMain = true;
			}

			city.Photos.Add(photo);
			if (_appRepository.SaveAll())
			{
				var photoToReturn = _mapper.Map<Photo>(photo);
				return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
			}

			return BadRequest("Could not add the photo");
		}




		[HttpGet("{id}",Name = "GetPhoto")]
		public ActionResult GetPhoto(int id)
		{
			var photoFromDb = _appRepository.GetPhoto(id);
			var photo = _mapper.Map<PhotoForReturnDto>(photoFromDb);

			return Ok(photo);
		}

	}
}
