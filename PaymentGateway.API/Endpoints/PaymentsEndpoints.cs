using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Ports;
using PaymentGateway.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using static PaymentGateway.Application.DTOs.PaymentsDTOs;

namespace PaymentGateway.Api.Endpoints;

public class PaymentsEndpoints
{
    public static void MapPaymentEndpoints(WebApplication app)
    {
        // Endpoint para processar um pagamento
        app.MapPost("/payments", async (
            [FromBody] ProcessPaymentRequest payload,
            IProcessPaymentUseCase useCase,
            ILogger<PaymentsEndpoints> logger,
            CancellationToken ct) =>
        {
            // Validação
            if (payload == null || payload.Amount <= 0)
            {
                logger.LogWarning("Invalid payment request received");
                return Results.BadRequest(new { error = "invalid request" });
            }

            using (logger.BeginScope("ProcessPayment"))
            {
                logger.LogInformation("Start processing payment. Amount={Amount} Currency={Currency}", payload.Amount, payload.Currency);

                try
                {
                    var req = new ProcessPaymentRequest(payload.Amount, payload.Currency ?? CurrencyEnum.BRL.ToString());
                    var result = await useCase.ExecuteAsync(req, ct);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning(ex, "Provider unavailable");
                    return Results.StatusCode(503);
                }
                catch (ArgumentException ex)
                {
                    logger.LogWarning(ex, "Validation error");
                    return Results.BadRequest(new { error = ex.Message });
                }
            }
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status503ServiceUnavailable)
        .WithName("ProcessPayment")
        .WithTags("Payments")
        .WithMetadata(new SwaggerOperationAttribute
        {
            Summary = "Processa um pagamento",
            Description = "Recebe 'amount' e opcionalmente 'currency' e tenta processar o pagamento."
        })
        .WithMetadata(new SwaggerResponseAttribute(StatusCodes.Status200OK, "Pagamento processado com sucesso"))
        .WithMetadata(new SwaggerResponseAttribute(StatusCodes.Status400BadRequest, "Requisição inválida ou erro de validação"))
        .WithMetadata(new SwaggerResponseAttribute(StatusCodes.Status503ServiceUnavailable, "Serviço de pagamento indisponível")); ;

        // [BONUS] Endpoint para listar todos os pagamentos
        app.MapGet("/payments", async (IPaymentRepository repository, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("PaymentsEndpoints");
            logger.LogInformation("MARKER: entering ListPayments handler");
            var payments = await repository.GetAllAsync(ct);
            logger.LogInformation("Returned {Count} payments", payments?.Count ?? 0);
            return Results.Ok(payments);
        })
        .Produces(StatusCodes.Status200OK)
        .WithName("ListPayments")
        .WithTags("Payments")
        .WithMetadata(new SwaggerOperationAttribute
        {
            Summary = "Lista pagamentos",
            Description = "Retorna todos os pagamentos armazenados."
        })
        .WithMetadata(new SwaggerResponseAttribute(StatusCodes.Status200OK, "Lista de pagamentos retornada com sucesso")); ;
    }
}

