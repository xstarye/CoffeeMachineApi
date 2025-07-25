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


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:7075";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

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
//注入工厂模式的单例，长期生命周期
builder.Services.AddSingleton<IDatetime, TimePro>();
//每次请求都更新实例
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
// 以下是autofac依赖注入
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{   // 注入所有实现了接口的程序集
    string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    builder.RegisterAssemblyTypes(Assembly.Load(assemblyName))
    .AsImplementedInterfaces()
    .InstancePerDependency();
});
/*如果实现了接口的同时又有继承抽象类，用下面这个
builder.RegisterAssemblyTypes(Assembly.Load(assemblyName))
    .AsImplementedInterfaces()      // 注册为接口
    .AsSelf()                       // 注册为自身类型
    .InstancePerDependency();
*/

// 读取配置或直接写连接字符串
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// 注册 IConnectionMultiplexer 为单例
builder.Services.AddSingleton<RedisLockProvider>(sp =>
{
    return new RedisLockProvider(ConnectionMultiplexer.Connect(redisConnectionString));
});

// 读取配置
var redisNodeList = builder.Configuration.GetSection("RedisCloud:Nodes").Get<List<string>>();

var redisMuxers = new List<IConnectionMultiplexer>();
foreach (var node in redisNodeList)
{
    var muxer = await ConnectionMultiplexer.ConnectAsync(node);
    redisMuxers.Add(muxer);
}

// 注册 RedlockDistributedLock 为单例服务（示例锁名和时间）
builder.Services.AddSingleton(new RedisRedLock(redisMuxers));

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
