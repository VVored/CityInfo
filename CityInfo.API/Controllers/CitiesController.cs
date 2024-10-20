﻿using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfIntrestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }
            var (citiesEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfIntrestDto>>(citiesEntities));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (cityToReturn == null)
            {
                return NotFound();
            }
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(cityToReturn));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfIntrestDto>(cityToReturn));
        }
    }
}
