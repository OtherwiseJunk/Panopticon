using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Data.Services;
using Panopticon.Shared.Models;

namespace Panopticon.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/ooc")]
    public class OOCController : ControllerBase
    {
        OOCService _service { get; set; }

        public OOCController(OOCService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<OOCItem> GetOOCItemByID([FromQuery] int id)
        {
            OOCItem? item = _service.GetOOCItem(id);
            if (item != null)
            {
                return item;
            }
            return new NotFoundResult();
        }

        [HttpDelete]
        public ActionResult<OOCItem> DeleteOOCItemByID([FromQuery] int id)
        {
            OOCItem? item = _service.GetOOCItem(id);
            if (item != null)
            {
                _service.DeleteOOCItem(item);
                return new NoContentResult();
            }
            return new NotFoundResult();
        }

        [HttpPost]
        public ActionResult CreateOOCItem([FromBody]OOCItem item)
        {
            try
            {
                _service.CreateOOCitem(item);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encountered an exception while creating an OOC Item: {ex.Message}");
                return new UnprocessableEntityResult();
            }
        }

        [HttpGet("rand")]
        public ActionResult<OOCItem> GetRandomOOCItem()
        {
            OOCItem? item = _service.GetRandomOOCItem();
            if(item != null)
            {
                return item;
            }
            return new NotFoundResult();
        }        
    }
}
