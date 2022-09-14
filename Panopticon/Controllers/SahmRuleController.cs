using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Services;
using Panopticon.Shared.Models;

namespace Panopticon.Controllers
{
    [ApiController]
    [Route("/sahm")]
    public class SahmRuleController : Controller
    {
        FREDService _fredService { get; set; }
        public SahmRuleController(FREDService service)
        {
            _fredService = service;
        }
        [HttpGet()]
        public ActionResult<SahmRuleObservation> GetLatestSahmRuleValue()
        {
            List<SahmRuleObservation> observations = _fredService.GetSahmRuleObservations().Result;

            if (observations.Count > 0)
            {
                SahmRuleObservation observation = observations.OrderByDescending(sro => sro.Date).First();
                return observation;
            }
            return new NotFoundResult();
        }

        [HttpGet("{startDate}")]
        public ActionResult<List<SahmRuleObservation>> GetSahmRuleObservationsFromDate(DateTime startDate)
        {
            List<SahmRuleObservation> observations = _fredService.GetSahmRuleObservations(startDate).Result;

            if (observations.Count > 0)
            {
                return observations;
            }
            return new NotFoundResult();
        }

        [HttpGet("all")]
        public ActionResult<List<SahmRuleObservation>> GetAllSahmRuleObservations()
        {
            List<SahmRuleObservation> observations = _fredService.GetSahmRuleObservations().Result;

            if (observations.Count > 0)
            {
                return observations;
            }
            return new NotFoundResult();
        }
    }
}
