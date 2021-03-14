using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Promociones.MappingConfigurations;
using Promociones.Model;
using Promociones.Services.Promociones;
using Promociones.Services.Promociones.Net;
using Promociones.Services.Repository.Net;
using Promociones.ViewModel;
using System;
using System.Linq;

namespace NUnitTest
{
    [TestFixture]
    public class MongoBookDBContextTests
    {
        private Mock<IOptions<BasePromociones>> _mockOptions;
        private Mock<IMongoDatabase> _mockDB;
        private Mock<IMongoClient> _mockClient;
        private IPromocionesService _promocionesService;

        public MongoBookDBContextTests()
        {
            _mockOptions = new Mock<IOptions<BasePromociones>>();
            _mockDB = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var settings = new BasePromociones()
            {
                StringConnexion = "mongodb://localhost:27017",
                NombreBase = "TestDB",
                NombreColeccion = "UnitTest"
            };

            _mockOptions.Setup(s => s.Value).Returns(settings);
            _mockClient.Setup(c => c
                .GetDatabase(_mockOptions.Object.Value.NombreBase, null))
                .Returns(_mockDB.Object);

            var Mapper = new MapperConfiguration(c => c.AddProfile<PromocionProfile>()).CreateMapper();

            _promocionesService = new PromocionesService(Mapper, new PromocionesRepository(_mockOptions.Object.Value));
        }

        [TearDown]
        public void TearDown()
        {
            var client = new MongoClient(_mockOptions.Object.Value.StringConnexion);
            client.DropDatabase(_mockOptions.Object.Value.NombreBase);
        }

        [Test]
        public void Ver_Listado_Promociones_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 30,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            _promocionesService.CrearPromocion(prom);

            var promociones = _promocionesService.ObtenerPromociones();
            Assert.AreEqual(1, promociones.Count);
        }

