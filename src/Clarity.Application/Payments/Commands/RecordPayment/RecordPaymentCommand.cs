using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Payments.Commands.RecordPayment;

public record RecordPaymentCommand : IRequest<Result<Guid>>
{
    public Guid InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public DateOnly PaymentDate { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
}
