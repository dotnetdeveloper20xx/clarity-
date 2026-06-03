using FluentValidation;

namespace Clarity.Application.TimeEntries.Commands.RecordTime;

public class RecordTimeCommandValidator : AbstractValidator<RecordTimeCommand>
{
    public RecordTimeCommandValidator()
    {
        RuleFor(x => x.MatterId).NotEmpty().WithMessage("Matter is required.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero.")
            .LessThanOrEqualTo(1440).WithMessage("Duration cannot exceed 24 hours.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}
