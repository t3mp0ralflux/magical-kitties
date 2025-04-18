using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class FlawService : IFlawService
{
    private readonly IValidator<Flaw> _flawValidator;
    private readonly IValidator<GetAllFlawsOptions> _optionsValidator;
    private readonly IFlawRepository _flawRepository;

    public FlawService(IValidator<Flaw> flawValidator, IFlawRepository flawRepository, IValidator<GetAllFlawsOptions> optionsValidator)
    {
        this._flawValidator = flawValidator;
        _flawRepository = flawRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Flaw flaw, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(flaw, token);

        bool result = await _flawRepository.CreateAsync(flaw, token);

        return result;
    }

    public async Task<Flaw?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return await _flawRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<Flaw>> GetAllAsync(GetAllFlawsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        
        return await _flawRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllFlawsOptions options, CancellationToken token = default)
    {
        return await _flawRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateAsync(Flaw flaw, CancellationToken token = default)
    {
        await _flawValidator.ValidateAndThrowAsync(flaw, token);

        return await _flawRepository.UpdateAsync(flaw, token);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        return await _flawRepository.DeleteAsync(id, token);
    }
}