using FluentValidation;
using MagicalKitties.Application.Models.GlobalSettings;

namespace MagicalKitties.Application.Validators.GlobalSettings;

public class GlobalSettingValidator : AbstractValidator<GlobalSetting>
{
    public GlobalSettingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}