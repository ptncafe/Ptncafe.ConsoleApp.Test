using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Ptncafe.Api.ES7.Test.Model;
using System.Threading.Tasks;

namespace Ptncafe.Api.ES7.Test.Controllers
{
    [Route("api/es7")]
    [ApiController]
    public class ESController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ESController> _logger;

        public ESController(ILogger<ESController> logger, IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int id, System.Threading.CancellationToken cancellationToken)
        {
            var data = await _elasticClient.GetAsync<TestModel>(id, idx => idx.Index("testindex"), cancellationToken);
            _logger.LogDebug("Get {0}", ObjectDumper.Dump(data));
            if (data.IsValid)
                return Ok(data);

            return StatusCode(500, data.DebugInformation);
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<ActionResult> Post([FromRoute] int id, System.Threading.CancellationToken cancellationToken)
        {
            TestModel testModel = new TestModel {
                Id = 1,
                IntValue = 1,
                Name = "name test",
                StringValue = "String Value StringValue"
            };
            var data = await _elasticClient.IndexAsync(testModel, cancellationToken);
            _logger.LogDebug("Get {0}", ObjectDumper.Dump(data));
            if (data.IsValid)
                return Ok(data);

            return StatusCode(500, data.DebugInformation);
        }

    }
}