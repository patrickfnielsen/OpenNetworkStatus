using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Models;
using OpenNetworkStatus.Services.IncidentServices.Queries;

namespace OpenNetworkStatus.Controllers
{
    public class IncidentController : Controller
    {
        private readonly IMediator _mediator;

        public IncidentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("[controller]/{incidentId}")]
        public async Task<IActionResult> GetById([FromRoute]GetIncidentByIdQuery incidentQuery)
        {
            var incidentResource = await _mediator.Send(incidentQuery);
            if (incidentResource == null)
            {
                return NotFound();
            }

            return View(new IncidentViewModel(incidentResource.Impact, incidentResource.Title, incidentResource.Updates));
        }
    }
}
