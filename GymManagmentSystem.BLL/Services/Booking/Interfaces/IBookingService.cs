using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Booking.ViewModel;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;

namespace GymManagmentSystem.BLL.Services.Booking.Interfaces;
public interface IBookingService
{
    public Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken ct = default);

    public Task<IEnumerable<MemberForSessionViewModel>> GetMemberForUpCommingSessionByIdAsync(int sessionId , CancellationToken ct = default);
    public Task<IEnumerable<MemberForSessionViewModel>> GetMemberForOnGoingSessionByIdAsync(int sessionId , CancellationToken ct = default);

    public Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropdownViewModelAsync(int sessionId , CancellationToken ct = default);

    public Task<Result> CreateNewBookingAsync(CreateBookingViewModel createBookingViewModel, CancellationToken ct = default);

    public Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);

    public Task<Result> MarkAsAttend(int memberId, int sessionId, CancellationToken ct = default);
}
