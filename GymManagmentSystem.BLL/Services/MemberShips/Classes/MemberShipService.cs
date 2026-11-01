using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.MemberShips.Interfaces;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.BLL.Services.MemberShips.Classes;

public class MemberShipService : IMemberShipService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ILogger<MemberShipService> _logger;
    public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberShipService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var isMemberExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, cancellationToken);

            if (!isMemberExists)
            {
                _logger.LogWarning("CreateMemberShip failed: member {MemberId} not found", model.MemberId);
                return Result.NotFound("Member Not Found");
            }

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, cancellationToken);

            if (plan is null)
            {
                _logger.LogWarning("CreateMemberShip failed: plan {PlanId} not found", model.PlanId);
                return Result.NotFound("Plan Not Found");
            }

            if (!plan.IsActive)
            {
                _logger.LogWarning("CreateMemberShip failed: plan {PlanId} is not active", model.PlanId);
                return Result.Fail("Plan Is Not Active");
            }

            var hasAvtiveMemberSip = await _unitOfWork.MemberShipRepository.
                AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, cancellationToken);

            if (hasAvtiveMemberSip)
            {
                _logger.LogWarning("CreateMemberShip failed: member {MemberId} already has an active membership", model.MemberId);
                return Result.Fail("Member Already Has an Active Membership");
            }

            var entity = _mapper.Map<MemberShip>(model);
            entity.EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays);

            await _unitOfWork.MemberShipRepository.AddAsync(entity);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Membership created for member {MemberId} on plan {PlanId}, ends {EndDate}", model.MemberId, model.PlanId, entity.EndDate);
                return Result.OK();
            }
            else
            {
                _logger.LogError("CreateMemberShip failed for member {MemberId}: SaveChanges returned 0 rows", model.MemberId);
                return Result.Fail("Member is not Added");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("CreateMemberShip was cancelled for member {MemberId}", model.MemberId);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<Result> DeleteActiceMemberShipAsync(int memberId, CancellationToken cancellationToken = default)
    {
        try
        {
            var active = await _unitOfWork.MemberShipRepository.FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now, tracking: true, cancellationToken);

            if (active is null)
            {
                _logger.LogWarning("DeleteActiveMemberShip failed: no active membership for member {MemberId}", memberId);
                return Result.NotFound("Active Membership Not Found");
            }

            await _unitOfWork.MemberShipRepository.DeleteAsync(active);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Active membership of member {MemberId} deleted successfully", memberId);
                return Result.OK();
            }
            else
            {
                _logger.LogError("DeleteActiveMemberShip failed for member {MemberId}: SaveChanges returned 0 rows", memberId);
                return Result.Fail("Membership is not Deleted");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DeleteActiveMemberShip was cancelled for member {MemberId}", memberId);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<IEnumerable<MemberSelectListViewModel>> GetAllMemberSelectListAsync(CancellationToken cancellationToken = default)
    {
        var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: cancellationToken);

        return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
    }

    public async Task<IEnumerable<MemberShipViewModel>> GetAllMembershipAsync(CancellationToken cancellationToken = default)
    {
        var memberships = await _unitOfWork.MemberShipRepository
        .GetAllMemberShipsWithMebersAndPlansAsync(m => m.EndDate > DateTime.Now, cancellationToken);

        return _mapper.Map<IEnumerable<MemberShipViewModel>>(memberships);
    }

    public async Task<IEnumerable<PlanSelectListViewModel>> GetAllPlanSelectListAsync(CancellationToken cancellationToken = default)
    {
        var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(p => p.IsActive, ct: cancellationToken);

        return _mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
    }
}
