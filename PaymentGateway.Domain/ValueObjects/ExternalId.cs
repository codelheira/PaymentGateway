using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.ValueObjects
{
    public sealed record ExternalId(string Value)
    {
        public static ExternalId None => new(string.Empty);
    }
}
