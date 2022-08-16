using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Data.Services;
using Panopticon.Shared.Models;

namespace Panopticon.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/userRecord")]
    public class UserRecordController
    {
        UserRecordService _userRecordService { get; set; }
        public UserRecordController(UserRecordService userRecordService)
        {
            _userRecordService = userRecordService;
        }

        [HttpGet("{userId}", Name = "GetUserRecord")]
        public ActionResult<UserRecord> GetUserRecord(ulong userId)
        {
            UserRecord? userRecord = _userRecordService.GetUserRecord(userId);

            if(userRecord == null)
            {
                return new NotFoundResult();
            }

            return userRecord;
        }

        [HttpPut(Name = "UpdateUserRecord")]
        public ActionResult UpdateUserRecord([FromBody] UserRecord userRecord)
        {
            _userRecordService.UpdateOrCreateUserRecords(userRecord);

            return new NoContentResult();
        }
    }
}
