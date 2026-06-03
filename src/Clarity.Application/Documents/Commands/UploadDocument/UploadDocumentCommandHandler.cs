using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;
    private readonly IFileStorageService _fileStorage;

    public UploadDocumentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        if (request.ClientId is null && request.MatterId is null)
            return Result<Guid>.Failure("Document must be linked to a client or matter.");

        var storagePath = await _fileStorage.UploadAsync(request.FileStream, request.FileName, request.ContentType, cancellationToken);

        var document = new Document
        {
            ClientId = request.ClientId,
            MatterId = request.MatterId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            StoragePath = storagePath,
            Version = 1,
            Category = request.Category,
            Status = DocumentStatus.Active,
            UploadedById = _currentUser.UserId ?? Guid.Empty,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(document.Id);
    }
}
