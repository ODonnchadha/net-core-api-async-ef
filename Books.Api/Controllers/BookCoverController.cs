using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCoverController : ControllerBase
    {
        public async Task<IActionResult> GetBookCover(string name, bool returnFault = false)
        {
            // If returnFault == true, wait 500 miliseconds and return Internal Server Error.
            if (returnFault)
            {
                await Task.Delay(500);
                return new StatusCodeResult(500);
            }

            var random = new Random { };
            int fakeCoverBytes = random.Next(2097152, 10485760);
            byte[] fakeCover = new byte[fakeCoverBytes];
            random.NextBytes(fakeCover);

            return Ok(new { Name = name, Content = fakeCover });
        }
    }
}