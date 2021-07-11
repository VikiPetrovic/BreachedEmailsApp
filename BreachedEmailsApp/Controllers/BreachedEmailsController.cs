using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreachedEmailsApp.Helpers;

namespace BreachedEmailsApp
{
    
    [Route("home")]
    public class BreachedEmailsController : Controller
    {
        private readonly IClusterClient _clusterClient;
        private IEnumerable<EndpointDataSource> _endpointSources;

        public BreachedEmailsController(IClusterClient clusterClient, IEnumerable<EndpointDataSource> endpointSources)
        {
            this._clusterClient = clusterClient;
            _endpointSources = endpointSources;
        }

        [HttpGet("emails/{email}")]
        public async Task<IActionResult> GetEmail(string email)
        {
            try
            {
                Email e = new Email(email: email);
                string res = await _clusterClient.GetGrain<IDomainGrain>(e.domain).Get(email: e);

                if (string.IsNullOrEmpty(res))
                {
                    return NotFound();
                }
                else
                {
                    return Ok();
                }
                
            }
            catch (Exception emailEx)
            {
                return Conflict();
            }
        }

        [HttpPost("emails/{email}")]
        public async Task<IActionResult> CreateEmail(string email)
        {
            // call create email grain method
            try
            {
                Email e = new Email(email: email);
                await _clusterClient.GetGrain<IDomainGrain>(e.domain).Create(email: e);
                return Ok();
            }
            catch(Exception e)
            {
                return Conflict();
            }          
        }

        [HttpGet]
        public JsonResult Get()
        {
            var endpoints = _endpointSources
            .SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>();
            var output = endpoints.Select(
                e =>
                {
                    var controller = e.Metadata
                        .OfType<ControllerActionDescriptor>()
                        .FirstOrDefault();                    

                    return new
                    {
                        Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?[0],
                        Route = $"/{e.RoutePattern.RawText.TrimStart('/')}"
                    };
                }
            );

            return Json(output);
        }
    }
}
