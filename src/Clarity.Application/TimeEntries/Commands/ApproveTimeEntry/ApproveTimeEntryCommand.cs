using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.TimeEntries.Commands.ApproveTimeEntry;

public record ApproveTimeEntryCommand(Guid Id) : IRequest<Result>;
