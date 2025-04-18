using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class TalentService : ITalentService
{
    private readonly IValidator<Endowment> _flawValidator;
    private readonly IValidator<GetAllTalentsOptions> _optionsValidator;
    private readonly ITalentRepository _talentRepository;

    public TalentService(IValidator<Endowment> flawValidator, ITalentRepository talentRepository, IValidator<GetAllTalentsOptions> optionsValidator)
    {
        this._flawValidator = flawValidator;
        _talentRepository = talentRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Endowment flaw, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(flaw, token);

        bool result = await _talentRepository.CreateAsync(flaw, token);

        return result;
    }

    public async Task<Endowment?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return await _talentRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<Endowment>> GetAllAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        
        return await _talentRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllTalentsOptions options, CancellationToken token = default)
    {
        return await _talentRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateAsync(Endowment flaw, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(flaw, token);

        return await _talentRepository.UpdateAsync(flaw, token);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        return await _talentRepository.DeleteAsync(id, token);
    }
}