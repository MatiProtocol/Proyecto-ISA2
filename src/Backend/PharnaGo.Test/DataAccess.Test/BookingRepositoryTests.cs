using Microsoft.EntityFrameworkCore;
using Moq;
using PharmaGo.DataAccess;
using PharmaGo.DataAccess.Repositories;
using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.Test.DataAccess.Test;

[TestClass]
public class BookingRepositoryTests
{
    private PharmacyGoDbContext _context = null!;
    private BookingRepository _bookingRepository = null!;
    private List<Booking> _bookingsSaved = null!;
    private Booking _booking = null!;
    private const int _bookingId = 1;
    private Booking _otherBooking = null!;
    private Mock<ILogManager> _logManager;


    [TestInitialize]
    public void InitTest()
    {
        _logManager = new Mock<ILogManager>();
        BookingDetail bookingDetail = new BookingDetail { Code = "PERIFAR", Name = "Perifar", PharmacyName = "Pharmacy Ala" };
        _booking = new Booking
        {
            Id = _bookingId,
            BookerEmail = "test@test.com",
            BookingDate = DateTime.Now.ToString("dd/MM/yyyy"),
            PublicKey = "public key",
            Details = new List<BookingDetail> { bookingDetail }
        };

        _otherBooking = new Booking
        {
            Id = _bookingId + 1,
            BookerEmail = "othertest@test.com",
            BookingDate = DateTime.Now.ToString("dd/MM/yyyy"),
            PublicKey = "public key2",
            Details = new List<BookingDetail> { bookingDetail }
        };
        _bookingsSaved = new List<Booking> { _booking, _otherBooking };
    }

    [TestCleanup]
    public void CleanUp()
    {
        _context.Database.EnsureDeleted();
    }

    private void CreateDataBase(string name)
    {
        var options = new DbContextOptionsBuilder<PharmacyGoDbContext>().UseInMemoryDatabase(databaseName: name).Options;
        _context = new PharmacyGoDbContext(options);
        _bookingsSaved.ForEach(b => _context.Set<Booking>().Add(b));
        _context.Set<Booking>().Include("Details");
        _context.SaveChanges();
        _bookingRepository = new BookingRepository(_context, _logManager.Object);
    }

    [TestMethod]
    public void GetAllByExpressionOkType()
    {
        CreateDataBase("getBookingsTestDb");
        var retrievedBooking = _bookingRepository.GetAllByExpression(b => b.BookerEmail == _booking.BookerEmail);
        Assert.IsInstanceOfType(retrievedBooking, typeof(IEnumerable<Booking>));
    }

    [TestMethod]
    public void GetBookingFromExpressionOk()
    {
        CreateDataBase("getBookingsTestDb");
        var retrievedBooking = _bookingRepository.GetOneByExpression(b => b.Id == _bookingId);
        Assert.AreEqual(_bookingId, retrievedBooking.Id);
    }

    [TestMethod]
    public void GetBookingFromExpressionReturnBookingWithDetailOk()
    {
        CreateDataBase("getBookingsTestDb");
        var retrievedBooking = _bookingRepository.GetOneByExpression(b => b.Id == _bookingId);
        Assert.AreEqual(_booking.Details.Count, retrievedBooking.Details.Count);
    }

    [TestMethod]
    public void GetAllByExpressionOkDetails()
    {
        var filteredBookings = _bookingsSaved.FindAll(b => b.BookerEmail == _booking.BookerEmail);
        CreateDataBase("getBookingsTestDb");
        var retrievedBooking = _bookingRepository.GetAllByExpression(b => b.BookerEmail == _booking.BookerEmail).ToList();
        Assert.AreEqual(filteredBookings.Count, retrievedBooking.Count);
        Assert.AreEqual(filteredBookings[0].Details.Count, retrievedBooking[0].Details.Count);
    }
}