using Application;
using Application.Features.Promociones.Commands.CreatePromocion;
using Application.Features.Promociones.Commands.DeletePromocionById;
using Application.Features.Promociones.Commands.UpdatePromocion;
using Application.Features.Promociones.Commands.UpdateVigenciaPromocion;
using Application.Features.Promociones.Queries.GetAllPromociones;
using Application.Features.Promociones.Queries.GetAllPromocionesVigentes;
using Application.Features.Promociones.Queries.GetPromocionById;
using Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta;
using Application.Features.Promociones.Queries.GetPromocionesVigentesPorFecha;
using Application.Interfaces.Repositories;
using Application.Mappings;
using AutoMapper;
using Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTest
{
    [TestFixture]
    public class MongoBookDBContextTests
    {
        private Mock<IOptions<BasePromociones>> _mockOptions;
        private Mock<IMongoDatabase> _mockDB;
        private Mock<IMongoClient> _mockClient;
        private IMapper _mapper;
        private IPromocionRepositoryAsync _promocionRepositoryAsync;

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

            _mapper = new MapperConfiguration(c => c.AddProfile<GeneralProfile>()).CreateMapper();
            _promocionRepositoryAsync = new PromocionRepositoryAsync(_mockOptions.Object);
        }

        [TearDown]
        public void TearDown()
        {
            var client = new MongoClient(_mockOptions.Object.Value.StringConnexion);
            client.DropDatabase(_mockOptions.Object.Value.NombreBase);
        }

        [Test]
        public async Task Ver_Listado_Promociones_Success()
        {
            CreatePromocionCommand createCommand = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 30,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };
            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreada = await createHandler.Handle(createCommand, default(CancellationToken));

            GetAllPromocionesQuery getAllCommand = new GetAllPromocionesQuery();
            GetAllPromocionesQueryHandler getAllHandler = new GetAllPromocionesQueryHandler(_promocionRepositoryAsync);

            var promociones = await getAllHandler.Handle(getAllCommand, default(CancellationToken));

            Assert.AreEqual(1, promociones.Data.Count());
            Assert.AreEqual(promocionCreada.Data, promociones.Data.First().Id);
        }

        [Test]
        public async Task Ver_Promocion_Success()
        {
            CreatePromocionCommand createCommand = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                PorcentajeDeDescuento = 30,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };
            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreada = await createHandler.Handle(createCommand, default(CancellationToken));

            GetPromocionByIdQuery getPromocionByIdQuery = new GetPromocionByIdQuery { Id = promocionCreada.Data };
            GetProductByIdQueryHandler getProductByIdQueryHandler = new GetProductByIdQueryHandler(_promocionRepositoryAsync);

            var promocionById = await getProductByIdQueryHandler.Handle(getPromocionByIdQuery, default(CancellationToken));

            Assert.AreEqual(promocionCreada.Data, promocionById.Data.Id);
        }

        [Test]
        public async Task Ver_Promociones_Vigentes_Success()
        {
            var createCommands = new CreatePromocionCommand[]
            {
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-4)
                }
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);

            foreach (var cmd in createCommands)
                await createHandler.Handle(cmd, default(CancellationToken));

            GetPromocionesVigentesQuery getPromocionesVigentesQuery = new GetPromocionesVigentesQuery();
            GetPromocionesVigentesHandler getPromocionesVigentesHandler = new GetPromocionesVigentesHandler(_promocionRepositoryAsync);

            var promocionesVigentes = await getPromocionesVigentesHandler.Handle(getPromocionesVigentesQuery, default(CancellationToken));
            Assert.AreEqual(2, promocionesVigentes.Data.Count());
        }

        [Test]
        public async Task Ver_Promociones_Vigentes_Fecha_Success()
        {
            var createCommands = new CreatePromocionCommand[]
            {
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-3)
                }
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);

            foreach (var cmd in createCommands)
                await createHandler.Handle(cmd, default(CancellationToken));

            GetPromocionesVigentesPorFechaQuery getPromocionesVigentesPorFechaQuery = new GetPromocionesVigentesPorFechaQuery { Date = DateTime.Now.Date.AddDays(-4) };
            GetPromocionesVigentesPorFechaHandler getPromocionesVigentesPorFechaHandler = new GetPromocionesVigentesPorFechaHandler(_promocionRepositoryAsync);

            var promocionesVigentes = await getPromocionesVigentesPorFechaHandler.Handle(getPromocionesVigentesPorFechaQuery, default(CancellationToken));
            Assert.AreEqual(1, promocionesVigentes.Data.Count());
        }

        [Test]
        public async Task Ver_Promociones_Vigentes_Venta_Success()
        {
            var createCommands = new CreatePromocionCommand[]
            {
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "Galicia" },
                    MediosDePago = new string[] { "TARJETA_CREDITO" },
                    CategoriasProductos = new string[] { "ElectroCocina" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 30,
                    FechaInicio = DateTime.Now.Date.AddDays(-1),
                    FechaFin = DateTime.Now.Date.AddDays(1)
                },
                new CreatePromocionCommand()
                {
                    Bancos = new string[] { "ICBC" },
                    MediosDePago = new string[] { "EFECTIVO" },
                    CategoriasProductos = new string[] { "Colchones" },
                    PorcentajeDeDescuento = 20,
                    FechaInicio = DateTime.Now.Date.AddDays(-5),
                    FechaFin = DateTime.Now.Date.AddDays(-3)
                }
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);

            foreach (var cmd in createCommands)
                await createHandler.Handle(cmd, default(CancellationToken));

            GetPromocionesVigentesParaVentaQuery getPromocionesVigentesParaVentaQuery = new GetPromocionesVigentesParaVentaQuery
            {
                Banco = "ICBC"
            };
            GetPromocionesVigentesParaVentaQueryHandler getPromocionesVigentesParaVentaQueryHandler = new GetPromocionesVigentesParaVentaQueryHandler(_promocionRepositoryAsync, _mapper);
            var promocionesVigentesVenta = await getPromocionesVigentesParaVentaQueryHandler.Handle(getPromocionesVigentesParaVentaQuery, default(CancellationToken));

            Assert.AreEqual(1, promocionesVigentesVenta.Data.Count());
        }

        [Test]
        public async Task Crear_Promocion_Success()
        {
            var command = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreada = await createHandler.Handle(command, default(CancellationToken));

            Assert.NotNull(promocionCreada.Data);
            Assert.AreNotEqual(Guid.Empty, promocionCreada.Data);
        }

        [Test]
        public async Task Modificar_Promocion_Success()
        {
            var command = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreada = await createHandler.Handle(command, default(CancellationToken));

            UpdatePromocionCommand updatePromocionCommand = new UpdatePromocionCommand
            {
                Id = promocionCreada.Data,
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "GIFT_CARD" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            UpdatePromocionCommandHandler updatePromocionCommandHandler = new UpdatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionModificado = await updatePromocionCommandHandler.Handle(updatePromocionCommand, default(CancellationToken));

            GetPromocionByIdQuery getPromocionByIdQuery = new GetPromocionByIdQuery { Id = promocionCreada.Data };
            GetProductByIdQueryHandler getProductByIdQueryHandler = new GetProductByIdQueryHandler(_promocionRepositoryAsync);
            var promocionById = await getProductByIdQueryHandler.Handle(getPromocionByIdQuery, default(CancellationToken));

            Assert.AreEqual(promocionCreada.Data, promocionModificado.Data);
            Assert.NotNull(promocionById.Data.FechaModificacion);
            Assert.AreEqual("GIFT_CARD", promocionById.Data.MediosDePago.FirstOrDefault());
        }

        [Test]
        public async Task Eliminar_Promocion_Success()
        {
            var command = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = new DateTime(2021, 3, 1),
                FechaFin = new DateTime(2021, 3, 31)
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreadaId = await createHandler.Handle(command, default(CancellationToken));

            DeletePromocionByIdCommand deletePromocionByIdCommand = new DeletePromocionByIdCommand { Id = promocionCreadaId.Data };
            DeleteProductByIdCommandHandler deleteProductByIdCommandHandler = new DeleteProductByIdCommandHandler(_promocionRepositoryAsync);
            var promocionEliminado = await deleteProductByIdCommandHandler.Handle(deletePromocionByIdCommand, default(CancellationToken));

            GetPromocionByIdQuery getPromocionByIdQuery = new GetPromocionByIdQuery { Id = promocionCreadaId.Data };
            GetProductByIdQueryHandler getProductByIdQueryHandler = new GetProductByIdQueryHandler(_promocionRepositoryAsync);
            var promocionById = await getProductByIdQueryHandler.Handle(getPromocionByIdQuery, default(CancellationToken));

            Assert.AreEqual(promocionCreadaId.Data, promocionEliminado.Data);
            Assert.IsFalse(promocionById.Data.Activo);
        }

        [Test]
        public async Task Modificar_Vigencia_Promocion_Success()
        {
            var command = new CreatePromocionCommand
            {
                Bancos = new string[] { "Galicia" },
                MediosDePago = new string[] { "TARJETA_CREDITO" },
                CategoriasProductos = new string[] { "ElectroCocina" },
                MaximaCantidadDeCuotas = 3,
                FechaInicio = DateTime.Now.Date.AddDays(-3),
                FechaFin = DateTime.Now.Date.AddDays(-2)
            };

            CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);
            var promocionCreadaId = await createHandler.Handle(command, default(CancellationToken));

            GetPromocionesVigentesQuery getPromocionesVigentesQuery = new GetPromocionesVigentesQuery();
            GetPromocionesVigentesHandler getPromocionesVigentesHandler = new GetPromocionesVigentesHandler(_promocionRepositoryAsync);
            var promocionesVigentes = await getPromocionesVigentesHandler.Handle(getPromocionesVigentesQuery, default(CancellationToken));
            Assert.AreEqual(0, promocionesVigentes.Data.Count());

            UpdateVigenciaPromocionCommand updateVigenciaPromocionCommand = new UpdateVigenciaPromocionCommand
            {
                Id = promocionCreadaId.Data,
                FechaInicio = command.FechaInicio,
                FechaFin = DateTime.Now.Date.AddDays(2)
            };
            UpdateVigenciaPromocionCommandHandler updateVigenciaPromocionCommandHandler = new UpdateVigenciaPromocionCommandHandler(_promocionRepositoryAsync);
            await updateVigenciaPromocionCommandHandler.Handle(updateVigenciaPromocionCommand, default(CancellationToken));

            promocionesVigentes = await getPromocionesVigentesHandler.Handle(getPromocionesVigentesQuery, default(CancellationToken));
            Assert.AreEqual(1, promocionesVigentes.Data.Count());
        }

        //[Test]
        //public async Task Crear_Promocion_Solapadas_Error()
        //{
        //    var services = new ServiceCollection();
        //    services.AddApplicationLayer();

        //    var proms = new CreatePromocionCommand[]
        //    {
        //        new CreatePromocionCommand()
        //        {
        //            Bancos = new string[] { "Galicia11" },
        //            MediosDePago = new string[] { "EFECTIVO11" },
        //            CategoriasProductos = new string[] { "ElectroCocina11" },
        //            PorcentajeDeDescuento = 30,
        //            FechaInicio = DateTime.Now.Date.AddDays(-2),
        //            FechaFin = DateTime.Now.Date.AddDays(2)
        //        }
        //        //new CreatePromocionCommand()
        //        //{
        //        //    Bancos = new string[] { "ICBC" },
        //        //    MediosDePago = new string[] { "EFECTIVO" },
        //        //    CategoriasProductos = new string[] { "Colchones" },
        //        //    PorcentajeDeDescuento = 30,
        //        //    FechaInicio = DateTime.Now.Date.AddDays(-1),
        //        //    FechaFin = DateTime.Now.Date.AddDays(1)
        //        //}
        //    };

        //    int errors = 0;
        //    string errorField = "";

        //    CreatePromocionCommandHandler createHandler = new CreatePromocionCommandHandler(_promocionRepositoryAsync, _mapper);

        //    foreach (var command in proms)
        //    {
        //        var promocionCreada = await createHandler.Handle(command, default(CancellationToken));
        //        if (!promocionCreada.Succeeded)
        //        {
        //            errors++;
        //            //errorField = ex.Errors.First().PropertyName;
        //        }
        //    }

        //    Assert.AreEqual(1, errors);
        //    //Assert.AreEqual("Solapamiento", errorField);
        //}

        //[Test]
        //public async Task Crear_Promocion_Sin_Cuotas_Ni_Descuento_Error()
        //{
        //    var prom = new PromocionPostViewModel
        //    {
        //        Bancos = new string[] { "Galicia" },
        //        MediosDePago = new string[] { "EFECTIVO" },
        //        CategoriasProductos = new string[] { "ElectroCocina" },
        //        FechaInicio = DateTime.Now.Date.AddDays(-1),
        //        FechaFin = DateTime.Now.Date.AddDays(1)
        //    };

        //    string errorField = "";

        //    try
        //    {
        //        _promocionesService.CrearPromocion(prom);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        errorField = ex.Errors.First().PropertyName;
        //    };

        //    Assert.IsNotEmpty(errorField);
        //}

        //[Test]
        //public async Task Crear_Promocion_Con_Porcentaje_Interes_Sin_Cuotas_Error()
        //{
        //    var prom = new PromocionPostViewModel
        //    {
        //        Bancos = new string[] { "Galicia" },
        //        MediosDePago = new string[] { "EFECTIVO" },
        //        CategoriasProductos = new string[] { "ElectroCocina" },
        //        PorcentajeDeDescuento = 35,
        //        ValorInteresCuotas = 5,
        //        FechaInicio = DateTime.Now.Date.AddDays(-1),
        //        FechaFin = DateTime.Now.Date.AddDays(1)
        //    };

        //    string errorField = "";

        //    try
        //    {
        //        _promocionesService.CrearPromocion(prom);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        errorField = ex.Errors.First().PropertyName;
        //    };

        //    Assert.AreEqual(nameof(prom.ValorInteresCuotas), errorField);
        //}

        //[Test]
        //public async Task Crear_Promocion_Con_Descuento_Fuera_Rango_Error()
        //{
        //    var prom = new PromocionPostViewModel
        //    {
        //        Bancos = new string[] { "Galicia" },
        //        MediosDePago = new string[] { "EFECTIVO" },
        //        CategoriasProductos = new string[] { "ElectroCocina" },
        //        PorcentajeDeDescuento = 2,
        //        FechaInicio = DateTime.Now.Date.AddDays(-1),
        //        FechaFin = DateTime.Now.Date.AddDays(1)
        //    };

        //    string errorField = "";

        //    try
        //    {
        //        _promocionesService.CrearPromocion(prom);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        errorField = ex.Errors.First().PropertyName;
        //    };

        //    Assert.AreEqual(nameof(prom.PorcentajeDeDescuento), errorField);
        //}

        //[Test]
        //public async Task Crear_Promocion_Con_Fin_Menor_Inicio_Error()
        //{
        //    var prom = new PromocionPostViewModel
        //    {
        //        Bancos = new string[] { "Galicia" },
        //        MediosDePago = new string[] { "EFECTIVO" },
        //        CategoriasProductos = new string[] { "ElectroCocina" },
        //        PorcentajeDeDescuento = 50,
        //        FechaInicio = DateTime.Now.Date.AddDays(-1),
        //        FechaFin = DateTime.Now.Date.AddDays(-2)
        //    };

        //    string errorField = "";

        //    try
        //    {
        //        _promocionesService.CrearPromocion(prom);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        errorField = ex.Errors.First().PropertyName;
        //    };

        //    Assert.AreEqual(nameof(prom.FechaFin), errorField);
        //}

        //[Test]
        //public async Task Modificar_Vigencia_Promocion_Error()
        //{
        //    var proms = new PromocionPostViewModel[]
        //    {
        //        new PromocionPostViewModel()
        //        {
        //            Bancos = new string[] { "Galicia" },
        //            MediosDePago = new string[] { "EFECTIVO" },
        //            CategoriasProductos = new string[] { "ElectroCocina" },
        //            PorcentajeDeDescuento = 30,
        //            FechaInicio = DateTime.Now.Date.AddDays(-2),
        //            FechaFin = DateTime.Now.Date.AddDays(-1)
        //        },
        //        new PromocionPostViewModel()
        //        {
        //            Bancos = new string[] { "ICBC" },
        //            MediosDePago = new string[] { "EFECTIVO" },
        //            CategoriasProductos = new string[] { "Colchones" },
        //            PorcentajeDeDescuento = 30,
        //            FechaInicio = DateTime.Now.Date,
        //            FechaFin = DateTime.Now.Date.AddDays(1)
        //        }
        //    };

        //    PromocionViewModel lastProm = null;
        //    string errorField = string.Empty;

        //    foreach (var prom in proms)
        //        lastProm = _promocionesService.CrearPromocion(prom);

        //    try
        //    {
        //        _promocionesService.ActualizarVigenciaPromocion(lastProm.Id, new VigenciaViewModel { FechaInicio = DateTime.Now.Date.AddDays(-1), FechaFin = lastProm.FechaFin });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        errorField = ex.Errors.First().PropertyName;
        //    }

        //    Assert.AreEqual("Solapamiento", errorField);
        //}
    }
}