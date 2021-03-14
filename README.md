# Contenedor
Correr los siguientes comandos para levantar el contenedor

    cd src\Promociones
    docker-compose build
    docker-compose up -d
    
Se accede a la api a partir de https://localhost:44348/api

# Consigna
Se requiere el desarrollo de un microservicio que gestione, almacene y proporcione las promociones de la venta de productos de toda la compañía. El servicio será consumido por la venta en sucursal y online.

Se deben proveer las siguientes funcionalidades a través de una API Rest:

 - Ver listado de promociones
 - Ver una promoción
	 - Parámetros de entrada:
		 - Id: Guid
 - Ver listado de promociones vigentes
 - Ver listado de promociones vigentes para una fecha x
	 - Parámetros de entrada:
		 - fecha
 - Ver listado de promociones vigentes para una venta
	 - Parámetros de entrada:
		 - Medio de pago: string
		 - Banco: string
		 - Categoría producto: IEnumerable<string>
	 - Respuesta:
		 - IEnumerable<>
			 - Id: Guid
			 - Medio de pago: string
			 - Banco: string
			 - Categoría producto: string
			 - Cantidad de cuotas: int?
			 - Porcentaje interés cuota: decimal?
			 - Porcentaje de descuento: decimal?
 - Crear una promoción
	 - Parámetros de entrada:
		 - Medio de pago: IEnumerable<string>
		 - Banco: IEnumerable<string>
		 - Categoria producto: IEnumerable<string>
		 - Cantidad de cuotas: int?
		 - Porcentaje interés cuota: decimal?
		 - Porcentaje de descuento: decimal?
		 - Fecha inicio: DateTime?
		 - Fecha fin: DateTime?
	 - Validaciones:
		 - No se deben solapar promociones para al menos un medio de pago, banco o categoría
		 - Cantidad de cuotas y porcentaje de descuento son nullables pero al menos una debe tener valor
		 - Porcentaje interés cuota solo puede tener valor si cantidad de cuotas tiene valor
		 - Porcentaje descuento en caso de tener valor, debe estar comprendido entre 5 y 80
		 - Fecha fin no puede ser menor que fecha inicio
	 - Respuesta:
		 - En caso de ok:
			 - Id: Guid
 - Modificar una promoción
	 - Idem creación, se agrega Id como parámetro de entrada
 - Eliminar una promoción
	 - Parámetros de entrada:
		 - Id: Guid
	 - Se debe usar borrado lógico
 - Modificar vigencia de promoción
	 - Parámetros de entrada:
		 - Id: Guid
		 - Fecha inicio: DateTime?
		 - Fecha fin: DateTime?
	 - Validaciones:
		 - Solapamiento ídem creación y modificación

## Entidad

    public class Promocion
    {
        public Guid Id { get; private set; }
        public IEnumerable<string> MediosDePago { get; private set; }
        public IEnumerable<string> Bancos { get; private set; }
        public IEnumerable<string> CategoriasProductos { get; private set; }
        public int? MaximaCantidadDeCuotas { get; private set; }
        public decimal? ValorInteresCuotas { get; private set; }
        public decimal? PorcentajeDeDescuento { get; private set; }
        public DateTime? FechaInicio { get; private set; }
        public DateTime? FechaFin { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaModificacion { get; private set; }
    }

## Consideraciones funcionales:

 - La promoción se crea para cero o muchos de medios de pago
 - La promoción se crea para cero o muchos bancos
 - La promoción se crea para cero o muchas categorías de productos
 - La promoción puede tener porcentaje de descuento o cuotas. NO ambas
 - Se deben contemplar validaciones de solapamiento y duplicidad

## Valores válidos:

 Medios de pago:
	- TARJETA_CREDITO
	- TARJETA_DEBITO
	- EFECTIVO
	- GIFT_CARD
- Bancos:
	- Galicia
	- Santander Rio
	- Ciudad
	- Nacion
	- ICBC
	- BBVA
	- Macro
- Categorías productos:
	- Hogar
	- Jardin
	- ElectroCocina
	- GrandesElectro
	- Colchones
	- Celulares
	- Tecnologia
	- Audio

## Ejemplos de promociones:

    //12 cuotas sin interes para todos los medios de pago, todos los bancos y todos las categorias d e productos 
    { 
	    "Id": "426cc3be-cd5c-4001-bd46-7566a18f2376",
	    "MediosDePago": [],
	    "Bancos": [],
	    "CategoriasProductos": [],
	    "MaximaCantidadDeCuotas": 12,
	    "ValorInteresCuotas": 0,
	    "PorcentajeDeDescuento": null,
	    "FechaInicio": “01/06/2018”,
	    "FechaFin": “01/06/2030”
    }
 .

    //%10 de descuento en efectivo para colchones
    {
        "Id": "426cc3be-cd5c-4001-bd46-7566a18f2376"
        "MediosDePago": ["EFECTIVO"],
        "Bancos": [],
        "CategoriasProductos": ["Colchones"],
        "MaximaCantidadDeCuotas": null,
        "ValorInteresCuotas": null,
        "PorcentajeDeDescuento": 10,
        "FechaInicio": “01/06/2018”,
        "FechaFin": “01/06/2030”,
    }
.

    //Hasta 12 cuotas con interés con tarjetas de crédito y débito de los bancos Galicia y Macro en celulares y audio
    {
        "Id": "426cc3be-cd5c-4001-bd46-7566a18f2376"
        "MediosDePago": ["TARJETA_CREDITO", "TARJETA_DEBITO"],
        "Bancos": ["Galicia", "Macro"],
        "CategoriasProductos": ["Celulares", "Audio"],
        "MaximaCantidadDeCuotas": 12,
        "ValorInteresCuotas": 3.15,
        "PorcentajeDeDescuento": null,
        "FechaInicio": “01/06/2018”,
        "FechaFin": “01/06/2030”,
    }

## Requisitos técnicos:

 - C# (.net core >3.0)/Golang
 - DB: MongoDb
 - Utilizar Clean-Arquitecture
 - Unit tests
 - El servicio se debe poder correr localmente utilizando docker. Incluir readme con instrucciones.
 - Incluir collection de postman con ejemplos de utilización de cada endpoint
