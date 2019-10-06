using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public CampsController(ICampRepository camprepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = camprepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        //public async Task<IActionResult> GetCamps()
        public async Task<ActionResult<CampModel[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetAllCampsAsync(includeTalks);
                //CampModel[] campModels = _mapper.Map<CampModel[]>(result);
                //return Ok(campModels);
                return _mapper.Map<CampModel[]>(result);

            }
            catch (Exception)
            {

                //return BadRequest("Database Failure");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database ERROR");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>>Get(string moniker)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker);
                if (result == null) return NotFound();
                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return BadRequest("Database Error");
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!result.Any()) return NotFound();
                return _mapper.Map<CampModel[]>(result);
            }
            catch (Exception)
            {

                return BadRequest("Database Error");
            }
        }

        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }
                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);
              
                if (await _campRepository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            return  BadRequest();
        }
    }
}