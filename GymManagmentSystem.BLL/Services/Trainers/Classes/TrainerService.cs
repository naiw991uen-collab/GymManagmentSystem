using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.Trainers.Interfaces;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.BLL.Services.Trainers.Classes;

public class TrainerService : ITrainerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly ILogger<TrainerService> _logger;

    public TrainerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TrainerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<IEnumerable<TrainerViewMoldel>>> GetTrainersAsync()
    {
        var trainers = await  _unitOfWork.GetRepository<Trainer>().GetAllAsync();

        return Result<IEnumerable<TrainerViewMoldel>>.OK(_mapper.Map<IEnumerable<TrainerViewMoldel>>(trainers));
    }
    public async Task<Result> AddTrainerAsyn(CreateTrainerViewModel model)
    {
        try
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            bool IsNotUniquePhone = trainers.Any(x => x.Phone == model.Phone);
            bool IsNotUniqueEmail = trainers.Any(x => x.Email == model.Email);

            if (IsNotUniquePhone)
            {
                _logger.LogWarning("AddTrainer failed: phone {Phone} already exists", model.Phone);
                return Result.Fail("Phone Already Exist");
            }

            if (IsNotUniqueEmail)
            {
                _logger.LogWarning("AddTrainer failed: email already exists");
                return Result.Fail("Email Already Exist");
            }

            var trainer = _mapper.Map<Trainer>(model);

            await _unitOfWork.GetRepository<Trainer>().AddAsync(trainer);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Trainer {TrainerId} added successfully", trainer.Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("AddTrainer failed: SaveChanges returned 0 rows");
                return Result.Fail("Trainer is not Added");
            }
            
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "AddTrainer threw an unexpected exception");
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<Result<TrainerDetailsViewModel?>> GetTrainerAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
        {
            _logger.LogWarning("GetTrainer failed: invalid id");
            return Result<TrainerDetailsViewModel?>.Fail("Id Not Found");
        }

        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(Id.Value);

        if (trainer == null)
        {
            _logger.LogWarning("GetTrainer failed: trainer {TrainerId} not found", Id);
            return Result<TrainerDetailsViewModel?>.Fail("Trainer Not Found");
        }

        return Result<TrainerDetailsViewModel?>.OK(_mapper.Map<TrainerDetailsViewModel>(trainer));
    }

    public async Task<Result<TrainerToUpdateViewModel?>> GetTrainerToUpdateAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
        {
            _logger.LogWarning("GetTrainerToUpdate failed: invalid id");
            return Result<TrainerToUpdateViewModel?>.Fail("Id Not Found");
        }

        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(Id.Value);

        if (trainer == null)
        {
            _logger.LogWarning("GetTrainerToUpdate failed: trainer {TrainerId} not found", Id);
            return Result<TrainerToUpdateViewModel?>.Fail("Trainer Not Found");
        }

        return Result<TrainerToUpdateViewModel?>.OK(_mapper.Map<TrainerToUpdateViewModel>(trainer));
    }

    public async Task<Result> UpdateTrainerAsync(TrainerToUpdateViewModel model)
    {
       try
        {
            if (model == null)
            {
                _logger.LogWarning("UpdateTrainer failed: model is null");
                return Result.Fail("Model Not Found");
            }

            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            bool IsNotUniquePhone = trainers.Any(x => x.Phone == model.Phone && x.Id != model.Id);
            bool IsNotUniqueEmail = trainers.Any(x => x.Email == model.Email && x.Id != model.Id);


            if (IsNotUniquePhone)
            {
                _logger.LogWarning("UpdateTrainer failed for trainer {TrainerId}: phone {Phone} already exists", model.Id, model.Phone);
                return Result.Fail("Phone Already Exist");
            }

            if (IsNotUniqueEmail)
            {
                _logger.LogWarning("UpdateTrainer failed for trainer {TrainerId}: email already exists", model.Id);
                return Result.Fail("Email Already Exist");
            }

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.Id);

            if (trainer == null)
            {
                _logger.LogWarning("UpdateTrainer failed: trainer {TrainerId} not found", model.Id);
                return Result.Fail("Trainer Not Found");
            }

            _mapper.Map(model, trainer);

            await _unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Trainer {TrainerId} updated successfully", model.Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("UpdateTrainer failed for trainer {TrainerId}: SaveChanges returned 0 rows", model.Id);
                return Result.Fail("Trainer Can not be Updated");
            }
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "UpdateTrainer threw an unexpected exception for trainer {TrainerId}", model?.Id);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<Result> DeleteTrainerAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value == 0)
            {
                _logger.LogWarning("DeleteTrainer failed: invalid id");
                return Result.Fail("Id Not Found");
            }

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(Id.Value);

            if (trainer == null)
            {
                _logger.LogWarning("DeleteTrainer failed: trainer {TrainerId} not found", Id);
                return Result.Fail("Trainer Not Found");
            }

            await _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainer);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Trainer {TrainerId} deleted successfully", Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("DeleteTrainer failed for trainer {TrainerId}: SaveChanges returned 0 rows", Id);
                return Result.Fail("Member Can not be Deleted");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "DeleteTrainer threw an unexpected exception for trainer {TrainerId}", Id);
            return Result.Fail("Internal Server Error");
        }
    }
}
