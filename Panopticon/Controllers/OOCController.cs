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
        public ActionResult<List<OOCItem>> GetAllOOCItems()
        {
            List<OOCItem> items = _service.GetAllOOCItems();
            if (items.Count != 0)
            {
                return items;
            }
            return new NotFoundResult();
        }

        [HttpGet("{id}")]
        public ActionResult<OOCItem> GetOOCItemByID(int id)
        {
            OOCItem? item = _service.GetOOCItem(id);
            if (item != null)
            {
                return item;
            }
            return new NotFoundResult();
        }

        [HttpDelete("{id}")]
        public ActionResult<OOCItem> DeleteOOCItemByID(int id)
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
        public async Task<ActionResult> CreateOOCItem([FromBody]OOCItem item)
        {
            try
            {
                await _service.CreateOOCitem(item);
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

        [HttpGet("allUrl")]
        public ActionResult<string[]> GetAllOccUrls()
        {
            List<OOCItem> items = _service.GetAllOOCItems();
            if (items.Count != 0)
            {
                return items.Select((item) => item.ImageUrl).ToArray();
            }
            return new NotFoundResult();
        }
    }
}
