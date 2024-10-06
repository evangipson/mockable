using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

using Mockable.Attributes;

namespace Mockable.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class ApplicationController(ILogger<ApplicationController> logger) : ControllerBase
    {
        public class SomeClass
        {
            public string? Name { get; set; }

            public int Age { get; set; }

            public Guid UniqueId { get; set; }

            public DateTime Birthday { get; set; }
        }

        private readonly ILogger<ApplicationController> _logger = logger;

        [HttpGet]
        [Mockable]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<SomeClass> Get() => await Task.FromResult(new SomeClass()
        {
            Name = "Test API"
        });

        [HttpGet("async")]
        [Mockable]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<SomeClass> GetAsyncResponse() => await Task.FromResult(new SomeClass()
        {
            Name = "Async Test API"
        });
    }
}
