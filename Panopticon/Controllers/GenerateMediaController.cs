using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Services;

namespace Panopticon.Controllers
{
    [Authorize]
    [Route("/media")]
    [ApiController]
    public class GenerateMediaController : ControllerBase
    {
        public FFMPEGService _ffmpeg { get; set; }
        private DOSpacesService _s3 { get; set; }

        public GenerateMediaController(FFMPEGService ffmpeg, DOSpacesService s3)
        {
            _ffmpeg = ffmpeg;
            _s3 = s3;
        }

        [HttpGet("/wilhelm")]
        public async Task<ActionResult<string>> GetWilhelmURL([FromQuery]string url, [FromQuery]MediaType mediaType)
        {
            string filename = Guid.NewGuid().ToString();
            if(await _ffmpeg.AddWilhelmToAttachment(url, mediaType, filename))
            {
                using (Stream stream = System.IO.File.OpenRead($"{filename}.mp4"))
                {
                    string wilhelmUrl = _s3.UploadMedia("Wilhelm", stream, "mp4");
                    System.IO.File.Delete($"{filename}.mp4");
                    return new CreatedAtActionResult(null,null, null, wilhelmUrl);
                }
            }
            else
            {
                return new UnprocessableEntityResult();
            }
        }
    }
}
