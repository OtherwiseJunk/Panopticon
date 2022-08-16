using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Shared.Models;
using Panopticon.Data.Services;

namespace Panopticon.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/feedback")]
    public class FeedbackController : ControllerBase
    {
        public FeedbackService _feedbackService { get; set; }

        public FeedbackController(FeedbackService feedback)
        {
            _feedbackService = feedback;
        }

        [HttpGet(Name = "GetAllFeedback")]
        public ActionResult<List<Feedback>> GetAllFeedback()
        {
            List<Feedback> feedback = _feedbackService.GetAllFeedback();

            if(feedback.Count == 0)
            {
                return new NotFoundResult();
            }

            return feedback;
        }

        [HttpPost(Name = "CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromQuery]ulong userId, [FromBody]string message)
        {
            await _feedbackService.CreateFeedback(userId, message);

            return new NoContentResult();
        }

        [HttpGet("{id}", Name = "GetSpecificFeedback")]
        public ActionResult<Feedback> GetFeedback([FromRoute] int id)
        {
            Feedback? feedback = _feedbackService.GetFeedback(id);

            if (feedback == null)
            {
                return new NotFoundResult();
            }

            return feedback;
        }

        [HttpDelete("{id}", Name = "DeleteSpecificFeedback")]
        public IActionResult DeleteFeedback([FromRoute] int id)
        {
            Feedback feedback = _feedbackService.GetFeedback(id);

            if (feedback == null)
            {
                return new NotFoundResult();
            }

            _feedbackService.DeleteFeedback(feedback);

            return new NoContentResult();
        }
    }
}
