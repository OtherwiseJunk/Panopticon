using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Panopticon.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/userRecord")]
    public class UserRecordController
    {
    }
}
