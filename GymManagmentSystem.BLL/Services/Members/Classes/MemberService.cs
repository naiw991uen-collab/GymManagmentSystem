using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Attachment.Interface;
using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.BLL.Services.Members.Classes;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly ILogger<MemberService> _logger;
    private readonly IAttachmentService _attachmentService;
    public MemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberService> logger, IAttachmentService attachmentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _attachmentService = attachmentService;
    }
    public  async Task<Result<IEnumerable<MemberViewModel>>> GetMembersAsyn()
    {
        var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

        return Result<IEnumerable<MemberViewModel>>.OK(_mapper.Map<IEnumerable<MemberViewModel>>(members));
    }
    public async Task<Result> AddMemberAsync(CreateMemberViewModel model)
    {
        try
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            if (members.Any(x => x.Phone == model.Phone))
            {
                _logger.LogWarning("AddMember failed: phone {Phone} already exists", model.Phone);
                return Result.Fail("Phone Already Exist");
            }

            if (members.Any(x => x.Email == model.Email))
            {
                _logger.LogWarning("AddMember failed: email {Email} already exists", model.Email);
                return Result.Fail("Email Already Exist");
            }

            using var photoStream = model.PhotoFile.OpenReadStream();
            var photoResult = await _attachmentService.UploadAsync(photoStream, model.PhotoFile.FileName, "MemberPhotos");

            if (!photoResult.Success)
            {
                _logger.LogWarning("AddMember failed: photo upload failed");
                return Result.Fail("Photo upload failed");
            }

            var member = _mapper.Map<Member>(model);
            member.Photo = photoResult.Value;

            await _unitOfWork.GetRepository<Member>().AddAsync(member);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;

            if (saved)
            {
                _logger.LogInformation("Member {MemberId} added successfully", member.Id);
                return Result.OK();
            }

            _attachmentService.DeleteAsync(photoResult.Value, "MemberPhotos");
            _logger.LogError("AddMember failed: SaveChanges returned 0 rows");
            return Result.Fail("Member is not Added");
        }
        catch (Exception ex)
        {
            _attachmentService.DeleteAsync(model.PhotoFile.FileName, "MembersPhotos");
            _logger.LogError(ex, "AddMember threw an unexpected exception");
            return Result.Fail("Internal Server Error");
        }
    }
    public async Task<Result<MemberDetailsViewModel?>> GetMemberAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
        {
            _logger.LogWarning("GetMember failed: invalid id");
            return Result<MemberDetailsViewModel?>.Fail("Id Not Found");
        }

        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

        if(member == null)
        {
            _logger.LogWarning("GetMember failed: member {MemberId} not found", Id);
            return Result<MemberDetailsViewModel?>.Fail("Member Not Found");
        }

        // Automatic Mapper
        var model = _mapper.Map<MemberDetailsViewModel>(member);

        var memberships = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync();

        if(memberships.Any())
        {
            var membershipActive =  memberships.Where(x => x.MemberId == member.Id && x.IsActive).FirstOrDefault();

            if (membershipActive != null)
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(membershipActive.PlanId);
                model.MembershipStartDate = membershipActive.CreatedAt.ToString("d/M/yyyy");
                model.MembershipEndDate = membershipActive.EndDate.ToString("d/M/yyyy");

                if(plan !=null)
                    model.PlanName = plan.Name;
                else
                    _logger.LogWarning("GetMember: plan {PlanId} of member {MemberId} not found", membershipActive.PlanId, member.Id);
            }
        }
        return Result<MemberDetailsViewModel?>.OK(model);
    }

    public async Task<Result<HealthRecordViewModel?>> GetHealthRecordAsync(int? Id)
    {
        var healthRecords = await _unitOfWork.GetRepository<HealthRecoed>().GetAllAsync();

        if(healthRecords.Any())
        {
            var healthRecord = healthRecords.Where(x => x.MemberId == Id).FirstOrDefault();

            if( healthRecord != null)
            {
                var model = _mapper.Map<HealthRecordViewModel>(healthRecord);

                return Result<HealthRecordViewModel?>.OK(model);
            }
        }
        _logger.LogWarning("GetHealthRecord failed: no health record for member {MemberId}", Id);
        return Result<HealthRecordViewModel?>.Fail("Health Record Not Found");
    }

    public async Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
        {
            _logger.LogWarning("GetMemberToUpdate failed: invalid id");
            return Result<MemberToUpdateViewModel?>.Fail("Id Not Found");
        }

        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

        if(member == null)
        {
            _logger.LogWarning("GetMemberToUpdate failed: member {MemberId} not found", Id);
            return Result<MemberToUpdateViewModel?>.Fail("Member Not Found");
        }

        var model = _mapper.Map<MemberToUpdateViewModel>(member);

        return Result<MemberToUpdateViewModel?>.OK(model);
    }

    public async Task<Result> UpdateMemberAsync(MemberToUpdateViewModel model)
    {
        try
        {
            if (model == null)
            {
                _logger.LogWarning("UpdateMember failed: model is null");
                return Result.Fail("Model Not Found"); 
            }

            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            bool IsNotUniquePhone = members.Any(x => x.Phone == model.Phone && x.Id != model.Id);
            bool IsNotUniqueEmail = members.Any(x => x.Email == model.Email && x.Id != model.Id);

            if (IsNotUniquePhone)
            {
                _logger.LogWarning("UpdateMember failed for member {MemberId}: phone {Phone} already exists", model.Id, model.Phone);
                return Result.Fail("Phone Already Exist");
            }

            if (IsNotUniqueEmail)
            {
                _logger.LogWarning("UpdateMember failed for member {MemberId}: email already exists", model.Id);
                return Result.Fail("Email Already Exist");
            }

            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.Id);

            if (member == null)
            {
                _logger.LogWarning("UpdateMember failed: member {MemberId} not found", model.Id);
                return Result.Fail("Member Not Found");
            }

            var mappedmember = _mapper.Map(model, member);

            await _unitOfWork.GetRepository<Member>().UpdateAsync(mappedmember);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if(result)
            {
                _logger.LogInformation("Member {MemberId} updated successfully", model.Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("UpdateMember failed for member {MemberId}: SaveChanges returned 0 rows", model.Id);
                return Result.Fail("Member Can not be Updated");
            }
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "UpdateMember threw an unexpected exception for member {MemberId}", model?.Id);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<Result> DeleteMemberAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value == 0)
            {
                _logger.LogWarning("DeleteMember failed: invalid id");
                return Result.Fail("Id Not Found");
            }
                

            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

            if (member == null)
            {
                _logger.LogWarning("DeleteMember failed: member {MemberId} not found", Id);
                return Result.Fail("Member Not Found");
            }

            await _unitOfWork.GetRepository<Member>().DeleteAsync(member);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Member {MemberId} deleted successfully", Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("DeleteMember failed for member {MemberId}: SaveChanges returned 0 rows", Id);
                return Result.Fail("Member Can not be Deleted");
            }
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "DeleteMember threw an unexpected exception for member {MemberId}", Id);
            return Result.Fail("Internal Server Error");
        }
    }
}
