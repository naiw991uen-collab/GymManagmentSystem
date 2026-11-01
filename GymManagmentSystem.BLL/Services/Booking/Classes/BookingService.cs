using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Booking.Interfaces;
using GymManagmentSystem.BLL.Services.Booking.ViewModel;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace GymManagmentSystem.BLL.Services.Booking.Classes;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ILogger<BookingService> _logger;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken ct = default)
    {
        try
        {
            var booking = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory();

            var filtered = booking.Where(s => s.EndDate >= DateTime.Now);

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(filtered).ToList();

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsnc(session.Id);
            }

            return Result<IEnumerable<SessionViewModel>>.OK(mappedSessions);
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "GetAllSessions (booking) threw an unexpected exception");
            return Result<IEnumerable<SessionViewModel>>.Fail("Internl Servver Error");
        }
    }

    public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForUpCommingSessionByIdAsync(int sessionId, CancellationToken ct = default)
    {
        var bookings = await _unitOfWork.BookingRepository.GetSessionByIdAsync(sessionId , ct);

        return _mapper.Map<IEnumerable<MemberForSessionViewModel>>(bookings);
    }

    public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForOnGoingSessionByIdAsync(int sessionId, CancellationToken ct = default)
    {
        var bookings = await _unitOfWork.BookingRepository.GetSessionByIdAsync(sessionId, ct);

        return _mapper.Map<IEnumerable<MemberForSessionViewModel>>(bookings);
    }

    public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel createBookingViewModel, CancellationToken ct = default)
    {
        try
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(createBookingViewModel.SessionId, ct);

            if (session is null)
            {
                _logger.LogWarning("CreateBooking failed: session {SessionId} not found", createBookingViewModel.SessionId);
                return Result.NotFound("Session Not Found");
            }

            if (session.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("CreateBooking failed: session {SessionId} already started", createBookingViewModel.SessionId);
                return Result.Fail("Session Started, Can not be booked");
            }

            var hasActtiveMebership = await _unitOfWork.MemberShipRepository.AnyAsync(m => m.MemberId == createBookingViewModel.MemberId && m.EndDate > DateTime.Now, ct);

            if (!hasActtiveMebership)
            {
                _logger.LogWarning("CreateBooking failed: member {MemberId} has no active membership", createBookingViewModel.MemberId);
                return Result.Fail("Member Does not hve an Active Membership");
            }

            var isAlreadyBooked = await _unitOfWork.BookingRepository.AnyAsync(b => b.SessionId == createBookingViewModel.SessionId && b.MemberId == createBookingViewModel.MemberId, ct);

            if (isAlreadyBooked)
            {
                _logger.LogWarning("CreateBooking failed: member {MemberId} already booked session {SessionId}", createBookingViewModel.MemberId, createBookingViewModel.SessionId);
                return Result.Fail("Session Already Booked");
            }

            var bookedSessions = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsnc(createBookingViewModel.SessionId);

            if (bookedSessions >= session.Capacity)
            {
                _logger.LogWarning("CreateBooking failed: session {SessionId} is full", createBookingViewModel.SessionId);
                return Result.Fail("Session is Full");
            }

            var memberSession = _mapper.Map<MemberSession>(createBookingViewModel);
            await _unitOfWork.BookingRepository.AddAsync(memberSession);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Member {MemberId} booked session {SessionId} successfully", createBookingViewModel.MemberId, createBookingViewModel.SessionId);
                return Result.OK();
            }
            else
            {
                _logger.LogError("CreateBooking failed for member {MemberId}, session {SessionId}: SaveChanges returned 0 rows", createBookingViewModel.MemberId, createBookingViewModel.SessionId);
                return Result.Fail("Failed To Book Session");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("CreateBooking was cancelled for session {SessionId}", createBookingViewModel?.SessionId);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropdownViewModelAsync(int sessionId, CancellationToken ct = default)
    {
        var bookings = await _unitOfWork.BookingRepository.GetAllAsync(s => s.SessionId == sessionId);

        var bookedMembers = bookings.Select(x => x.MemberId).ToList();

        var availableMembers = await _unitOfWork.GetRepository<Member>()
            .GetAllAsync(m => !bookedMembers.Contains(m.Id));

        return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
    }
    public async Task<Result> MarkAsAttend(int memberId, int sessionId, CancellationToken ct = default)
    {
        try
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct);

            if (booking is null)
            {
                _logger.LogWarning("MarkAsAttend failed: booking not found for member {MemberId}, session {SessionId}", memberId, sessionId);
                return Result.NotFound("Booking Not Found");
            }

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookingRepository.UpdateAsync(booking);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Member {MemberId} marked as attended in session {SessionId}", memberId, sessionId);
                return Result.OK();
            }
            else
            {
                _logger.LogError("MarkAsAttend failed for member {MemberId}, session {SessionId}: SaveChanges returned 0 rows", memberId, sessionId);
                return Result.Fail("Failed To Book Mark As Attend");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("MarkAsAttend was cancelled for member {MemberId}, session {SessionId}", memberId, sessionId);
            return Result.Fail("Operation Cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MarkAsAttend threw an unexpected exception for member {MemberId}, session {SessionId}", memberId, sessionId);
            return Result.Fail("Internal Server Error");
        }
    }

    public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
    {
       try
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            if (session is null)
            {
                _logger.LogWarning("CancelBooking failed: session {SessionId} not found", sessionId);
                return Result.NotFound("Session Not Found");
            }

            if (session.StartDate <= DateTime.Now)
            {
                _logger.LogWarning("CancelBooking failed: session {SessionId} already started", sessionId);
                return Result.Fail("Session Started, Can not cancel booking");
            }

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct);

            if (booking is null)
            {
                _logger.LogWarning("CancelBooking failed: booking not found for member {MemberId}, session {SessionId}", memberId, sessionId);
                return Result.NotFound("Booking Not Found");
            }

            await _unitOfWork.BookingRepository.DeleteAsync(booking);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Booking of member {MemberId} in session {SessionId} cancelled successfully", memberId, sessionId);
                return Result.OK();
            }
            else
            {
                _logger.LogError("CancelBooking failed for member {MemberId}, session {SessionId}: SaveChanges returned 0 rows", memberId, sessionId);
                return Result.Fail("Failed To Delete Booking  ");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("CancelBooking was cancelled for member {MemberId}, session {SessionId}", memberId, sessionId);
            return Result.Fail("Operation Cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CancelBooking threw an unexpected exception for member {MemberId}, session {SessionId}", memberId, sessionId);
            return Result.Fail("Internal Server Error");
        }
    }
}
