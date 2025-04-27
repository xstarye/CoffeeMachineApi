using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

using var server = WireMockServer.Start();

server.Given(Request.Create().WithPath("/data/2.5/weather").UsingGet().WithParam("q", "Hamilton").WithParam("appid", "api_key").WithParam("units", "metric")).RespondWith(Response.Create().WithBody("{\"Main\":{\"Temp\":32.5}}").WithStatusCode(200));
server.Given(Request.Create().WithPath("/data/2.5/weather").UsingGet().WithParam("q", "Auckland").WithParam("appid", "api_key").WithParam("units", "metric")).RespondWith(Response.Create().WithBody("{\"Main\":{\"Temp\":23.5}}").WithStatusCode(200));
Console.WriteLine(server.Url);
Console.ReadLine();

