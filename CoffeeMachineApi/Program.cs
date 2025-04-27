using Autofac.Extensions.DependencyInjection;
using Autofac;
using CoffeeMachineApi.Factory;
using CoffeeMachineApi.Service.CoffeeCounter;
using CoffeeMachineApi.Service.CountHandler;
using CoffeeMachineApi.Service.DateHandler;
using CoffeeMachineApi.Service.Datetime;
using CoffeeMachineApi.Utils;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using System.Net;
using System.Reflection;
using System.Net.NetworkInformation;
using CoffeeMachineApi.Models;
using Microsoft.EntityFrameworkCore;
using CoffeeMachineApi.Data;
using CoffeeMachineApi.Service.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
//builder.Services.AddDbContext<CoffeeDbContext>(options =>
    //options.UseInMemoryDatabase("MyDb"));
builder.Services.AddDbContext<CoffeeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddSingleton<IMyDataCacheService, MyDataCacheService>();
builder.Services.AddHostedService(sp => (CoffeeDateCache)sp.GetRequiredService<ICoffeeDateCache>());

/*
//ע�빤��ģʽ�ĵ�����������������
builder.Services.AddSingleton<IDatetime, TimePro>();
//ÿ�����󶼸���ʵ��
//builder.Services.AddTransient<IDatetime, TimePro>();
//builder.Services.AddSingleton<ICoffeeCounter, CoffeeCounter>();
builder.Services.AddHttpClient<IWeather, Weather>().AddPolicyHandler(GetRetryPolicy());
builder.Services.AddTransient<IDateHandler, Date4_1>();
builder.Services.AddTransient<IDateHandler, Date4_20>();
builder.Services.AddScoped<IDateHandlerFactory, DateHandlerFactory>();

builder.Services.AddTransient<ICountHandler, Count5>();
builder.Services.AddScoped<ICountHandlerFactory, CountHandlerFactory>();

builder.Services.AddTransient<ICoffeeCounter, CoffeeCounter>();
builder.Services.AddTransient<ICoffeeCounter, Counter4_19>();
builder.Services.AddScoped<ICoffeeCountFactory, CoffeeCountFactory>();
*/
builder.Services.AddHttpClient<IWeather, Weather>().AddPolicyHandler(GetRetryPolicy());
builder.Services.AddSingleton<CountState>();
// ������autofac����ע��
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{   // ע������ʵ���˽ӿڵĳ���
    string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    builder.RegisterAssemblyTypes(Assembly.Load(assemblyName))
    .AsImplementedInterfaces()
    .InstancePerDependency();
});

// ��ȡ���û�ֱ��д�����ַ���
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// ע�� IConnectionMultiplexer Ϊ����
builder.Services.AddSingleton<RedisLockProvider>(sp =>
{
    return new RedisLockProvider(ConnectionMultiplexer.Connect(redisConnectionString));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//adding the polly policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    //catch error and retry for 2 times
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode != HttpStatusCode.OK)
        .WaitAndRetryAsync(
            retryCount: 2,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Result?.StatusCode}");
            });
}

//for API integration test
public partial class Program { }