        [Test]
        public void Ver_Promocion_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 30,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            var promocion = _promocionesService.CrearPromocion(prom);
            var cargaPromocion = _promocionesService.ObtenerPromocion(promocion.Id);
            Assert.AreEqual(promocion.Id, cargaPromocion.Id);
        }

        [Test]
        public void Ver_Promociones_Vigentes_Success()
        {
            var proms = new PromocionPostViewModel[]
            {
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-4)
                }
            };

            foreach (var prom in proms)
                _promocionesService.CrearPromocion(prom);

            var promocionesVigentes = _promocionesService.ObtenerPromocionesVigentes();
            Assert.AreEqual(2, promocionesVigentes.Count);
        }

        [Test]
        public void Ver_Promociones_Vigentes_Fecha_Success()
        {
            var proms = new PromocionPostViewModel[]
            {
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-3)
                }
            };

            foreach (var prom in proms)
                _promocionesService.CrearPromocion(prom);

            var promocionesVigentes = _promocionesService.ObtenerPromocionesVigentes(DateTime.Now.Date.AddDays(-4));
            Assert.AreEqual(1, promocionesVigentes.Count);
        }

        [Test]
        public void Ver_Promociones_Vigentes_Venta_Success()
        {
            var proms = new PromocionPostViewModel[]
            {
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-3)
                }
            };

            foreach (var prom in proms)
                _promocionesService.CrearPromocion(prom);

            var promocionesVigentesVenta = _promocionesService.ObtenerPromocionesVigentesVenta(new FiltroVentaViewModel { Banco = "ICBC" });
            Assert.AreEqual(1, promocionesVigentesVenta.Count);
        }

        [Test]
        public void Crear_Promocion_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            var promocion = _promocionesService.CrearPromocion(prom);

            Assert.NotNull(promocion.Id);
            Assert.AreNotEqual(Guid.Empty, promocion.Id);
        }

        [Test]
        public void Modificar_Promocion_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            var promocion = _promocionesService.CrearPromocion(prom);

            prom.MediosDePago = new string[] { "GIFT_CARD" };

            var promocionModificado = _promocionesService.ActualizarPromocion(promocion.Id, prom);

            Assert.AreEqual(promocion.Id, promocionModificado.Id);
            Assert.AreEqual(prom.MediosDePago, promocionModificado.MediosDePago);
            Assert.IsNotEmpty(promocionModificado.MediosDePago);
            Assert.NotNull(promocionModificado.FechaModificacion);
            Assert.AreEqual("GIFT_CARD", promocionModificado.MediosDePago.FirstOrDefault());
        }

        [Test]
        public void Eliminar_Promocion_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            var promocion = _promocionesService.CrearPromocion(prom);

            var promocionEliminado = _promocionesService.EliminarPromocion(promocion.Id);

            Assert.AreEqual(promocion.Id, promocionEliminado.Id);
            Assert.IsFalse(promocionEliminado.Activo);
        }

        [Test]
        public void Modificar_Vigencia_Promocion_Success()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = DateTime.Now.Date.AddDays(-3),
                FechaFin = DateTime.Now.Date.AddDays(-2)
            };

            var promocion = _promocionesService.CrearPromocion(prom);

            var promocionesVigentes = _promocionesService.ObtenerPromocionesVigentes();
            Assert.AreEqual(0, promocionesVigentes.Count);

            _promocionesService.ActualizarVigenciaPromocion(promocion.Id, new VigenciaViewModel { FechaInicio = promocion.FechaInicio, FechaFin = DateTime.Now.Date.AddDays(2) });

            promocionesVigentes = _promocionesService.ObtenerPromocionesVigentes();
            Assert.AreEqual(1, promocionesVigentes.Count);
        }

        [Test]
        public void Crear_Promocion_Solapadas_Error()
        {
            var proms = new PromocionPostViewModel[]
            {
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-2),
                    FechaFin = DateTime.Now.Date.AddDays(2)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                }
            };

            int errors = 0;
            string errorField = "";

            foreach (var prom in proms)
            {
                try
                {
                    _promocionesService.CrearPromocion(prom);
                }
                catch (ValidationException ex)
                {
                    errors++;
                    errorField = ex.Errors.First().PropertyName;
                    continue;
                };
            }

            Assert.AreEqual(1, errors);
            Assert.AreEqual("Solapamiento", errorField);
        }

        [Test]
        public void Crear_Promocion_Sin_Cuotas_Ni_Descuento_Error()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "EFECTIVO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                FechaInicio = DateTime.Now.Date.AddDays(-1),
                FechaFin = DateTime.Now.Date.AddDays(1)
            };

            string errorField = "";

            try
            {
                _promocionesService.CrearPromocion(prom);
            }
            catch (ValidationException ex)
            {
                errorField = ex.Errors.First().PropertyName;
            };

            Assert.IsNotEmpty(errorField);
        }

        [Test]
        public void Crear_Promocion_Con_Porcentaje_Interes_Sin_Cuotas_Error()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "EFECTIVO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 35,
                ValorInteresCuotas = 5,
                FechaInicio = DateTime.Now.Date.AddDays(-1),
                FechaFin = DateTime.Now.Date.AddDays(1)
            };

            string errorField = "";

            try
            {
                _promocionesService.CrearPromocion(prom);
            }
            catch (ValidationException ex)
            {
                errorField = ex.Errors.First().PropertyName;
            };

            Assert.AreEqual(nameof(prom.ValorInteresCuotas), errorField);
        }

        [Test]
        public void Crear_Promocion_Con_Descuento_Fuera_Rango_Error()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "EFECTIVO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 2,
                FechaInicio = DateTime.Now.Date.AddDays(-1),
                FechaFin = DateTime.Now.Date.AddDays(1)
            };

            string errorField = "";

            try
            {
                _promocionesService.CrearPromocion(prom);
            }
            catch (ValidationException ex)
            {
                errorField = ex.Errors.First().PropertyName;
            };

            Assert.AreEqual(nameof(prom.PorcentajeDeDescuento), errorField);
        }

        [Test]
        public void Crear_Promocion_Con_Fin_Menor_Inicio_Error()
        {
            var prom = new PromocionPostViewModel
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "EFECTIVO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 50,
                FechaInicio = DateTime.Now.Date.AddDays(-1),
                FechaFin = DateTime.Now.Date.AddDays(-2)
            };

            string errorField = "";

            try
            {
                _promocionesService.CrearPromocion(prom);
            }
            catch (ValidationException ex)
            {
                errorField = ex.Errors.First().PropertyName;
            };

            Assert.AreEqual(nameof(prom.FechaFin), errorField);
        }

        [Test]
        public void Modificar_Vigencia_Promocion_Error()
        {
            var proms = new PromocionPostViewModel[]
            {
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-2),
                    FechaFin = DateTime.Now.Date.AddDays(-1)
                },
                new PromocionPostViewModel()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date,
                    FechaFin = DateTime.Now.Date.AddDays(1)
                }
            };

            PromocionViewModel lastProm = null;
            string errorField = string.Empty;

            foreach (var prom in proms)
                lastProm = _promocionesService.CrearPromocion(prom);

            try
            {
                _promocionesService.ActualizarVigenciaPromocion(lastProm.Id, new VigenciaViewModel { FechaInicio = DateTime.Now.Date.AddDays(-1), FechaFin = lastProm.FechaFin });
            }
            catch (ValidationException ex)
            {
                errorField = ex.Errors.First().PropertyName;
            }

            Assert.AreEqual("Solapamiento", errorField);
        }
    }
}