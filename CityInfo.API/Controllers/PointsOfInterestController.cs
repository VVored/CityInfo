using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointsOfInterest = await _cityInfoRepository.GetPointOfInterestsAsync(cityId);
            if (pointsOfInterest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
        }
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestsAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }
       /* [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, [FromBody] PointsOfInterestForCreationDto pointsOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointOfInterests).Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointsOfInterest.Name,
                Description = pointsOfInterest.Description
            };
            city.PointOfInterests.Add(finalPointOfInterest);
            return CreatedAtRoute(
                routeName: "GetPointOfInterest",
                routeValues: new
                {
                    cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                value: finalPointOfInterest
            );
        }
        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, [FromBody] PointsOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var editPointOfInterest = city.PointOfInterests.FirstOrDefault(pointOfInterest => pointOfInterest.Id == pointOfInterestId);
            if (editPointOfInterest == null)
            {
                return NotFound();
            }
            editPointOfInterest.Name = pointOfInterest.Name;
            editPointOfInterest.Description = pointOfInterest.Description;

            return NoContent();
        }
        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointsOfInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var editPointOfInterest = city.PointOfInterests.FirstOrDefault(pointOfInterest => pointOfInterest.Id == pointOfInterestId);
            if (editPointOfInterest == null)
            {
                return NotFound();
            }
            var pointOfInterestPatch = new PointsOfInterestForUpdateDto()
            {
                Name = editPointOfInterest.Name,
                Description = editPointOfInterest.Description,
            };
            patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
            if (TryValidateModel(pointOfInterestPatch))
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            editPointOfInterest.Name = pointOfInterestPatch.Name;
            editPointOfInterest.Description = pointOfInterestPatch.Description;
            return NoContent();
        }
        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var deletePointOfInterest = city.PointOfInterests.FirstOrDefault(pointOfInterest => pointOfInterest.Id == pointOfInterestId);
            if (deletePointOfInterest == null)
            {
                return NotFound();
            }
            city.PointOfInterests.Remove(deletePointOfInterest);
            _mailService.Send("Point of interest deleted.", $"Point of interest {deletePointOfInterest.Name} with id {deletePointOfInterest.Id} was deleted.");
            return NoContent();
        }*/
    }
}
