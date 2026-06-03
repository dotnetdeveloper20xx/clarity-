using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Invoices.Commands.IssueInvoice;

public record IssueInvoiceCommand(Guid Id) : IRequest<Result>;
