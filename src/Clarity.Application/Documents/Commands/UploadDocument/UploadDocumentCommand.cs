using System.Text.Json.Serialization;
using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Documents.Commands.UploadDocument;

public record UploadDocumentCommand : IRequest<Result<Guid>>
{
    public Guid? ClientId { get; init; }
    public Guid? MatterId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    [JsonIgnore]
    public Stream FileStream { get; init; } = Stream.Null;
    public string? Category { get; init; }
}
