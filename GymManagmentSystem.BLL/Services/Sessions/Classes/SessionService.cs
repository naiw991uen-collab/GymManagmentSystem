using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.Sessions.Interfaces;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Models.Enums;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using static System.Collections.Specialized.BitVector32;

namespace GymManagmentSystem.BLL.Services.Sessions.Classes;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly ILogger<SessionService> _logger;
    public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<IEnumerable<SessionViewModel>?>> GetAllSessionsAsync()
    {
        var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory();

        // Manual Mapping
        var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);



        foreach (var session in MappedSessions)
        {
            session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsnc(session.Id);
            // N + 1 Problem
        }

        return Result<IEnumerable<SessionViewModel>?>.OK(MappedSessions);
    }

    public async Task<Result> CreateSessionAsync(CreateSessionViewModel model)
    {
        try
        {
            if (model.EndDate <= model.StartDate)
            {
                _logger.LogWarning("CreateSession failed: EndDate must be after StartDate");
                return Result.Validation("EndDate must be After Start Date");
            }


            if (model.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("CreateSession failed: StartDate {StartDate} is not in the future", model.StartDate);
                return Result.Validation("StartDate must be In The Future");
            }


            if (model.Capacity < 1 || model.Capacity > 25)
            {
                _logger.LogWarning("CreateSession failed: invalid capacity {Capacity}", model.Capacity);
                return Result.Validation("Capacity must be between 1 and 25");
            }

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);

            if (trainer == null)
            {
                _logger.LogWarning("CreateSession failed: trainer {TrainerId} not found", model.TrainerId);
                return Result.NotFound("Trainer Not Found");
            }

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);

            if (category == null)
            {
                _logger.LogWarning("CreateSession failed: category {CategoryId} not found", model.CategoryId);
                return Result.NotFound("Category Not Found");
            }

            var IsValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var categorySpecialty);

            if (!IsValid || trainer.Specialty != categorySpecialty)
            {
                _logger.LogWarning("CreateSession failed: trainer {TrainerId} does not match category {CategoryId}", model.TrainerId, model.CategoryId);
                return Result.Validation("Can not create this session because trainer not match with category");
            }

            var session = _mapper.Map<Session>(model);

            await _unitOfWork.SessionRepository.AddAsync(session);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Session {SessionId} created successfully", session.Id);
                return Result.OK();
            }

            _logger.LogError("CreateSession failed: SaveChanges returned 0 rows");
            return Result.Fail("Failed to create session");
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "CreateSession threw an unexpected exception");
            return Result.Fail("Inernal Server Error");
        }
    }

    public async Task<Result<IEnumerable<TrainerSelectedViewModel>>> GetTrainersForDropDownAsync()
    {
        var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

        var model = _mapper.Map<IEnumerable<TrainerSelectedViewModel>>(result);

        return Result<IEnumerable<TrainerSelectedViewModel>>.OK(model);
    }

    public async Task<Result<IEnumerable<CategorySelectedViewModel>>> GetCategoriesForDropDownAsync()
    {
        var result = await _unitOfWork.GetRepository<Category>().GetAllAsync();

        var model = _mapper.Map<IEnumerable<CategorySelectedViewModel>>(result);

        return Result<IEnumerable<CategorySelectedViewModel>>.OK(model);
    }

    public async Task<Result<SessionDetailsViewModel?>> GetSessionDetailsAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value <= 0)
            {
                _logger.LogWarning("GetSessionDetails failed: invalid id");
                return Result<SessionDetailsViewModel?>.Fail("Invalid Id");
            }

            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(Id.Value);

            if (session == null)
            {
                _logger.LogWarning("GetSessionDetails failed: session {SessionId} not found", Id);
                return Result<SessionDetailsViewModel?>.Fail("Session Not Found");
            }

            var model = _mapper.Map<SessionDetailsViewModel>(session);

            model.AvailableSlots = model.Capacity
                - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsnc(session.Id);

            return Result<SessionDetailsViewModel?>.OK(model);
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "GetSessionDetails threw an unexpected exception for session {SessionId}", Id);
            return Result<SessionDetailsViewModel?>.Fail("Internal Server Error");
        }
    }

    public async Task<Result<SessionToUpdateViewModel?>> GetSessionToUpdateAsync(int? Id)
    {
       try
       {
            if (Id == null || Id.Value == 0)
            {
                _logger.LogWarning("GetSessionToUpdate failed: invalid id");
                return Result<SessionToUpdateViewModel?>.Fail("Id Not Found");
            }

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(Id.Value);

            if (session == null)
            {
                _logger.LogWarning("GetSessionToUpdate failed: session {SessionId} not found", Id);
                return Result<SessionToUpdateViewModel?>.Fail("Session Not Found");
            }

            if (session.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("GetSessionToUpdate failed: session {SessionId} already started", Id);
                return Result<SessionToUpdateViewModel?>.Fail("Only upcoming sessions can be edited");
            }

            return Result<SessionToUpdateViewModel?>.OK(_mapper.Map<SessionToUpdateViewModel>(session));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "GetSessionToUpdate threw an unexpected exception for session {SessionId}", Id);
            return Result<SessionToUpdateViewModel?>.Fail("Internal Server Error");
        }
    }

    public async Task<Result> UpdateSessionAsync(SessionToUpdateViewModel model)
    {
        try
        {
            if (model == null)
            {
                _logger.LogWarning("UpdateSession failed: model is null");
                return Result.Fail("Model Not Found");
            }

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(model.Id);

            if (session == null)
            {
                _logger.LogWarning("UpdateSession failed: session {SessionId} not found", model.Id);
                return Result.Fail("Session Not Found");
            }

            if (session.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("UpdateSession failed: session {SessionId} already started", model.Id);
                return Result.Fail("Only upcoming sessions can be edited");
            }

            if (model.EndDate <= model.StartDate)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: EndDate must be after StartDate", model.Id);
                return Result.Fail("EndDate must be After Start Date");
            }

            if (model.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: StartDate is not in the future", model.Id);
                return Result.Fail("StartDate must be In The Future");
            }

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: trainer {TrainerId} not found", model.Id, model.TrainerId);
                return Result.Fail("Trainer Not Found");
            }

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            if (category == null)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: category {CategoryId} not found", model.Id, session.CategoryId);
                return Result.Fail("Category Not Found");
            }

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var categorySpecialty);

            if (!isValid || trainer.Specialty != categorySpecialty)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: trainer {TrainerId} does not match category {CategoryId}", model.Id, model.TrainerId, session.CategoryId);
                return Result.Fail("Trainer does not match the session category");
            }

            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();

            bool IsTrainerBusy = sessions.Any(x =>
                x.TrainerId == model.TrainerId &&
                x.Id != model.Id &&
                x.StartDate < model.EndDate &&
                x.EndDate > model.StartDate);

            if (IsTrainerBusy)
            {
                _logger.LogWarning("UpdateSession failed for session {SessionId}: trainer {TrainerId} is already booked in this time slot", model.Id, model.TrainerId);
                return Result.Fail("Trainer is already booked in this time slot");
            }

            _mapper.Map(model, session);

            await _unitOfWork.GetRepository<Session>().UpdateAsync(session);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Session {SessionId} updated successfully", model.Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("UpdateSession failed for session {SessionId}: SaveChanges returned 0 rows", model.Id);
                return Result.Fail("Session Can not be Updated");
            }
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "UpdateSession threw an unexpected exception for session {SessionId}", model?.Id);
            return Result.Fail("Internal Server Error");
        }
    }
    public async Task<Result> DeleteSessionAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value <= 0)
            {
                _logger.LogWarning("DeleteSession failed: invalid id");
                return Result.Fail("Invalid Id");
            }

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(Id.Value);

            if (session == null)
            {
                _logger.LogWarning("DeleteSession failed: session {SessionId} not found", Id);
                return Result.Fail("Session Not Found");
            }

            await _unitOfWork.GetRepository<Session>().DeleteAsync(session);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Session {SessionId} deleted successfully", Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("DeleteSession failed for session {SessionId}: SaveChanges returned 0 rows", Id);
                return Result.Fail("Member Can not be Deleted");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "DeleteSession threw an unexpected exception for session {SessionId}", Id);
            return Result.Fail("Internal Server Error");
        }
    }
}
