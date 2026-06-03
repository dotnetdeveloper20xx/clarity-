using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Matters.Queries.GetMatter;

public record GetMatterQuery(Guid Id) : IRequest<Result<MatterDto>>;
