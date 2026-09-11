// PROMPT v2.2: Типізований статус бронювання

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Статус життєвого циклу бронювання.
/// </summary>
public enum BookingStatus
{
  Booked = 1,
  Cancelled = 2,
}