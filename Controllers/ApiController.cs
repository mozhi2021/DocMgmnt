using DocMgmnt.Interface;
using Microsoft.AspNetCore.Mvc;
using DocMgmnt.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocMgmnt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IApiHandler _apiHandler;

        public ApiController(IApiHandler apiHandler) 
        {
            _apiHandler = apiHandler;
        }


        [HttpGet("{header}")]
        public IActionResult AuthenticateViaHeader()
        {
            string? apiKey = Request.Headers[Constants.ApiKeyHeaderName];

            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            bool isValid = _apiHandler.IsValidApiKey(apiKey);

            if (!isValid)
                return Unauthorized();

            return Ok();
        }    


        // GET api/<ApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
