using Application.Features.Promociones.Commands.CreatePromocion;
using Application.Features.Promociones.Commands.UpdatePromocion;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using System;
using System.Linq;

namespace Application.Features.Promociones.Commands
{
    public class PromocionCommandModelValidator : AbstractValidator<CreatePromocionCommand>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        private readonly IMapper _mapper;

        public PromocionCommandModelValidator(IPromocionRepositoryAsync promocionRepository, IMapper mapper)
        {
            _promocionRepository = promocionRepository;
            _mapper = mapper;

            var medioDePagos = new string[]
            {
                "TARJETA_CREDITO",
                "TARJETA_DEBITO",
                "EFECTIVO",
                "GIFT_CARD"
            };

            var bancos = new string[]
            {
                "Galicia",
                "Santander Rio",
                "Ciudad",
                "Nacion",
                "ICBC",
                "BBVA",
                "Macro"
            };

            var categorias = new string[]
            {
                "Hogar",
                "Jardin",
                "ElectroCocina",
                "GrandesElectro",
                "Colchones",
                "Celulares",
                "Tecnologia",
                "Audio"
            };

            RuleFor(p => p.ValorInteresCuotas)
                .Null()
                .When(p => !p.MaximaCantidadDeCuotas.HasValue || p.MaximaCantidadDeCuotas.Value > 0)
                .WithMessage(p => $"{nameof(p.ValorInteresCuotas)} solo puede tener valor si {nameof(p.MaximaCantidadDeCuotas)} tiene valor.");

            RuleFor(p => p.PorcentajeDeDescuento)
                .InclusiveBetween(5, 80)
                .When(p => p.PorcentajeDeDescuento.HasValue && p.PorcentajeDeDescuento.Value > 0)
                .WithMessage(p => $"{nameof(p.PorcentajeDeDescuento)} debe ser superio o igual a 5, y menor o igual a 80.");

            RuleFor(p => p.FechaFin)
                .GreaterThan(p => p.FechaInicio)
                .When(p => p.FechaFin.HasValue && p.FechaInicio.HasValue)
                .WithMessage(p => $"{nameof(p.FechaFin)} no puede ser menor a {nameof(p.FechaInicio)}.");

            RuleFor(p => p.MediosDePago).Custom((list, context) =>
            {
                if (list.ToList().Any(s => !medioDePagos.Contains(s)))
                    context.AddFailure($"Los medios de pago válidos son: {string.Join(", ", medioDePagos)}");
            });

            RuleFor(p => p.Bancos).Custom((list, context) =>
            {
                if (list.ToList().Any(s => !bancos.Contains(s)))
                    context.AddFailure($"Los bancos válidos son: {string.Join(", ", bancos)}");
            });

            RuleFor(p => p.CategoriasProductos).Custom((list, context) =>
            {
                if (list.ToList().Any(s => !categorias.Contains(s)))
                    context.AddFailure($"Las categorias de productos válidos son: {string.Join(", ", categorias)}");
            });

            RuleFor(p => p).Custom((prom, context) =>
            {

                if (prom.MaximaCantidadDeCuotas.HasValue && prom.PorcentajeDeDescuento.HasValue)
                    if ((prom.MaximaCantidadDeCuotas.Value == 0 && prom.PorcentajeDeDescuento.Value == 0) || (prom.MaximaCantidadDeCuotas.Value > 0 && prom.PorcentajeDeDescuento.Value > 0))
                        context.AddFailure($"{nameof(prom.MaximaCantidadDeCuotas)} o {nameof(prom.PorcentajeDeDescuento)} debe tener un valor mayor a 0");
            });

            RuleFor(p => p).MustAsync(async (prom, context) =>
            {
                if (prom.FechaInicio.HasValue)
                {
                    var solapada = await _promocionRepository.GetPromocionSolapadaAsync(_mapper.Map<Promocion>(prom));
                    if (solapada != null)
                        return false;
                }
                return true;
            }).WithMessage(p => $"Ya existe una promoción activa que cumple con el rango de fechas indicado y contiene al menos un item repetido para {nameof(p.MediosDePago)}, {nameof(p.Bancos)} y/o {nameof(p.CategoriasProductos)}");
        }
    }
}
