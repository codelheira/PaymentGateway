using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.DTOs
{
    public class PaymentsDTOs
    {
        public record ProcessPaymentRequest(decimal Amount, string Currency);
        public record ProcessPaymentResult(
            Guid Id,
            string ExternalId,
            string Status,
            string Provider,
            decimal GrossAmount,
            decimal Fee,
            decimal NetAmount
        );


    }
}
