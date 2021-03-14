using FluentValidation;
using MongoDB.Driver;
using Promociones.Model;
using Promociones.Services.Repository;
using System;
using System.Linq;

namespace Promociones.Validators
{
    public class PromocionValidator : AbstractValidator<Promocion>
    {
        public PromocionValidator(IPromocionesRepository promocionesRepository)
        {
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

            RuleFor(p => p.MaximaCantidadDeCuotas)
                .NotNull()
                .When(p => !p.PorcentajeDeDescuento.HasValue)
                .WithMessage(p => $"Solo {nameof(p.MaximaCantidadDeCuotas)} o {nameof(p.PorcentajeDeDescuento)} puede tener un valor.");

            RuleFor(p => p.PorcentajeDeDescuento)
                .NotNull()
                .When(p => !p.MaximaCantidadDeCuotas.HasValue)
                .WithMessage(p => $"Solo {nameof(p.MaximaCantidadDeCuotas)} o {nameof(p.PorcentajeDeDescuento)} puede tener un valor mayor a 0");

            RuleFor(p => p.ValorInteresCuotas)
                .Null()
                .When(p => !p.MaximaCantidadDeCuotas.HasValue)
                .WithMessage(p => $"{nameof(p.ValorInteresCuotas)} solo puede tener valor si {nameof(p.MaximaCantidadDeCuotas)} tiene valor.");

            RuleFor(p => p.PorcentajeDeDescuento)
                .InclusiveBetween(5, 80)
                .When(p => p.PorcentajeDeDescuento.HasValue)
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
                if (prom.FechaInicio.HasValue && promocionesRepository.Query()
                    .Where(p => p.Id != prom.Id)
                    .Where(p =>
                        (!p.FechaFin.HasValue || (prom.FechaInicio ?? DateTime.MinValue) <= p.FechaFin) && (p.FechaInicio <= (prom.FechaFin ?? DateTime.MaxValue)) && (
                        prom.MediosDePago.Where(mp => p.MediosDePago.Contains(mp)).Any() ||
                        prom.Bancos.Where(b => p.Bancos.Contains(b)).Any() ||
                        prom.CategoriasProductos.Where(c => p.CategoriasProductos.Contains(c)).Any())
                    )
                    .Any())
                    context.AddFailure("Solapamiento", $"Ya existe una promoción activa que cumple con el rango de fechas indicado y contiene al menos un item repetido para {nameof(prom.MediosDePago)}, {nameof(prom.Bancos)} y/o {nameof(prom.CategoriasProductos)}");
            });
        }
    }
}
