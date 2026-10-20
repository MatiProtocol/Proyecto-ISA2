using PharmaGo.Domain.Entities;

namespace PharmaGo.IBusinessLogic;

public interface IBookingManager
{
    public Booking Create(Booking bookingRequest, bool hasPrescriptionAll);
    public Booking GetBooking(int bookingId);
    public List<Booking> GetAll(string? email);
    public Booking PickUpBooking(int bookingId);
    public string CreateChallenge(int bookingId);
    public string ConfirmChallenge(int bookingId, string challenge, string signature);
    public List<Booking> GetAll();
    public List<Booking> GetByPharmacyId(int id);
    public bool ValidateEmployee(string name, int pharmacyId);
    public bool ValidateBookingEmployee(int pharmacyId, string name, int bookingId, bool cancel);
}