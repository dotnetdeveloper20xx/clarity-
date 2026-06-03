using FluentValidation;

namespace Clarity.Application.Matters.Commands.CreateMatter;

public class CreateMatterCommandValidator : AbstractValidator<CreateMatterCommand>
{
    public CreateMatterCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Matter title is required.")
            .MaximumLength(300).WithMessage("Matter title must not exceed 300 characters.");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client is required.");

        RuleFor(x => x.LeadConsultantId)
            .NotEmpty().WithMessage("Lead consultant is required.");

        RuleFor(x => x.MatterType)
            .IsInEnum().WithMessage("Invalid matter type.");

        RuleFor(x => x.FeeArrangement)
            .IsInEnum().WithMessage("Invalid fee arrangement.");

        RuleFor(x => x.EstimatedValue)
            .GreaterThanOrEqualTo(0).When(x => x.EstimatedValue.HasValue)
            .WithMessage("Estimated value must be zero or greater.");
    }
}
