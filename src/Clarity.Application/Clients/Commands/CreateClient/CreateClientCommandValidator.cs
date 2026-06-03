using FluentValidation;

namespace Clarity.Application.Clients.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(200).WithMessage("Client name must not exceed 200 characters.");

        RuleFor(x => x.ClientType)
            .IsInEnum().WithMessage("Invalid client type.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.PostCode)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.PostCode));
    }
}
