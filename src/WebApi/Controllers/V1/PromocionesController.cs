using Application.Features.Promociones.Commands.CreatePromocion;
using Application.Features.Promociones.Commands.DeletePromocionById;
using Application.Features.Promociones.Commands.UpdatePromocion;
using Application.Features.Promociones.Commands.UpdateVigenciaPromocion;
using Application.Features.Promociones.Queries.GetAllPromociones;
using Application.Features.Promociones.Queries.GetAllPromocionesVigentes;
using Application.Features.Promociones.Queries.GetPromocionById;
using Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta;
using Application.Features.Promociones.Queries.GetPromocionesVigentesPorFecha;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Parameters;

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
        public async Task<IActionResult> Post(CreatePromocionCommand command) =>
             Ok(await Mediator.Send(command));

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
        public async Task<IActionResult> Delete(Guid id) =>
            Ok(await Mediator.Send(new DeletePromocionByIdCommand { Id = id }));

        [HttpGet("Vigentes")]
        public async Task<IActionResult> GetVigentes(GetPromocionesVigentesQuery command) =>
            Ok(await Mediator.Send(command));

        [HttpGet("Vigentes/{fecha}")]
        public async Task<IActionResult> GetVigentesFecha(DateTime fecha) =>
            Ok(await Mediator.Send(new GetPromocionesVigentesPorFechaQuery { Date = fecha }));

        [HttpPost("Vigentes/Venta")]
        public async Task<IActionResult> GetVigenteVenta(FiltroVenta filtro) =>
            Ok(await Mediator.Send(new GetPromocionesVigentesParaVentaQuery { Banco = filtro.Banco, CategoriaProducto = filtro.CategoriaProducto, MedioDePago = filtro.MedioDePago }));

        [HttpPut("{id}/Vigencia")]
        public async Task<IActionResult> UpdateVigencia(Guid id, VigenciaModel vigencia) =>
            Ok(await Mediator.Send(new UpdateVigenciaPromocionCommand { Id = id, FechaInicio = vigencia.FechaInicio, FechaFin = vigencia.FechaFin }));
    }
}
