using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Shared.Models;
using Panopticon.Services;

namespace Panopticon.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/feedback")]
    public class FeedbackController : ControllerBase
    {
        public FeedbackService _feedback { get; set; }

        public FeedbackController(FeedbackService feedback)
        {
            _feedback = feedback;
        }

        [HttpGet(Name = "GetAllFeedback")]
        public IEnumerable<Feedback> GetAllFeedback()
        {
            return _feedback.GetAllFeedback();
        }

        [HttpPost(Name = "CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromQuery]ulong userId, [FromBody]string message)
        {
            await _feedback.CreateFeedback(userId, message);

            return new NoContentResult();
        }

        [HttpGet("{id}", Name = "GetSpecificFeedback")]
        public Feedback? GetFeedback([FromRoute] int id)
        {
            return _feedback.GetFeedback(id);
        }

        [HttpDelete("{id}", Name = "DeleteSpecificFeedback")]
        public IActionResult DeleteFeedback([FromRoute] int id)
        {
            Feedback feedback = _feedback.GetFeedback(id);

            if (feedback == null)
            {
                return new NotFoundResult();
            }

            _feedback.DeleteFeedback(feedback);

            return new NoContentResult();
        }
    }
}
