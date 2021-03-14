using AutoMapper;
using FluentValidation;
using MongoDB.Driver;
using Promociones.Model;
using Promociones.Services.Repository;
using Promociones.Validators;
using Promociones.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Promociones.Services.Promociones.Net
{
    public class PromocionesService : IPromocionesService
    {
        private readonly IMapper _mapper;
        private readonly IPromocionesRepository _promocionesRepository;

        public PromocionesService(
            IMapper mapper,
            IPromocionesRepository promocionesRepository)
        {
            _mapper = mapper;
            _promocionesRepository = promocionesRepository;
        }

        public IList<PromocionViewModel> ObtenerPromociones() =>
            _mapper.Map<List<PromocionViewModel>>(_promocionesRepository.Get());

        public PromocionViewModel ObtenerPromocion(Guid id) =>
            _mapper.Map<PromocionViewModel>(_promocionesRepository.Get(id));

        public List<PromocionViewModel> ObtenerPromocionesVigentes(DateTime? fecha = null) =>
            _mapper.Map<List<PromocionViewModel>>(VigentesQuery(fecha).ToList());

        public List<PromocionVentaViewModel> ObtenerPromocionesVigentesVenta(FiltroVentaViewModel filtro)
        {
            var promocionQuery = VigentesQuery(null);

            if (!string.IsNullOrWhiteSpace(filtro.MedioDePago))
                promocionQuery = promocionQuery.Where(p => p.MediosDePago.Contains(filtro.MedioDePago));
            if (!string.IsNullOrWhiteSpace(filtro.Banco))
                promocionQuery = promocionQuery.Where(p => p.Bancos.Contains(filtro.Banco));
            if (filtro.CategoriaProducto != null && filtro.CategoriaProducto.Count() > 0)
                promocionQuery = promocionQuery.Where(p => filtro.CategoriaProducto.Where(cp => p.CategoriasProductos.Contains(cp)).Any());

            return _mapper.Map<List<PromocionVentaViewModel>>(promocionQuery.ToList());
        }

        public PromocionViewModel CrearPromocion(PromocionPostViewModel promocionPostViewModel)
        {
            var promocion = _mapper.Map<Promocion>(promocionPostViewModel);

            // TODO: VALIDAR
            promocion.Activo = true;
            promocion.FechaCreacion = DateTime.UtcNow;

            Validate(promocion);

            return _mapper.Map<PromocionViewModel>(_promocionesRepository.Create(promocion));
        }

        public PromocionViewModel ActualizarPromocion(Guid id, PromocionPostViewModel promocionPostViewModel)
        {
            var promocion = _promocionesRepository.Get(id);

            if (promocion == null)
                return null;

            // TODO: VALIDAR
            promocion.FechaModificacion = DateTime.UtcNow;

            promocion = _mapper.Map(promocionPostViewModel, promocion);

            Validate(promocion);

            _promocionesRepository.Update(id, promocion);

            return _mapper.Map<PromocionViewModel>(promocion);
        }

        public PromocionViewModel ActualizarVigenciaPromocion(Guid id, VigenciaViewModel vigenciaViewModel)
        {
            var promocion = _promocionesRepository.Get(id);

            if (promocion == null)
                return null;

            promocion.FechaInicio = vigenciaViewModel.FechaInicio;
            promocion.FechaFin = vigenciaViewModel.FechaFin;

            Validate(promocion);

            _promocionesRepository.Update(id, promocion);

            return _mapper.Map<PromocionViewModel>(promocion);
        }

        public PromocionViewModel EliminarPromocion(Guid id)
        {
            var promocion = _promocionesRepository.Get(id);

            if (promocion == null)
                return null;

            promocion.Activo = false;

            _promocionesRepository.Update(id, promocion);

            return _mapper.Map<PromocionViewModel>(promocion);
        }

        #region Metodo privados de ayuda
        IQueryable<Promocion> VigentesQuery(DateTime? fecha = null)
        {
            var filterDate = fecha.HasValue ? fecha.Value.Date.AddDays(1).AddMilliseconds(-1) : DateTime.UtcNow;
            return _promocionesRepository.Query()
                .Where(p =>
                    p.Activo
                    && p.FechaInicio.HasValue && p.FechaInicio <= filterDate
                    && (!p.FechaFin.HasValue || p.FechaFin.Value >= filterDate)
                );
        }

        void Validate(Promocion promocion)
        {
            var validator = new PromocionValidator(_promocionesRepository);
            var results = validator.Validate(promocion);
            if (!results.IsValid)
                throw new ValidationException(results.Errors);
        }
        #endregion
    }
}
