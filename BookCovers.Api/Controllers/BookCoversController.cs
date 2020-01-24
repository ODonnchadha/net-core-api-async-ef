using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BookCovers.Api.Controllers
{
    [ApiController(), Route("api/bookcovers")]
    public class BookCoversController : ControllerBase
    {
        /// <summary>
        /// Mimic a longer-runing action in order to mock receiving a book cover from an external service.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnFault"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetBookCover(string name, bool returnFault = false)
        {
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
