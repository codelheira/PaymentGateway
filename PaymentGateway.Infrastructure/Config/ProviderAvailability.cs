namespace PaymentGateway.Infrastructure.Config
{
    public record ProviderAvailability(bool FastPayAvailable, bool SecurePayAvailable);
}
