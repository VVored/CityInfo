using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, [FromBody] PointsOfInterestForCreationDto pointsOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointsOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
            
            return CreatedAtRoute(
                routeName: "GetPointOfInterest",
                routeValues: new
                {
                    cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                value: createdPointOfInterestToReturn
            );
        }
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, [FromBody] PointsOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestsAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointsOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestsAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            var pointOfInterestPatch = _mapper.Map<PointsOfInterestForUpdateDto>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
            if (TryValidateModel(pointOfInterestPatch))
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(pointOfInterestPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var deletePointOfInterest = await _cityInfoRepository.GetPointOfInterestsAsync(cityId, pointOfInterestId); 
            if (deletePointOfInterest == null)
            {
                return NotFound();
            }
            await _cityInfoRepository.DeletePointOfInterestForCityAsync(cityId, deletePointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            _mailService.Send("Point of interest deleted.", $"Point of interest {deletePointOfInterest.Name} with id {deletePointOfInterest.Id} was deleted.");
            return NoContent();
        }
    }
}
