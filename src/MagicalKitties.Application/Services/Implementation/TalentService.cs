using FluentValidation;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class TalentService : ITalentService
{
    private readonly IValidator<Talent> _flawValidator;
    private readonly IValidator<GetAllTalentsOptions> _optionsValidator;
    private readonly ITalentRepository _talentRepository;

    public TalentService(IValidator<Talent> flawValidator, ITalentRepository talentRepository, IValidator<GetAllTalentsOptions> optionsValidator)
    {
        _flawValidator = flawValidator;
        _talentRepository = talentRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Talent flaw, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(flaw, token);

        bool result = await _talentRepository.CreateAsync(flaw, token);

        return result;
    }

    public async Task<Talent?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return await _talentRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<Talent>> GetAllAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _talentRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        return await _talentRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateAsync(Talent talent, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(talent, token);

        return await _talentRepository.UpdateAsync(talent, token);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        return await _talentRepository.DeleteAsync(id, token);
    }
}