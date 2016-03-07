<a name="inicio"></a>		
sdk-net	
=======		
		
Modulo para conexión con gateway de pago Todo Pago		

######[Instalación](#instalacion)		
######[Versiones de .net soportadas](#Versionesdenetoportadas)
######[Generalidades](#general)	
######[Uso](#uso)		
######[Datos adicionales para prevencion de fraude] (#datosadicionales) 		
######[Ejemplo](#ejemplo)		
######[Modo test](#test)
######[Status de la operación](#status)
######[Consulta de operaciones por rango de tiempo](#statusdate)
######[Devoluciónes](#devolucion)
######[Tablas de referencia](#tablas)		

<a name="instalacion"></a>		
## Instalación		
Se debe descargar la última versión del SDK desde el botón Download ZIP, branch master.		
Una vez descargado y descomprimido, se debe agregar la libreria TodoPagoConnector.dll que se encuntra dentro de la carpeta dist, a las librerias del proyecto y en el codigo se debe agregar siguiente using.
```C#
using TodoPagoConnector;
```

<br />		
[<sub>Volver a inicio</sub>](#inicio)		

<a name="Versionesdenetsoportadas"></a>		
## Versiones de .net soportadas		
La versi&oacute;n implementada de la SDK, esta testeada para versiones desde .net 3.5
<br />		
[<sub>Volver a inicio</sub>](#inicio)		

<a name="general"></a>
## Generalidades
Esta versión soporta únicamente pago en moneda nacional argentina (CURRENCYCODE = 32).
[<sub>Volver a inicio</sub>](#inicio)		


<a name="uso"></a>		
## Uso		
####1.Inicializar la clase correspondiente al conector (TodoPago).
- crear un String con el Endpoint suministrados por Todo Pago

```C#
String endpoint = "https://developers.todopago.com.ar/";
```
- crear un Dictionary<String, String> con los http header suministrados por Todo Pago
```C#
var headers = new Dictionary<String, String>();
headers.Add("Authorization", "PRISMA 912EC803B2CE49E4A541068D495AB570");
```
- crear una instancia de la clase TodoPago
```C#		
TPConnector tpc = new TPConnector(endpoint, headers);	
```		
		
####2.Solicitud de autorización		
En este caso hay que llamar a sendAuthorizeRequest(). Este metodo devuelve Dictionary<string, object>
```C#		
var res = connector.SendAuthorizeRequest(request, payload);		
```		
<ins><strong>datos propios del comercio</strong></ins>		

request y payload deben ser un Dictionary<string, string> con la siguiente estructura:		
		
```C#
var request = new Dictionary<string, string>();
request.Add(SECURITY, "f3d8b72c94ab4a06be2ef7c95490f7d3");
request.Add(SESSION, "ABCDEF-1234-12221-FDE1-00000200");
request.Add(MERCHANT, "2153");
request.Add(URL_OK, "http://someurl.com/ok");
request.Add(URL_ERROR, "http://someurl.com/fail");
request.Add(ENCODING_METHOD, "XML");

var payload = new Dictionary<string, string>();
payload.Add("MERCHANT", "2153");
payload.Add("OPERATIONID", "8000");
payload.Add("CURRENCYCODE", "032");
payload.Add("AMOUNT", "1.00");
payload.Add("EMAILCLIENTE", "some@someurl.com");
		
```		
		
####3.Confirmación de transacción.		
En este caso hay que llamar a getAuthorizeAnswer(), que retorna Dictionary<string, object>, enviando como parámetro un Dictionary<String, String> como se describe a continuación.		
```C#		
var request = new Dictionary<String, String>();
request.Add(SECURITY, "f3d8b72c94ab4a06be2ef7c95490f7d3");
request.Add(SESSION, null);
request.Add(MERCHANT, "2153");
request.Add(REQUESTKEY, "710268a7-7688-c8bf-68c9-430107e6b9da");
request.Add(ANSWERKEY, "693ca9cc-c940-06a4-8d96-1ab0d66f3ee6");

var res = connector.GetAuthorizeAnswer(request);
```		
<strong><ins>*Importante:</ins></strong>El campo AnswerKey se adiciona  en la redireccion que se realiza a alguna de las direcciones ( URL ) epecificadas en el  servicio SendAurhorizationRequest, esto sucede cuando la transaccion ya fue resuelta y es necesario regresar al Site para finalizar la transaccion de pago, tambien se adiciona el campo Order, el cual tendra el contenido enviado en el campo OPERATIONID. para nuestro ejemplo: <strong>http://susitio.com/paydtodopago/ok?Order=27398173292187&Answer=1111-2222-3333-4444-5555-6666-7777</strong>		
		
Este método devuelve el resumen de los datos de la transacción.		
<br />		
		
[<sub>Volver a inicio</sub>](#inicio)		
		
<a name="datosadicionales"></a>		
## Datos adicionales para control de fraude		

<a name="generales"></a>		
##### Parámetros Adicionales en el post inicial comunes a todos los rubros:		
```C#		
//Example
var parameters = new Dictionary<string, string>();		
request.Add("CSBTCITY", "Villa General Belgrano"); //Ciudad de facturación, MANDATORIO.		
parameters.Add("CSBTCOUNTRY", "AR");//País de facturación. MANDATORIO. Código ISO.
parameters.Add("CSBTCUSTOMERID", "453458"); //Identificador del usuario al que se le emite la factura. MANDATORIO. No 
			//puede contener un correo electrónico.		
parameters.Add(CSBTIPADDRESS", "192.0.0.4"); //IP de la PC del comprador. MANDATORIO.		
parameters.Add(CSBTEMAIL", "some@someurl.com"); //Mail del usuario al que se le emite la factura. MANDATORIO.
parameters.Add(CSBTFIRSTNAME", "Juan");//Nombre del usuario al que se le emite la factura. MANDATORIO.		
parameters.Add(CSBTLASTNAME", "Perez");//Apellido del usuario al que se le emite la factura. MANDATORIO.
parameters.Add(CSBTPHONENUMBER", "541160913988");//Teléfono del usuario al que se le emite la factura. No utilizar 
			//guiones, puntos o espacios. Incluir código de país. MANDATORIO.		
parameters.Add(CSBTPOSTALCODE", " 1010");//Código Postal de la dirección de facturación. MANDATORIO.
parameters.Add(CSBTSTATE", "B");//Provincia de la dirección de facturación. MANDATORIO. Ver tabla anexa de provincias.
parameters.Add(CSBTSTREET1", "Some Street 2153");//Domicilio de facturación (calle y nro). MANDATORIO.		
parameters.Add(CSBTSTREET2", "");//Complemento del domicilio. (piso, departamento). NO MANDATORIO.
parameters.Add(CSPTCURRENCY", "ARS");//Moneda. MANDATORIO.		
parameters.Add(CSPTGRANDTOTALAMOUNT", "1.00");//Con decimales opcional usando el puntos como separador de decimales.
			//No se permiten comas, ni como separador de miles ni como separador de decimales. MANDATORIO.
			//(Ejemplos:$125,38-> 125.38 $12-> 12 o 12.00)		
parameters.Add(CSMDD7", "");// Fecha registro comprador(num Dias). NO MANDATORIO.		
parameters.Add(CSMDD8", ""); //Usuario Guest? (Y/N). En caso de ser Y, el campo CSMDD9 no deberá enviarse. NO 
			//MANDATORIO.		
parameters.Add(CSMDD9", "");//Customer password Hash: criptograma asociado al password del comprador final. NO 	
			//MANDATORIO.		
parameters.Add(CSMDD10", "");//Histórica de compras del comprador (Num transacciones). NO MANDATORIO.		
parameters.Add(CSMDD11", "");//Customer Cell Phone. NO MANDATORIO.		
```		
		
<a name="retail"></a>		
##### Parámetros Adicionales en el post inicial para el rubro RETAIL		
```C#	
//Example
var parameters = new Dictionary<string, string>();		
parameters.Add("CSSTCITY", "Villa General Belgrano");//Ciudad de enví­o de la orden. MANDATORIO.		
parameters.Add("CSSTCOUNTRY", "AR");//País de envío de la orden. MANDATORIO.		
parameters.Add("CSSTEMAIL", "some@someurl.com");//Mail del destinatario, MANDATORIO.		
parameters.Add("CSSTFIRSTNAME", "Juan");//Nombre del destinatario. MANDATORIO.		
parameters.Add("CSSTLASTNAME", "Perez");//Apellido del destinatario. MANDATORIO.		
parameters.Add("CSSTPHONENUMBER", "541160913988");//Número de teléfono del destinatario. MANDATORIO.		
parameters.Add("CSSTPOSTALCODE", "1010");//Código postal del domicilio de envío. MANDATORIO.		
parameters.Add("CSSTSTATE", "B");//Provincia de envío. MANDATORIO. Son de 1 caracter		
parameters.Add("CSSTSTREET1", "Some Street 2153");//Domicilio de envío. MANDATORIO.		
parameters.Add("CSMDD12", "");//Shipping DeadLine (Num Dias). NO MADATORIO.		
parameters.Add("CSMDD13", "");//Método de Despacho. NO MANDATORIO.		
parameters.Add("CSMDD14", "");//Customer requires Tax Bill ? (Y/N). NO MANDATORIO.		
parameters.Add("CSMDD15", "");//Customer Loyality Number. NO MANDATORIO. 		
parameters.Add("CSMDD16", "");//Promotional / Coupon Code. NO MANDATORIO. 		
	//Retail: datos a enviar por cada producto, los valores deben estar separado con #:		
parameters.Add("CSITPRODUCTCODE", "electronic_good");//Código de producto. CONDICIONAL. Valores posibles(adult_content;coupon;default;electronic_good;electronic_software;gift_certificate;handling_only;service;shipping_and_handling;shipping_only;subscription)		
parameters.Add("CSITPRODUCTDESCRIPTION", "Test Prd Description");//Descripción del producto. CONDICIONAL.		
parameters.Add("CSITPRODUCTNAME", "TestPrd");//Nombre del producto. CONDICIONAL.	
parameters.Add("CSITPRODUCTSKU", "SKU1234");//Código identificador del producto. CONDICIONAL.		
parameters.Add("CSITTOTALAMOUNT", "10.01");//CSITTOTALAMOUNT=CSITUNITPRICE*CSITQUANTITY "999999[.CC]" Con decimales opcional usando el puntos como separador de decimales. No se permiten comas, ni como separador de miles ni como separador de decimales. CONDICIONAL.		
parameters.Add("CSITQUANTITY", "1");//Cantidad del producto. CONDICIONAL.		
parameters.Add("CSITUNITPRICE", "10.01");//Formato Idem CSITTOTALAMOUNT. CONDICIONAL.	
```		

[<sub>Volver a inicio</sub>](#inicio)		
		
<a name="ejemplo"></a>		
## Ejemplo		
Existe un ejemplo en la carpeta https://github.com/TodoPago/SDK-.NET/tree/master/Ejemplo que muestra los resultados de los métodos principales del SDK.		
		
[<sub>Volver a inicio</sub>](#inicio)

<a name="status"></a>
## Status de la Operación
La SDK cuenta con un m&eacute;todo para consultar el status de la transacci&oacute;n desde la misma SDK. El m&eacute;todo se utiliza de la siguiente manera:
```C#
TPConnector tpc = new TPConnector(endpoint, headers);
String merchant = "2153";
String operationID = "02";
var res = connector.GetStatus(merchant, operationID);// Merchant es el id site y operationID es el id operación que se envio en el array a través del método sendAuthorizeRequest() 
```
El siguiente m&eacute;todo retornara el status actual de la transacci&oacute;n en Todopago, y devuelve List<Dictionary<string, object>>.
[<sub>Volver a inicio</sub>](#inicio)		

<a name="statusdate"></a>
## Consulta de operaciones por rango de tiempo
En este caso hay que llamar a getByRangeDateTime() y devolvera todas las operaciones realizadas en el rango de fechas dado

```C#
TPConnector tpc = new TPConnector(endpoint, headers);

Dictionary<string, string> gbrdt = new Dictionary<string, string>();
gbrdt.Add(TPConnector.MERCHANT, "2153");
gbrdt.Add(TPConnector.STARTDATE, "2015-01-01");
gbrdt.Add(TPConnector.ENDDATE, "2015-12-10");
gbrdt.Add(TPConnector.PAGENUMBER, "1");

Dictionary<string, Object> res = connector.getByRangeDateTime(gbrdt);
```

[<sub>Volver a inicio</sub>](#inicio)	

<a name="devolucion"></a>
## Devoluciónes
La SDK dispone de dos m&eacute;todos para realizar la Devolución online, total o parcial, de una transacci&oacute;n realizada a traves de TodoPago. El m&eacute;todo se utiliza de la siguiente manera:

Devolución Total
```C#
TPConnector tpc = new TPConnector(endpoint, headers);

Dictionary<string, string> gbrdt = new Dictionary<string, string>();
gbrdt.Add(TPConnector.MERCHANT, "2153");
gbrdt.Add(TPConnector.SECURITY, "f3d8b72c94ab4a06be2ef7c95490f7d3");
gbrdt.Add(TPConnector.REQUESTKEY, "bb25d589-52bc-8e21-fc5d-47d677b0995c");

string res = connector.VoidRequest(gbrdt);// Merchant es el id site y requestKey es la key se que retorna a travÃ©s del mÃ©todo SendAuthorizeRequest() 
```
Devolución Parcial
```C#
TPConnector tpc = new TPConnector(endpoint, headers);

Dictionary<string, string> gbrdt = new Dictionary<string, string>();
gbrdt.Add(TPConnector.MERCHANT, "2153");
gbrdt.Add(TPConnector.SECURITY, "f3d8b72c94ab4a06be2ef7c95490f7d3");
gbrdt.Add(TPConnector.REQUESTKEY, "0db2e848-b0ab-6baf-f9e1-b70a82ed5844");
gbrdt.Add(TPConnector.AMOUNT, "10");

string res = connector.ReturnRequest(gbrdt);// Merchant es el id site , AuthorizationKey es la key se que retorna a travÃ©s del mÃ©todo SendAuthorizeRequest() y Amount la cantidad a devolver (float Type)
```

Si la operación fue realizada correctamente se informará con un código 2011 y un mensaje indicando el éxito de la operación.

[<sub>Volver a inicio</sub>](#inicio)	

<a name="tablas"></a>		
## Tablas de Referencia		
######[Códigos de Estado](#cde)		
######[Provincias](#p)		
<a name="cde"></a>		
<p>Codigos de Estado</p>		
<table>		
<tr><th>IdEstado</th><th>Descripción</th></tr>		
<tr><td>1</td><td>Ingresada</td></tr>		
<tr><td>2</td><td>A procesar</td></tr>		
<tr><td>3</td><td>Procesada</td></tr>		
<tr><td>4</td><td>Autorizada</td></tr>		
<tr><td>5</td><td>Rechazada</td></tr>		
<tr><td>6</td><td>Acreditada</td></tr>		
<tr><td>7</td><td>Anulada</td></tr>		
<tr><td>8</td><td>Anulación Confirmada</td></tr>		
<tr><td>9</td><td>Devuelta</td></tr>		
<tr><td>10</td><td>Devolución Confirmada</td></tr>		
<tr><td>11</td><td>Pre autorizada</td></tr>		
<tr><td>12</td><td>Vencida</td></tr>		
<tr><td>13</td><td>Acreditación no cerrada</td></tr>		
<tr><td>14</td><td>Autorizada *</td></tr>		
<tr><td>15</td><td>A reversar</td></tr>		
<tr><td>16</td><td>A registar en Visa</td></tr>		
<tr><td>17</td><td>Validación iniciada en Visa</td></tr>		
<tr><td>18</td><td>Enviada a validar en Visa</td></tr>		
<tr><td>19</td><td>Validada OK en Visa</td></tr>		
<tr><td>20</td><td>Recibido desde Visa</td></tr>		
<tr><td>21</td><td>Validada no OK en Visa</td></tr>		
<tr><td>22</td><td>Factura generada</td></tr>		
<tr><td>23</td><td>Factura no generada</td></tr>		
<tr><td>24</td><td>Rechazada no autenticada</td></tr>		
<tr><td>25</td><td>Rechazada datos inválidos</td></tr>		
<tr><td>28</td><td>A registrar en IdValidador</td></tr>		
<tr><td>29</td><td>Enviada a IdValidador</td></tr>		
<tr><td>32</td><td>Rechazada no validada</td></tr>		
<tr><td>38</td><td>Timeout de compra</td></tr>		
<tr><td>50</td><td>Ingresada Distribuida</td></tr>		
<tr><td>51</td><td>Rechazada por grupo</td></tr>		
<tr><td>52</td><td>Anulada por grupo</td></tr>		
</table>		
		
<a name="p"></a>		
<p>Provincias</p>
<p>Solo utilizado para incluir los datos de control de fraude</p>
<table>		
<tr><th>Provincia</th><th>Código</th></tr>		
<tr><td>CABA</td><td>C</td></tr>		
<tr><td>Buenos Aires</td><td>B</td></tr>		
<tr><td>Catamarca</td><td>K</td></tr>		
<tr><td>Chaco</td><td>H</td></tr>		
<tr><td>Chubut</td><td>U</td></tr>		
<tr><td>Córdoba</td><td>X</td></tr>		
<tr><td>Corrientes</td><td>W</td></tr>		
<tr><td>Entre Ríos</td><td>E</td></tr>		
<tr><td>Formosa</td><td>P</td></tr>		
<tr><td>Jujuy</td><td>Y</td></tr>		
<tr><td>La Pampa</td><td>L</td></tr>		
<tr><td>La Rioja</td><td>F</td></tr>		
<tr><td>Mendoza</td><td>M</td></tr>		
<tr><td>Misiones</td><td>N</td></tr>		
<tr><td>Neuquén</td><td>Q</td></tr>		
<tr><td>Río Negro</td><td>R</td></tr>		
<tr><td>Salta</td><td>A</td></tr>		
<tr><td>San Juan</td><td>J</td></tr>		
<tr><td>San Luis</td><td>D</td></tr>		
<tr><td>Santa Cruz</td><td>Z</td></tr>		
<tr><td>Santa Fe</td><td>S</td></tr>		
<tr><td>Santiago del Estero</td><td>G</td></tr>		
<tr><td>Tierra del Fuego</td><td>V</td></tr>		
<tr><td>Tucumán</td><td>T</td></tr>		
</table>		
[<sub>Volver a inicio</sub>](#inicio)



