using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class HumanService : IHumanService
{
    private readonly IHumanRepository _humanRepository;
    private readonly IValidator<Human> _humanValidator;
    private readonly IValidator<GetAllHumansOptions> _optionsValidator;
    private readonly IProblemRepository _problemRepository;

    public HumanService(IHumanRepository humanRepository, IValidator<Human> humanValidator, IValidator<GetAllHumansOptions> optionsValidator, IProblemRepository problemRepository)
    {
        _humanRepository = humanRepository;
        _humanValidator = humanValidator;
        _optionsValidator = optionsValidator;
        _problemRepository = problemRepository;
    }

    public async Task<Human> CreateAsync(Guid characterId, CancellationToken token = default)
    {
        Human human = new()
                      {
                          Id = Guid.NewGuid(),
                          CharacterId = characterId,
                          Name = "",
                          Description = "",
                          Problems = []
                      };

        await _humanValidator.ValidateAndThrowAsync(human, token);

        await _humanRepository.CreateAsync(human, token);

        return human;
    }

    public async Task<Problem> CreateProblemAsync(Guid characterId, Guid humanId, CancellationToken token = default)
    {
        Human? human = await _humanRepository.GetByIdAsync(characterId, humanId, token: token);

        if (human is null)
        {
            throw new ValidationException([new ValidationFailure("Human", "No Human Found")]);
        }

        Problem problem = new()
                          {
                              Id = Guid.NewGuid(),
                              HumanId = humanId,
                              Emotion = "",
                              Rank = 0,
                              Solved = false,
                              Source = ""
                          };

        bool success =  await _problemRepository.CreateProblemAsync(problem, token);

        return success ? problem : throw new Exception("Failure creating Human Problem.");
    }

    public async Task<Human?> GetByIdAsync(Guid characterId, Guid humanId, CancellationToken token = default)
    {
        return await _humanRepository.GetByIdAsync(characterId, humanId, token: token);
    }

    public async Task<IEnumerable<Human>> GetAllAsync(GetAllHumansOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _humanRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllHumansOptions options, CancellationToken token = default)
    {
        return await _humanRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        bool humanExists = await _humanRepository.ExistsByIdAsync(update.CharacterId, update.HumanId, token);

        if (!humanExists)
        {
            return false;
        }

        return update.DescriptionOption switch
        {
            DescriptionOption.name => await _humanRepository.UpdateNameAsync(update, token),
            DescriptionOption.description => await _humanRepository.UpdateDescriptionAsync(update, token),
            _ => throw new ValidationException([new ValidationFailure("DescriptionOption", "Selected description option not valid")])
        };
    }

    public async Task<bool> UpdateProblemAsync(ProblemUpdate update, CancellationToken token = default)
    {
        Human? human = await _humanRepository.GetByIdAsync(update.CharacterId, update.HumanId, token: token);

        if (human is null)
        {
            return false;
        }

        return update.ProblemOption switch
        {
            ProblemOption.source => await _problemRepository.UpdateSourceAsync(update, token),
            ProblemOption.emotion => await _problemRepository.UpdateEmotionAsync(update, token),
            ProblemOption.rank => await _problemRepository.UpdateRankAsync(update, token),
            ProblemOption.solved => await _problemRepository.UpdateSolvedAsync(update, token),
            _ => throw new ValidationException([new ValidationFailure("ProblemOption", "Selected problem option not valid")])
        };
    }

    public async Task<bool> DeleteAsync(Guid characterId, Guid humanId, CancellationToken token = default)
    {
        Human? human = await _humanRepository.GetByIdAsync(characterId, humanId, token: token);

        if (human is null)
        {
            return false;
        }

        return await _humanRepository.DeleteAsync(human.Id, token);
    }

    public async Task<bool> DeleteProblemAsync(Guid characterId, Guid humanId, Guid problemId, CancellationToken token = default)
    {
        bool humanExists = await _humanRepository.ExistsByIdAsync(characterId, humanId, token);

        if (!humanExists)
        {
            return false;
        }

        bool problemExists = await _problemRepository.ExistsByIdAsync(problemId, token);

        if (!problemExists)
        {
            return false;
        }

        return await _problemRepository.DeleteAsync(problemId, token);
    }
}