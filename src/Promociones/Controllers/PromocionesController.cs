using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Promociones.Model;
using Promociones.Services.Promociones;
using Promociones.Services.Repository;
using Promociones.Validators;
using Promociones.ViewModel;
using System;
using System.Collections.Generic;

namespace Promociones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPromocionesService _promocionesService;
        private readonly IPromocionesRepository _promocionesRepository;

        public PromocionesController(
            IMapper mapper,
            IPromocionesService promocionesService,
            IPromocionesRepository promocionesRepository)
        {
            _mapper = mapper;
            _promocionesService = promocionesService;
            _promocionesRepository = promocionesRepository;
        }

        [HttpGet]
        public ActionResult<List<PromocionViewModel>> Get() =>
            Ok(_promocionesService.ObtenerPromociones());

        [HttpGet("{id}")]
        public ActionResult<PromocionViewModel> Get(Guid id)
        {
            var promocion = _promocionesService.ObtenerPromocion(id);

            if (promocion == null)
                return NotFound();

            return promocion;
        }

        [HttpGet("Vigentes")]
        public ActionResult<List<PromocionViewModel>> GetVigentes() =>
            GetVigentesFecha(null);

        [HttpGet("Vigentes/{fecha}")]
        public ActionResult<List<PromocionViewModel>> GetVigentesFecha(DateTime? fecha) =>
            _promocionesService.ObtenerPromocionesVigentes(fecha);

        [HttpPost("Vigentes/Venta")]
        public ActionResult<IEnumerable<PromocionVentaViewModel>> GetVigenteVenta(FiltroVentaViewModel filtro) =>
            _promocionesService.ObtenerPromocionesVigentesVenta(filtro);

        [HttpPost]
        public ActionResult<Guid> Create(PromocionPostViewModel promocionPostViewModel)
        {
            try
            {
                var promocionView = _promocionesService.CrearPromocion(promocionPostViewModel);

                return Ok(promocionView.Id);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Guid> Update(Guid id, PromocionPostViewModel promocionIn)
        {
            try
            {
                var promocionView = _promocionesService.ActualizarPromocion(id, promocionIn);

                if (promocionView == null)
                    return NotFound();

                return Ok(promocionView.Id);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException);
            }
        }

        [HttpPut("{id}/Vigencia")]
        public ActionResult<Guid> UpdateVigencia(Guid id, VigenciaViewModel vigenciaViewModel)
        {
            try
            {
                var promocion = _promocionesService.ActualizarVigenciaPromocion(id, vigenciaViewModel);

                if (promocion == null)
                    return NotFound();

                return Ok(promocion.Id);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var promocion = _promocionesService.EliminarPromocion(id);

            if (promocion == null)
                return NotFound();

            return NoContent();
        }
    }
}
