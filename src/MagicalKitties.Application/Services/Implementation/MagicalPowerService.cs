using FluentValidation;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class MagicalPowerService : IMagicalPowerService
{
    private readonly IMagicalPowerRepository _magicalPowerRepository;
    private readonly IValidator<MagicalPower> _magicalPowerValidator;
    private readonly IValidator<GetAllMagicalPowersOptions> _optionsValidator;

    public MagicalPowerService(IValidator<MagicalPower> magicalPowerValidator, IMagicalPowerRepository magicalPowerRepository, IValidator<GetAllMagicalPowersOptions> optionsValidator)
    {
        _magicalPowerValidator = magicalPowerValidator;
        _magicalPowerRepository = magicalPowerRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(MagicalPower magicalPower, CancellationToken token = default)
    {
        await _magicalPowerValidator.ValidateAndThrowAsync(magicalPower, token);

        bool result = await _magicalPowerRepository.CreateAsync(magicalPower, token);

        return result;
    }

    public async Task<MagicalPower?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return await _magicalPowerRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<MagicalPower>> GetAllAsync(GetAllMagicalPowersOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _magicalPowerRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllMagicalPowersOptions options, CancellationToken token = default)
    {
        return await _magicalPowerRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateAsync(MagicalPower magicalPower, CancellationToken token = default)
    {
        await _magicalPowerValidator.ValidateAndThrowAsync(magicalPower, token);

        return await _magicalPowerRepository.UpdateAsync(magicalPower, token);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        return await _magicalPowerRepository.DeleteAsync(id, token);
    }
}