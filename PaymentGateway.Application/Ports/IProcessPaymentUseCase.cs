using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGateway.Application.DTOs.PaymentsDTOs;

namespace PaymentGateway.Application.Ports
{
    public interface IProcessPaymentUseCase
    {
        Task<ProcessPaymentResult> ExecuteAsync(ProcessPaymentRequest request, CancellationToken ct = default);
    }

}
