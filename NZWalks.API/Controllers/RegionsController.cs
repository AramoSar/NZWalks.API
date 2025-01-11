using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }



        // GET ALL REGIONS
        // GET: https://localhost:portnumber/api/regions
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            var regionsDomain = await regionRepository.GetAllAsync();

            /*var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Name = regionDomain.Name,
                    Code = regionDomain.Code,
                    RegionImageUrl = regionDomain.RegionImageUrl,
                });
            }*/

            // Return DTOs
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            return Ok(regionsDto);
        }

        // GET SINGLE REGION(By Id)
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // var region = dbContext.Regions.Find(id);

            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            /*var regionDto = new RegionDto()
            {
                Id = regionDomain.Id,
                Name = regionDomain.Name,
                Code = regionDomain.Code,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };*/

            var regionDto = mapper.Map<RegionDto>(regionDomain);

            return Ok(regionDto);

        }

        // POST To Create New Region
        // GET: https://localhost:portnumber/api/regions

        [HttpPost]
        [Authorize(Roles = "Writer")]
        //[ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            
                // Map or Convert DTO to Domain Model
                /*var regionDomainModel = new Region
                {
                    Code = addRegionRequestDto.Code,
                    Name = addRegionRequestDto.Name,
                    RegionImageUrl = addRegionRequestDto.RegionImageUrl
                };*/

                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                // Use Domain Model To Create Region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //Map Domaun Model back to DTO
                /*var regionDto = new RegionDto
                {
                    Id = regionDomainModel.Id,
                    Code = regionDomainModel.Code,
                    Name = regionDomainModel.Name,
                    RegionImageUrl = regionDomainModel.RegionImageUrl
                };*/

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);
            
            
            
        }

        // Update Region
        // PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto) 
        {
            // Map Dto to Domain Model
            /*var regionDomainModel = new Region
            {
                Code = updateRegionRequestDto.Code,
                Name = updateRegionRequestDto.Name,
                RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            };*/

            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            // Check if region exists
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }


            // Convert Domain Model to DTO
            /*var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };*/

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);

        }

        // DELETE REGION
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id)
        {
            
                var regionDomainModel = await regionRepository.DeleteAsync(id);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                // return deleted region back
                // Convert Domain Model to DTO
                /*var regionDto = new RegionDto
                {
                    Id = regionDomainModel.Id,
                    Code = regionDomainModel.Code,
                    Name = regionDomainModel.Name,
                    RegionImageUrl = regionDomainModel.RegionImageUrl
                };*/

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return Ok(regionDto);
            
            
            
        }
    }
}
