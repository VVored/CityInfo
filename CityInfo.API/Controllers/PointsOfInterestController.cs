using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointOfInterests);
        }
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null) {
                return NotFound();
            }
            return Ok(pointOfInterest);
        }
        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, [FromBody] PointsOfInterestForCreationDto pointsOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointOfInterests).Max(p => p.Id);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            return NoContent();
        }
    }
}
