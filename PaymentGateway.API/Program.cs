using PaymentGateway.Api.Endpoints;
using PaymentGateway.Application.Ports;
using PaymentGateway.Application.Services;
using PaymentGateway.Infrastructure.Config;
using PaymentGateway.Infrastructure.Persistence;
using PaymentGateway.Infrastructure.Providers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Configure logging to include scopes (essencial para correlation/trace nos logs)
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
});
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // importante para usar SwaggerOperation/SwaggerResponse
    var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Bind ProviderAvailability from configuration (inclui variáveis de ambiente)
var providerAvailability = builder.Configuration.GetSection("ProviderAvailability").Get<ProviderAvailability>()
                         ?? new ProviderAvailability(true, true);

// Register the bound instance so DI can resolve it for adapters
builder.Services.AddSingleton(providerAvailability);

// Injeções de dependências
builder.Services.AddSingleton<IProviderPort, FastPayAdapter>();
builder.Services.AddSingleton<IProviderPort, SecurePayAdapter>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<IProcessPaymentUseCase, ProcessPaymentService>();

var app = builder.Build();
// Adiciona middleware de tracing logo no início do pipeline
app.UseMiddleware<PaymentGateway.Api.Middleware.TracingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Configuração de Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
PaymentsEndpoints.MapPaymentEndpoints(app);
app.Run();