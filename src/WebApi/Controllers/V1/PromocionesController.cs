using Application.Features.Promociones.Commands.CreatePromocion;
using Application.Features.Promociones.Commands.DeletePromocionById;
using Application.Features.Promociones.Commands.UpdatePromocion;
using Application.Features.Promociones.Queries.GetAllPromociones;
using Application.Features.Promociones.Queries.GetPromocionById;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers.V1
{
    public class PromocionesController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllPromocionesQuery()));

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id) =>
            Ok(await Mediator.Send(new GetPromocionByIdQuery { Id = id }));

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post(CreatePromocionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdatePromocionCommand command)
        {
            if (id != command.Id)
                return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new DeletePromocionByIdCommand { Id = id }));
        }
    }
}
