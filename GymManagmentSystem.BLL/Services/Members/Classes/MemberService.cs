using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;

namespace GymManagmentSystem.BLL.Services.Members.Classes;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<MemberViewModel>> GetMembersAsyn()
    {
        var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

        return members.Select(x => new MemberViewModel()
        {
            Id = x.Id,
            Email = x.Email,
            Gender = x.Gender.ToString(),
            Name = x.Name,
            Phone = x.Phone,
            Photo = x.Photo,
        });
    }
    public async Task<bool> AddMemberAsync(CreateMemberViewModel model)
    {
        try
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            bool IsNotUniquePhone = members.Any(x => x.Phone == model.Phone);
            bool IsNotUniqueEmail = members.Any(x => x.Email == model.Email);

            if (IsNotUniquePhone || IsNotUniqueEmail)
                return false;

            var Member = new Member()
            {
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                Gender = model.Gender,
                Name = model.Name,
                Phone = model.Phone,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                },
                HealthRecoed = new HealthRecoed()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note,
                }
            };

            await _unitOfWork.GetRepository<Member>().AddAsync(Member);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        catch
        {
            return false;
        }
    }
    public async Task<MemberDetailsViewModel?> GetMemberAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
            return null;

        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

        if(member == null) 
            return null;

        var model = new MemberDetailsViewModel()
        {
            Email = member.Email,
            Gender = member.Gender.ToString(),
            Name = member.Name,
            Phone = member.Phone,
            Photo = member.Photo,
            DateOfBirth = member.DateOfBirth.ToString("d/M/yyyy"),
            Address = $"{member.Address?.BuildingNumber} - {member.Address?.Street} - {member.Address?.City}",
            //MembeshipStartDate = member.MemberShips.Where(x => x.IsActive).Select(x => x.CreatedAt.ToString()).FirstOrDefault(),
            //MembeshipEndDate = member.MemberShips.Where(x => x.IsActive).Select(x => x.EndDate.ToString()).FirstOrDefault(),
            //PlanName = member.MemberShips.Where(x => x.IsActive).Select(x => x.Plan.Name).FirstOrDefault()
        };

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
            }
        }
        return model;
    }

    public async Task<HealthRecordViewModel?> GetHealthRecordAsync(int? Id)
    {
        var healthRecords = await _unitOfWork.GetRepository<HealthRecoed>().GetAllAsync();

        if(healthRecords.Any())
        {
            var healthRecord = healthRecords.Where(x => x.MemberId == Id).FirstOrDefault();

            if( healthRecord != null)
            {
                var model = new HealthRecordViewModel()
                {
                    BloodType = healthRecord.BloodType,
                    Height = healthRecord.Height,
                    Weight = healthRecord.Weight,
                    Note = healthRecord.Note,
                };

                return model;
            }
        }
        return null;
    }

    public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
            return null;

        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

        if(member == null)
            return null;

        var model = new MemberToUpdateViewModel()
        {
            Id = member.Id,
            Email = member.Email,
            Name = member.Name,
            Phone = member.Phone,
            Photo = member.Photo,
            BuildingNumber = member.Address?.BuildingNumber ?? 0,
            City = member.Address?.City ?? "Not Found",
            Street = member.Address?.Street ?? "Not Found",
        };

        return model;
    }

    public async Task<bool> UpdateMemberAsync(MemberToUpdateViewModel model)
    {
        try
        {
            if (model == null)
                return false;

            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            bool IsNotUniquePhone = members.Any(x => x.Phone == model.Phone && x.Id != model.Id);
            bool IsNotUniqueEmail = members.Any(x => x.Email == model.Email && x.Id != model.Id);

            if (IsNotUniquePhone || IsNotUniqueEmail)
                return false;

            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.Id);

            if (member == null)
                return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BuildingNumber = model.BuildingNumber;

            await _unitOfWork.GetRepository<Member>().UpdateAsync(member);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> DeleteMemberAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value == 0)
                return false;

            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(Id.Value);

            if (member == null)
                return false;

            await _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        catch 
        {
            return false;
        }
    }
}
