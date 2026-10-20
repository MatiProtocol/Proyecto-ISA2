using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NSec.Cryptography;
using PharmaGo.BusinessLogic;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.Test.BusinessLogic.Test;

[TestClass]
public class BookingManagerTests
{
    private Mock<IRepository<Pharmacy>> _pharmacysRepository = null!;
    private Mock<IRepository<Booking>> _bookingsRepository = null!;
    private MemoryCache _memoryCacheObject = null!;
    private Mock<IRepository<User>> _usersRepository = null!;
    private BookingManager _bookingManager = null!;
    private Booking _bookingRequest = null!;
    private BookingDetail _bookingDetail = null!;
    private Pharmacy _pharmacy = null!;
    private Booking _booking = null!;
    private bool _hasPrescriptionAll;
    private string _bookingDate;
    private List<Booking>? _bookings;
    private Booking? _otherBooking;
    private int _bookingId;
    private readonly string _validChallenge = "123:1700000000:abcdef12";
    private string _validSignature = Convert.ToBase64String(new byte[64]);
    private Mock<ILogManager> _logManager;

    [TestInitialize]
    public void Initialize()
    {
        _pharmacy = new Pharmacy { Id = 1, Name = "Pharmacy Ala" };
        _pharmacysRepository = new Mock<IRepository<Pharmacy>>();
        _bookingsRepository = new Mock<IRepository<Booking>>();
        _usersRepository = new Mock<IRepository<User>>();
        _memoryCacheObject = new MemoryCache(new MemoryCacheOptions());

        var algorithm = SignatureAlgorithm.Ed25519;
        using var key = new Key(algorithm, new KeyCreationParameters());
        var publicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
        var challengeBytes = Encoding.UTF8.GetBytes(_validChallenge);
        var signature = algorithm.Sign(key, challengeBytes);
        var validPublicKey = Convert.ToBase64String(publicKey);
        _validSignature = Convert.ToBase64String(signature);

        const string bookerEmail = "client@gmail.com";
        _bookingDate = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        _hasPrescriptionAll = true;
        _bookingId = 1;

        _bookingDetail = new BookingDetail
        {
            Id = 1,
            Name = "Perifar",
            Code = "PERIFAR",
            PharmacyName = "Pharmacy Ala",
            Quantity = 1,
            PharmacyId = 1,
            PrescriptionImage = null
        };

        _pharmacy.Drugs = new List<Drug>();
        _pharmacy.Drugs.Add(new Drug { Code = "PERIFAR", Name = "Perifar", Deleted = false, Quantity = 1, Stock = 1, Price = 10 });

        _logManager = new Mock<ILogManager>();
        _bookingManager = new BookingManager(_pharmacysRepository.Object, _bookingsRepository.Object, _memoryCacheObject, _usersRepository.Object, _logManager.Object);

        _bookingRequest = new Booking()
        {
            Id = 1,
            BookerEmail = bookerEmail,
            PublicKey = validPublicKey,
            BookingDate = _bookingDate,
            Status = BookingStatus.Pending,
            Details = new List<BookingDetail> { _bookingDetail }
        };

        _booking = new Booking { Id = _bookingId, Status = BookingStatus.Confirmed, PublicKey = validPublicKey };

        _otherBooking = new Booking
        {
            Id = 2,
            BookerEmail = "otherClient@gmail.com",
            BookingDate = _bookingRequest.BookingDate,
            ConfirmationDate = "",
            ClosureDate = "",
            TotalAmount = 100,
            PublicKey = "M1Z0mNpL2gV9vR0qZ1bC3dE4fF0hD7kL8zX9cV0bOlQ",
            Status = BookingStatus.Pending,
            Details = new List<BookingDetail>
            {
                new BookingDetail
                {
                    Id = 2,
                    Code = "KALITRON",
                    Name = "Kalitron",
                    Quantity = 1,
                    Price = 100,
                    PharmacyId = 1,
                    PharmacyName = "Pharmacy Ala",
                    PrescriptionImage = null
                }
            }
        };

        _bookings = new List<Booking> { _bookingRequest, _otherBooking };

        _pharmacysRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Pharmacy, bool>>>())).Returns(_pharmacy);
        _usersRepository
            .Setup(s => s.Exists(usr =>
                usr.UserName == "joan" && usr.Pharmacy.Id != null && usr.Pharmacy.Id == 1))
            .Returns(true); _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);
        _memoryCacheObject.Set($"challenge_{_bookingId}", _validChallenge, TimeSpan.FromMinutes(5));
        _bookingsRepository.Setup(r => r.GetAllByExpression(b => true)).Returns(_bookings);
    }

    [TestMethod]
    public void Create_ReturnsCorrectType()
    {
        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.IsInstanceOfType(response, typeof(Booking));
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithVoidConfirmationDateAndClosureDate()
    {
        var expectedDate = "";

        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.AreEqual(response.ConfirmationDate, expectedDate);
        Assert.AreEqual(response.ClosureDate, expectedDate);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithCorrectPublicKey()
    {
        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.AreEqual(response.PublicKey, _bookingRequest.PublicKey);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithPendingStatus_WhenHasPrescriptionAllIsTrue()
    {
        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.AreEqual(response.Status, BookingStatus.Pending);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithPendingValidationStatus_WhenHasPrescriptionAllIsFalse()
    {
        const bool notHasAllPrescriptions = false;
        var response = _bookingManager.Create(_bookingRequest, notHasAllPrescriptions);

        Assert.AreEqual(response.Status, BookingStatus.PendingValidation);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithValidBookingDate()
    {
        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        var responseRequestDate = DateTimeOffset.Parse(response.BookingDate);
        var actualDate = DateTimeOffset.Now;

        Assert.IsTrue(responseRequestDate < actualDate);
        Assert.AreEqual(response.BookingDate, _bookingDate);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WithFutureBookingDateSetsBookingDateNow()
    {
        var futureDate = DateTimeOffset.Now.AddHours(1);
        _bookingRequest.BookingDate = futureDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var response = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        var responseDate = DateTimeOffset.Parse(response.BookingDate);
        var actualDate = DateTimeOffset.Now;

        Assert.IsTrue(futureDate > actualDate);
        Assert.IsTrue(responseDate <= actualDate);
    }

    [TestMethod]
    public void Create_Booking_FailEmptyEmail()
    {
        _bookingRequest.BookerEmail = "";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual("Invalid Email", message);
    }

    [TestMethod]
    public void Create_Booking_FailInvalidEmail()
    {
        _bookingRequest.BookerEmail = "carlos@@gmail.com";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual("Invalid Email", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenDrugsIsEmpty()
    {
        _bookingRequest.Details = new List<BookingDetail>();

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual("The list of drugs can't be empty", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenDrugsIsNull()
    {
        _bookingRequest.Details = null;

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual("The list of drugs can't be empty", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenPharmacyIsNotFound()
    {
        _pharmacysRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Pharmacy, bool>>>())).Returns((Pharmacy)null);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"Pharmacy {_bookingDetail.PharmacyId} not found", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenPharmacyIdIsInvalid()
    {
        const int invalidId = 0;

        _bookingRequest.Details[0].PharmacyId = invalidId;

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"Pharmacy Id is a mandatory field", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenDetailQuantityIsLowerThan1()
    {
        _bookingRequest.Details[0].Quantity = 0;
        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"Quantity must be greater than 0", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenDrugIsNotFound()
    {
        _pharmacy.Drugs = new List<Drug>();
        _pharmacysRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Pharmacy, bool>>>())).Returns(_pharmacy);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"Drug {_bookingRequest.Details[0].Code} not found in Pharmacy {_pharmacy.Name}", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenDrugQuantityIsGraterThanPharmacyStock()
    {

        _pharmacy.Drugs.ToArray()[0].Stock = 0;
        _pharmacysRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Pharmacy, bool>>>())).Returns(_pharmacy);

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"The selected Quantity is more than available", message);
    }

    [TestMethod]
    public void Create_Booking_CallsInsertOne()
    {
        _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        _bookingsRepository!.Verify(m => m.InsertOne(It.IsAny<Booking>()), Times.Once);
    }

    [TestMethod]
    public void Create_Booking_CallsSave()
    {
        _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        _bookingsRepository!.Verify(m => m.Save(), Times.Once);
    }

    [TestMethod]
    public void Create_Booking_FailWhenPublicKeyIsNull()
    {
        _bookingRequest.PublicKey = "";

        var message = Assert
                    .ThrowsException<InvalidResourceException>(() =>
                        _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
                    .Message;

        Assert.AreEqual($"The public key must not be null", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenPublicKeyLengthIsDifferentThan44()
    {
        _bookingRequest.PublicKey = "WJNDA9iIgQ7z6rwmjR2j4rFjnYkTIiygs944ltdYqlQ";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll))
            .Message;

        Assert.AreEqual($"The public key must be 44 characters", message);
    }

    [TestMethod]
    public void Create_Booking_FailWhenBookingDateIsInvalid()
    {
        _bookingRequest.BookingDate = "Lalo Curanza";

        var message = Assert
            .ThrowsException<FormatException>(() =>
                _bookingManager.Create(_bookingRequest, _hasPrescriptionAll)).Message;
        Assert.AreEqual("The booking date must has the format 'yyyy-MM-ddTHH:mm:ss.fffZ'", message);
    }

    [TestMethod]
    public void Create_Booking_SetDrugPrice()
    {
        var booking = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.IsNotNull(booking.Details[0].Price);
        Assert.IsInstanceOfType(booking.Details[0].Price, typeof(int));
    }

    [TestMethod]
    public void Create_Booking_SetBookingTotalAmount()
    {
        var booking = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.AreEqual(booking.TotalAmount, _pharmacy.Drugs.Sum(d => d.Price * d.Quantity));
        Assert.IsNotNull(booking.TotalAmount);
        Assert.IsInstanceOfType(booking.TotalAmount, typeof(int));
    }

    [TestMethod]
    public void Create_Booking_SetNamesCorrectly()
    {
        var booking = _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);

        Assert.AreEqual(booking.Details[0].Name, _bookingDetail.Name);
        Assert.AreEqual(booking.Details[0].PharmacyName, _pharmacy.Name);
    }

    [TestMethod]
    public void GetAll_Booking_ReturnsCorrectType()
    {
        var response = _bookingManager.GetAll();
        Assert.IsInstanceOfType(response, typeof(IEnumerable<Booking>));
    }

    [TestMethod]
    public void GetAll_Booking_ReturnsCorrectBooking()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(new List<Booking> { _bookingRequest });
        _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);
        var response = _bookingManager.GetAll();
        Assert.AreEqual(response.Count(), 1);
    }

    [TestMethod]
    public void GetAllByPharmacyId_Booking_ReturnsCorrectType()
    {
        var response = _bookingManager.GetByPharmacyId(1);
        Assert.IsInstanceOfType(response, typeof(IEnumerable<Booking>));
    }

    [TestMethod]
    public void GetAllByPharmacyId_Booking_ReturnsCorrectBooking()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        _bookingManager.Create(_bookingRequest, _hasPrescriptionAll);
        var response = _bookingManager.GetByPharmacyId(1);
        Assert.AreEqual(response.Count(), 1);
        Assert.AreEqual(response.First().Details.First().PharmacyName, "Pharmacy Ala");
    }

    [TestMethod]
    public void ValidateEmployee_ReturnsTrue()
    {
        var response = _bookingManager.ValidateEmployee("joan", 1);

        Assert.IsTrue(response);
    }

    [TestMethod]
    public void ValidateEmployee_ReturnsFalse()
    {
        var response = _bookingManager.ValidateEmployee("joana", 1);

        Assert.IsFalse(response);
    }

    [TestMethod]
    public void ValidateBookingEmployee_Correctly()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        var response = _bookingManager.ValidateBookingEmployee(1, "joan", 1, false);
        Assert.IsTrue(response);
    }

    [TestMethod]
    public void ValidateBookingEmployee_WrongName()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        var response = _bookingManager.ValidateBookingEmployee(1, "joana", 1, false);
        Assert.IsFalse(response);
    }

    [TestMethod]
    public void ValidateBookingEmployee_EmptyBookingList()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking>());
        var response = _bookingManager.ValidateBookingEmployee(1, "joana", 1, false);
        Assert.IsFalse(response);
    }
    [TestMethod]
    public void ValidateBookingEmployee_StatusDifferentThanPending()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        _bookingRequest.Status = BookingStatus.Confirmed;
        var response = _bookingManager.ValidateBookingEmployee(1, "joan", 1, false);
        Assert.IsFalse(response);
    }
    [TestMethod]
    public void ValidateBookingEmployee_WrongBookingId()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        var response = _bookingManager.ValidateBookingEmployee(1, "joan", 10, false);
        Assert.IsFalse(response);
    }

    [TestMethod]
    public void ValidateBookingEmployee_StatusCanceled()
    {
        _bookingsRepository.Setup(s => s.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(new List<Booking> { _bookingRequest });
        _bookingRequest.Status = BookingStatus.Pending;
        var response = _bookingManager.ValidateBookingEmployee(1, "joan", 1, true);
        Assert.IsTrue(response);
    }

    [TestMethod]
    public void GetALl_WithoutEmail_ReturnsCorrectType()
    {
        var response = _bookingManager.GetAll(null);

        Assert.IsInstanceOfType(response, typeof(List<Booking>));
    }

    [TestMethod]
    public void GetAll_WithoutEmail_CallsBookingRepository()
    {
        _bookingManager.GetAll(null);

        _bookingsRepository!.Verify(r => r.GetAllByExpression(b => true), Times.Once);
    }

    [TestMethod]
    public void GetAll_WithoutEmail_ReturnsWithDetailsCorrectResult()
    {
        var response = _bookingManager!.GetAll(null);

        Assert.AreEqual(_bookings!.Count, response.Count);
        Assert.AreEqual(_bookings[0].BookerEmail, response[0].BookerEmail);
        Assert.AreEqual(_bookings[0].BookingDate, response[0].BookingDate);
        Assert.AreEqual(_bookings[0].ConfirmationDate, response[0].ConfirmationDate);
        Assert.AreEqual(_bookings[0].ClosureDate, response[0].ClosureDate);
        Assert.AreEqual(_bookings[0].TotalAmount, response[0].TotalAmount);
        Assert.AreEqual(_bookings[0].PublicKey, response[0].PublicKey);
        Assert.AreEqual(_bookings[0].Status, response[0].Status);
        Assert.AreEqual(_bookings[0].Details.Count, response[0].Details.Count);
        Assert.AreEqual(_bookings[0].Details[0].Id, response[0].Details[0].Id);
        Assert.AreEqual(_bookings[0].Details[0].Code, response[0].Details[0].Code);
        Assert.AreEqual(_bookings[0].Details[0].Name, response[0].Details[0].Name);
        Assert.AreEqual(_bookings[0].Details[0].Quantity, response[0].Details[0].Quantity);
        Assert.AreEqual(_bookings[0].Details[0].Price, response[0].Details[0].Price);
        Assert.AreEqual(_bookings[0].Details[0].PharmacyId, response[0].Details[0].PharmacyId);
        Assert.AreEqual(_bookings[0].Details[0].PharmacyName, response[0].Details[0].PharmacyName);
        Assert.AreEqual(_bookings[0].Details[0].PrescriptionImage, response[0].Details[0].PrescriptionImage);

        Assert.AreEqual(_bookings[1].BookerEmail, response[1].BookerEmail);
        Assert.AreEqual(_bookings[1].BookingDate, response[1].BookingDate);
        Assert.AreEqual(_bookings[1].ConfirmationDate, response[1].ConfirmationDate);
        Assert.AreEqual(_bookings[1].ClosureDate, response[1].ClosureDate);
        Assert.AreEqual(_bookings[1].TotalAmount, response[1].TotalAmount);
        Assert.AreEqual(_bookings[1].PublicKey, response[1].PublicKey);
        Assert.AreEqual(_bookings[1].Status, response[1].Status);
        Assert.AreEqual(_bookings[1].Details.Count, response[1].Details.Count);
        Assert.AreEqual(_bookings[1].Details[0].Id, response[1].Details[0].Id);
        Assert.AreEqual(_bookings[1].Details[0].Code, response[1].Details[0].Code);
        Assert.AreEqual(_bookings[1].Details[0].Name, response[1].Details[0].Name);
        Assert.AreEqual(_bookings[1].Details[0].Quantity, response[1].Details[0].Quantity);
        Assert.AreEqual(_bookings[1].Details[0].Price, response[1].Details[0].Price);
        Assert.AreEqual(_bookings[1].Details[0].PharmacyId, response[1].Details[0].PharmacyId);
        Assert.AreEqual(_bookings[1].Details[0].PharmacyName, response[1].Details[0].PharmacyName);
        Assert.AreEqual(_bookings[1].Details[0].PrescriptionImage, response[1].Details[0].PrescriptionImage);
    }

    [TestMethod]
    public void GetAll_WithEmail_ReturnsOnlyBookingsWithCorrectMail()
    {
        var clientEmail = _bookings[0].BookerEmail;
        var filteredBookings = _bookings.FindAll(b => b.BookerEmail == clientEmail);
        _bookingsRepository.Setup(r => r.GetAllByExpression(It.IsAny<Expression<Func<Booking, bool>>>()))
            .Returns(filteredBookings);

        var response = _bookingManager.GetAll(clientEmail);

        Assert.AreEqual(filteredBookings.Count, response.Count);
        Assert.AreEqual(filteredBookings[0].BookerEmail, response[0].BookerEmail);
        Assert.AreEqual(filteredBookings[0].PublicKey, response[0].PublicKey);
        Assert.AreEqual(filteredBookings[0].Details.Count, response[0].Details.Count);
        Assert.AreEqual(filteredBookings[0].Details[0].Id, response[0].Details[0].Id);
        Assert.AreEqual(filteredBookings[0].Details[0].Code, response[0].Details[0].Code);
    }

    [TestMethod]
    public void GetAll_WithEmptyEmail_ThrowsInvalidResourceException()
    {
        var invalidEmail = "";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.GetAll(invalidEmail))
            .Message;

        Assert.AreEqual("Invalid Email", message);
    }

    [TestMethod]
    public void GetAll_WithInvalidEmail_ThrowsInvalidResourceException()
    {
        var invalidEmail = "abc";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.GetAll(invalidEmail))
            .Message;

        Assert.AreEqual("Invalid Email", message);
    }

    [TestMethod]
    public void GetBooking_ReturnsCorrectType()
    {
        var response = _bookingManager.GetBooking(_bookingId);

        Assert.IsInstanceOfType(response, typeof(Booking));
    }

    [TestMethod]
    public void GetBooking_CallsGetOneByExpressionFromBookingRepository()
    {
        _bookingManager.GetBooking(_bookingId);

        _bookingsRepository.Verify(m => m.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void GetBooking_ReturnsCorrectBooking()
    {
        var bookingResponse = _bookingManager.GetBooking(_bookingId);

        Assert.AreEqual(_bookingId, bookingResponse.Id);
    }

    [TestMethod]
    public void GetBooking_FailWhenBookingIsNotFound()
    {
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(
            (Booking)null);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.GetBooking(_bookingId))
            .Message;

        Assert.AreEqual($"Booking {_bookingId} don't exist", message);
    }

    [TestMethod]
    public void CreateChallenge_ReturnsCorrectType()
    {
        var response = _bookingManager.CreateChallenge(_bookingId);

        Assert.IsInstanceOfType(response, typeof(string));
    }

    [TestMethod]
    public void GenerateChallenge_ReturnsChallengeWithCorrectFormat()
    {
        string challenge = _bookingManager.CreateChallenge(_bookingId);
        var parts = challenge.Split(':');

        const int expectedParts = 3;

        Assert.AreEqual(expectedParts, parts.Length);

        var obtainedBookingId = parts[0];
        var timeStamp = parts[1];
        var nonce = parts[2];

        var expectedCharactersNonce = 8;

        Assert.AreEqual(_bookingId.ToString(), obtainedBookingId);
        Assert.IsTrue(long.TryParse(timeStamp, out _));
        Assert.AreEqual(expectedCharactersNonce, nonce.Length);
    }

    [TestMethod]
    public void CreateChallenge_FailWhenBookingIsNotConfirmed()
    {
        _booking.Status = BookingStatus.Pending;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.CreateChallenge(_bookingId))
            .Message;

        Assert.AreEqual("The booking must be in confirmed status", message);
    }

    [TestMethod]
    public void GenerateChallenge_StorageChallengeInMemoryCache()
    {
        var challenge = _bookingManager.CreateChallenge(_bookingId);

        var cachedChallenge = "";
        var found = _memoryCacheObject.TryGetValue("challenge_1", out cachedChallenge);

        Assert.IsTrue(found);
        Assert.AreEqual(challenge, cachedChallenge);
    }

    [TestMethod]
    public void ConfirmChallenge_ReturnsCorrectType()
    {
        var response = _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature);

        Assert.IsInstanceOfType(response, typeof(string));
    }

    [TestMethod]
    public void ConfirmChallenge_ReturnsCorrectChallenge()
    {
        var response = _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature);

        Assert.AreEqual(response, _validChallenge);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenStorageChallengeIsNotFoundInMemoryCache()
    {
        _memoryCacheObject.Remove($"challenge_{_bookingId}");

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("No stored challenge was found for this booking", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenStorageChallengeAndRequestChallengeAreDifferent()
    {
        _memoryCacheObject.Remove($"challenge_{_bookingId}");
        var otherChallenge = "other challenge";
        _memoryCacheObject.Set($"challenge_{_bookingId}", otherChallenge, TimeSpan.FromMinutes(5));

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("The provided challenge does not match the stored challenge", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenBookingIsNotFound()
    {
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(
            (Booking)null);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual($"Booking {_bookingId} don't exist", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenBookingIsNotConfirmed()
    {
        _booking.Status = BookingStatus.Pending;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("The booking must be in confirmed status", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenPublicKeyIsNotInBase64()
    {
        var invalidPublicKey = "Public key";
        _booking.PublicKey = invalidPublicKey;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("The public key format is invalid", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenPublicKeyHasInvalidLength()
    {
        var invalidPublicKey = Convert.ToBase64String(new byte[16]);
        _booking.PublicKey = invalidPublicKey;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("The public key format is invalid", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenSignatureIsNotInBase64()
    {
        var invalidSignature = "signature";

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, invalidSignature))
            .Message;

        Assert.AreEqual("The signature format is invalid", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenSignatureHasInvalidLength()
    {
        var invalidSignature = Convert.ToBase64String(new byte[16]);

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, invalidSignature))
            .Message;

        Assert.AreEqual("The signature format is invalid", message);
    }

    [TestMethod]
    public void ConfirmChallenge_FailWhenVerificationFails()
    {
        var invalidPublicKey = "WJNDA9iIgQ7z6rwmjR2j4rFjnYkTIiygs944ltdYqlQ="; ;
        _booking.PublicKey = invalidPublicKey;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<InvalidResourceException>(() =>
                _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature))
            .Message;

        Assert.AreEqual("The provided private key is not valid for this reservation", message);
    }

    [TestMethod]
    public void ConfirmChallenge_RemoveChallengeFromMemoryCache()
    {
        _bookingManager.ConfirmChallenge(_bookingId, _validChallenge, _validSignature);

        var cachedChallenge = "";
        var found = _memoryCacheObject.TryGetValue($"challenge_{_bookingId}", out cachedChallenge);

        Assert.IsFalse(found);
    }

    [TestMethod]
    public void PickUpBooking_FailWhenBookingIsNotFound()
    {
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(
            (Booking)null);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.PickUpBooking(_bookingId))
            .Message;

        Assert.AreEqual($"Booking {_bookingId} don't exist", message);
    }

    [TestMethod]
    public void PickUpBooking_FailWhenBookingIsNotConfirmed()
    {
        _booking.Status = BookingStatus.Pending;
        _bookingsRepository.Setup(s => s.GetOneByExpression(It.IsAny<Expression<Func<Booking, bool>>>())).Returns(_booking);

        var message = Assert
            .ThrowsException<ResourceNotFoundException>(() =>
                _bookingManager.PickUpBooking(_bookingId))
            .Message;

        Assert.AreEqual("The booking must be in confirmed status", message);
    }

    [TestMethod]
    public void PickUpBooking_ReturnCorrectBooking()
    {
        var booking = _bookingManager.PickUpBooking(_bookingId);

        Assert.AreEqual(_bookingId, booking.Id);
    }

    [TestMethod]
    public void PickUpBooking_ReturnUpdatedBooking()
    {
        var booking = _bookingManager.PickUpBooking(_bookingId);

        var expectedStatus = BookingStatus.PickUp;

        Assert.AreEqual(expectedStatus, booking.Status);
        Assert.IsNotNull(booking.ClosureDate);
    }

    [TestMethod]
    public void PickUpBooking_CallsUpdateOneFromBookingRepository()
    {
        _bookingManager.PickUpBooking(_bookingId);

        _bookingsRepository.Verify(m => m.UpdateOne(It.IsAny<Booking>()), Times.Once);
    }

    [TestMethod]
    public void PickUpBooking_CallsSaveFromBookingRepository()
    {
        _bookingManager.PickUpBooking(_bookingId);

        _bookingsRepository.Verify(m => m.Save(), Times.Once);
    }
}