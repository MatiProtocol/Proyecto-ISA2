using Microsoft.AspNetCore.Mvc;
using Moq;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.IBusinessLogic;
using PharmaGo.WebApi.Controllers;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;

namespace PharmaGo.Test.WebApi.Test;

[TestClass]
public class BookingsControllerTests
{
    private BookingController _bookingController = null!;
    private BookingModelRequest _bookingRequest = null!;
    private int _bookingId;
    private string? _bookerEmail;
    private string? _bookingDate;
    private string? _bookingPublicKey;
    private const string _challenge = "Challenge";
    private Mock<IBookingManager> _mockBookingManager = null!;
    private Booking _booking = null!;
    private ChallengeModelRequest _challengeRequest = null!;
    private List<Booking>? _bookings;
    private Booking? _otherBooking;

    [TestInitialize]
    public void Initialize()
    {
        _mockBookingManager = new Mock<IBookingManager>();
        var signature = "Signature";
        _challengeRequest = new ChallengeModelRequest { Challenge = _challenge, Signature = signature };
        _bookerEmail = "client@gmail.com";
        _bookingDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        _bookingPublicKey = "U8X5mNpL2gY9vR0qW1bC3dE4fG6hJ7kL8zX9cV0bNnM";
        _bookingController = new BookingController(_mockBookingManager.Object);
        _bookingId = 1;

        _bookingRequest = new BookingModelRequest
        {
            BookerEmail = _bookerEmail,
            BookingDate = _bookingDate,
            PublicKey = _bookingPublicKey,
            HasPrescriptionAll = true,
            Details = new List<BookingModelRequestDetail>(){
                new BookingModelRequestDetail
                {
                    Id = 1,
                    Code = "PERIFAR",
                    Quantity = 1,
                    PharmacyId = 1,
                    PrescriptionImage = null
                }
            }
        };
        _booking = new Booking
        {
            Id = _bookingId,
            BookerEmail = _bookingRequest.BookerEmail,
            BookingDate = _bookingRequest.BookingDate,
            ConfirmationDate = "",
            ClosureDate = "",
            TotalAmount = 50,
            PublicKey = _bookingRequest.PublicKey,
            Status = BookingStatus.Pending,
            Details = new List<BookingDetail>
            {
                new BookingDetail
                {
                    Id = 1,
                    Code = "PERIFAR",
                    Name = "Perifar",
                    Quantity = 1,
                    Price = 50,
                    PharmacyId = 1,
                    PharmacyName = "Pharmacy Ala",
                    PrescriptionImage = null
                }
            }
        };

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

        _bookings = new List<Booking> { _booking, _otherBooking };

        _mockBookingManager!.Setup(m => m.Create(It.IsAny<Booking>(), _bookingRequest.HasPrescriptionAll))
            .Returns(_booking!);
        _mockBookingManager!.Setup(m => m.GetAll()).Returns(new List<Booking> { _booking });

        _mockBookingManager!.Setup(m => m.GetByPharmacyId(1)).Returns(new List<Booking> { _booking });
        _mockBookingManager!.Setup(m => m.GetByPharmacyId(0)).Returns(new List<Booking>());
        _mockBookingManager!.Setup(m => m.ValidateBookingEmployee(1, "mari.gonzale", 1, false)).Returns(true);
        _mockBookingManager!.Setup(m => m.ValidateBookingEmployee(0, "mari.gonzale", 1, false)).Returns(false);
        _mockBookingManager!.Setup(m => m.ValidateBookingEmployee(1, "Jonata", 1, false)).Returns(false);
        _mockBookingManager!.Setup(m => m.ValidateBookingEmployee(1, "mari.gonzale", 5, false)).Returns(false);

        _mockBookingManager!.Setup(m => m.ValidateBookingEmployee(1, "mari.gonzale", 1, true)).Returns(true);

        _mockBookingManager!.Setup(m => m.GetAll(null)).Returns(_bookings!);
        _mockBookingManager!.Setup(m => m.GetAll(_bookerEmail)).Returns(new List<Booking> { _booking });
        _mockBookingManager!.Setup(m => m.GetBooking(_bookingId)).Returns(_booking);
        _mockBookingManager.Setup(m => m.CreateChallenge(_bookingId)).Returns(_challenge);
        _mockBookingManager.Setup(m => m.ConfirmChallenge(_bookingId, _challenge, signature)).Returns(_challenge);
        _booking.Status = BookingStatus.PickUp;
        _mockBookingManager.Setup(m => m.PickUpBooking(_bookingId)).Returns(_booking);
    }

    [TestMethod]
    public void Create_Booking_CreatedCorrectResult()
    {
        var response = _bookingController.CreateBooking(_bookingRequest);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
        var createResult = response as CreatedResult;
        var createResultValue = createResult!.Value as BookingModelResponse;

        Assert.AreEqual(_bookerEmail, createResultValue.BookerEmail);
        Assert.AreEqual(_bookingDate, createResultValue.BookingDate);
    }

    [TestMethod]
    public void Create_Booking_CallsBookingManager()
    {
        _bookingController!.CreateBooking(_bookingRequest!);

        _mockBookingManager!.Verify(m => m.Create(It.IsAny<Booking>(), _bookingRequest!.HasPrescriptionAll),
            Times.Once);
    }

    [TestMethod]
    public void Create_Booking_CreatedWithDetailsCorrectResult()
    {
        var response = _bookingController!.CreateBooking(_bookingRequest!);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
        var createResult = response as CreatedResult;
        var createResultValue = createResult!.Value as BookingModelResponse;

        Assert.AreEqual(_booking!.Id, createResultValue!.Id);
        Assert.AreEqual(_bookingRequest!.BookerEmail, createResultValue!.BookerEmail);
        Assert.AreEqual(_bookingRequest.BookingDate, createResultValue.BookingDate);
        Assert.AreEqual(_booking!.ConfirmationDate, createResultValue!.ConfirmationDate);
        Assert.AreEqual(_booking!.ClosureDate, createResultValue!.ClosureDate);
        Assert.AreEqual(_booking!.TotalAmount, createResultValue.TotalAmount);
        Assert.AreEqual(_bookingRequest.PublicKey, createResultValue.PublicKey);
        Assert.AreEqual(_booking!.Status.ToString(), createResultValue.Status);
        Assert.AreEqual(_bookingRequest.Details.Count, createResultValue.Details.Count);
        Assert.AreEqual(_bookingRequest.Details[0].Id, createResultValue.Details[0].Id);
        Assert.AreEqual(_bookingRequest.Details[0].Code, createResultValue.Details[0].Code);
        Assert.AreEqual(_booking.Details[0].Name, createResultValue.Details[0].Name);
        Assert.AreEqual(_booking.Details[0].Quantity, createResultValue.Details[0].Quantity);
        Assert.AreEqual(_booking.Details[0].Price, createResultValue.Details[0].Price);
        Assert.AreEqual(_bookingRequest.Details[0].PharmacyId, createResultValue.Details[0].PharmacyId);
        Assert.AreEqual(_booking.Details[0].PharmacyName, createResultValue.Details[0].PharmacyName);
        Assert.AreEqual(_booking.Details[0].PrescriptionImage, createResultValue.Details[0].PrescriptionImage);
    }

    [TestMethod]
    public void Get_Booking_GetWithPharmacyId()
    {
        var response = _bookingController!.GetBookingByPharmacy(1);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as List<BookingModelResponse>;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Count, 1);
    }

    [TestMethod]
    public void Get_Booking_GetWithNoQuery()
    {

        var response = _bookingController.GetBookingByPharmacy(null);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as List<BookingModelResponse>;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Count, 1);
    }

    [TestMethod]
    public void Post_ApproveBooking_Ok()
    {
        var bookingEmployeRequest = new BookingEmployeeRequest()
        {
            PharmacyId = 1,
            ApproverName = "mari.gonzale",
            BookingId = 1,
            Cancel = false
        };
        var response = _bookingController.ApproveBooking(1, bookingEmployeRequest);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as BookingEmployeeResponse;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Success, true);
    }
    [TestMethod]
    public void Post_ApproveBooking_EmptyBooking()
    {
        var bookingEmployeRequest = new BookingEmployeeRequest()
        {
            PharmacyId = 0,
            ApproverName = "mari.gonzale",
            BookingId = 1,
            Cancel = false
        };
        var response = _bookingController.ApproveBooking(0, bookingEmployeRequest);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as BookingEmployeeResponse;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Success, false);
    }

    [TestMethod]
    public void Post_ApproveBooking_WrongEmployee()
    {
        var bookingEmployeRequest = new BookingEmployeeRequest()
        {
            PharmacyId = 0,
            ApproverName = "Jonata",
            BookingId = 1,
            Cancel = false
        };
        var response = _bookingController.ApproveBooking(0, bookingEmployeRequest);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as BookingEmployeeResponse;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Success, false);
    }
    [TestMethod]
    public void Post_ApproveBooking_WrongBookingId()
    {
        var bookingEmployeRequest = new BookingEmployeeRequest()
        {
            PharmacyId = 1,
            ApproverName = "mari.gonzale",
            BookingId = 5,
            Cancel = false
        };
        var response = _bookingController.ApproveBooking(1, bookingEmployeRequest);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as BookingEmployeeResponse;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Success, false);
    }

    [TestMethod]
    public void Post_CancelBooking_Ok()
    {
        var bookingEmployeRequest = new BookingEmployeeRequest()
        {
            PharmacyId = 1,
            ApproverName = "mari.gonzale",
            BookingId = 1,
            Cancel = true
        };
        var response = _bookingController.CancelBooking(1, bookingEmployeRequest);
        var okResult = response as OkObjectResult;
        var okResultValue = okResult.Value as BookingEmployeeResponse;

        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.AreEqual(okResultValue.Success, true);
    }

    [TestMethod]
    public void GetAllBookings_WithoutEmail_GivesCorrectType()
    {
        var response = _bookingController.GetAllBookings(null);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getAllResult = response as ObjectResult;
        var getAllResultValue = getAllResult!.Value as List<BookingModelResponse>;

        Assert.IsInstanceOfType(getAllResultValue, typeof(List<BookingModelResponse>));
    }

    [TestMethod]
    public void GetAllBookings_WithoutEmail_CallsBookingManager()
    {
        _bookingController.GetAllBookings(null);

        _mockBookingManager!.Verify(m => m.GetAll(null), Times.Once);
    }

    [TestMethod]
    public void GetAllBookings_WithoutEmail_ReturnsWithDetailsCorrectResult()
    {
        var response = _bookingController!.GetAllBookings(null);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getAllResult = response as OkObjectResult;
        var getAllResultValue = getAllResult!.Value as List<BookingModelResponse>;

        Assert.AreEqual(_bookings!.Count, getAllResultValue.Count);
        Assert.AreEqual(_bookings[0].BookerEmail, getAllResultValue[0].BookerEmail);
        Assert.AreEqual(_bookings[0].BookingDate, getAllResultValue[0].BookingDate);
        Assert.AreEqual(_bookings[0].ConfirmationDate, getAllResultValue[0].ConfirmationDate);
        Assert.AreEqual(_bookings[0].ClosureDate, getAllResultValue[0].ClosureDate);
        Assert.AreEqual(_bookings[0].TotalAmount, getAllResultValue[0].TotalAmount);
        Assert.AreEqual(_bookings[0].PublicKey, getAllResultValue[0].PublicKey);
        Assert.AreEqual(_bookings[0].Status.ToString(), getAllResultValue[0].Status);
        Assert.AreEqual(_bookings[0].Details.Count, getAllResultValue[0].Details.Count);
        Assert.AreEqual(_bookings[0].Details[0].Id, getAllResultValue[0].Details[0].Id);
        Assert.AreEqual(_bookings[0].Details[0].Code, getAllResultValue[0].Details[0].Code);
        Assert.AreEqual(_bookings[0].Details[0].Name, getAllResultValue[0].Details[0].Name);
        Assert.AreEqual(_bookings[0].Details[0].Quantity, getAllResultValue[0].Details[0].Quantity);
        Assert.AreEqual(_bookings[0].Details[0].Price, getAllResultValue[0].Details[0].Price);
        Assert.AreEqual(_bookings[0].Details[0].PharmacyId, getAllResultValue[0].Details[0].PharmacyId);
        Assert.AreEqual(_bookings[0].Details[0].PharmacyName, getAllResultValue[0].Details[0].PharmacyName);
        Assert.AreEqual(_bookings[0].Details[0].PrescriptionImage, getAllResultValue[0].Details[0].PrescriptionImage);

        Assert.AreEqual(_bookings[1].BookerEmail, getAllResultValue[1].BookerEmail);
        Assert.AreEqual(_bookings[1].BookingDate, getAllResultValue[1].BookingDate);
        Assert.AreEqual(_bookings[1].ConfirmationDate, getAllResultValue[1].ConfirmationDate);
        Assert.AreEqual(_bookings[1].ClosureDate, getAllResultValue[1].ClosureDate);
        Assert.AreEqual(_bookings[1].TotalAmount, getAllResultValue[1].TotalAmount);
        Assert.AreEqual(_bookings[1].PublicKey, getAllResultValue[1].PublicKey);
        Assert.AreEqual(_bookings[1].Status.ToString(), getAllResultValue[1].Status);
        Assert.AreEqual(_bookings[1].Details.Count, getAllResultValue[1].Details.Count);
        Assert.AreEqual(_bookings[1].Details[0].Id, getAllResultValue[1].Details[0].Id);
        Assert.AreEqual(_bookings[1].Details[0].Code, getAllResultValue[1].Details[0].Code);
        Assert.AreEqual(_bookings[1].Details[0].Name, getAllResultValue[1].Details[0].Name);
        Assert.AreEqual(_bookings[1].Details[0].Quantity, getAllResultValue[1].Details[0].Quantity);
        Assert.AreEqual(_bookings[1].Details[0].Price, getAllResultValue[1].Details[0].Price);
        Assert.AreEqual(_bookings[1].Details[0].PharmacyId, getAllResultValue[1].Details[0].PharmacyId);
        Assert.AreEqual(_bookings[1].Details[0].PharmacyName, getAllResultValue[1].Details[0].PharmacyName);
        Assert.AreEqual(_bookings[1].Details[0].PrescriptionImage, getAllResultValue[1].Details[0].PrescriptionImage);
    }

    [TestMethod]
    public void GetAllBookings_WithEmail_ReturnsOnlyBookingsWithCorrectMail()
    {
        var response = _bookingController!.GetAllBookings(_bookerEmail);

        var filteredBookings = _bookings.FindAll(b => b.BookerEmail == _bookerEmail);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getAllResult = response as OkObjectResult;
        var getAllResultValue = getAllResult!.Value as List<BookingModelResponse>;

        Assert.AreEqual(filteredBookings.Count, getAllResultValue.Count);
        Assert.AreEqual(filteredBookings[0].BookerEmail, getAllResultValue[0].BookerEmail);
        Assert.AreEqual(filteredBookings[0].BookingDate, getAllResultValue[0].BookingDate);
        Assert.AreEqual(filteredBookings[0].ConfirmationDate, getAllResultValue[0].ConfirmationDate);
        Assert.AreEqual(filteredBookings[0].ClosureDate, getAllResultValue[0].ClosureDate);
        Assert.AreEqual(filteredBookings[0].TotalAmount, getAllResultValue[0].TotalAmount);
        Assert.AreEqual(filteredBookings[0].PublicKey, getAllResultValue[0].PublicKey);
        Assert.AreEqual(filteredBookings[0].Status.ToString(), getAllResultValue[0].Status);
        Assert.AreEqual(filteredBookings[0].Details.Count, getAllResultValue[0].Details.Count);
        Assert.AreEqual(filteredBookings[0].Details[0].Id, getAllResultValue[0].Details[0].Id);
        Assert.AreEqual(filteredBookings[0].Details[0].Code, getAllResultValue[0].Details[0].Code);
        Assert.AreEqual(filteredBookings[0].Details[0].Name, getAllResultValue[0].Details[0].Name);
        Assert.AreEqual(filteredBookings[0].Details[0].Quantity, getAllResultValue[0].Details[0].Quantity);
        Assert.AreEqual(filteredBookings[0].Details[0].Price, getAllResultValue[0].Details[0].Price);
        Assert.AreEqual(filteredBookings[0].Details[0].PharmacyId, getAllResultValue[0].Details[0].PharmacyId);
        Assert.AreEqual(filteredBookings[0].Details[0].PharmacyName, getAllResultValue[0].Details[0].PharmacyName);
        Assert.AreEqual(filteredBookings[0].Details[0].PrescriptionImage, getAllResultValue[0].Details[0].PrescriptionImage);
    }

    [TestMethod]
    public void GetBooking_ReturnOK_WithValidBookingId()
    {
        var response = _bookingController.GetBooking(_booking.Id);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var okResponse = response as OkObjectResult;
        Assert.IsInstanceOfType(okResponse!.Value, typeof(BookingModelResponse));
    }

    [TestMethod]
    public void GetBooking_ReturnBookingCorrectResult()
    {
        var response = _bookingController.GetBooking(_booking.Id);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getResult = response as OkObjectResult;
        var getResultValue = getResult!.Value as BookingModelResponse;

        Assert.AreEqual(_bookingId, getResultValue!.Id);
    }

    [TestMethod]
    public void GetBooking_CallsBookingManager()
    {
        _bookingController.GetBooking(_bookingId);

        _mockBookingManager.Verify(m => m.GetBooking(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void GetBooking_GetBookingWithDetailsCorrectResult()
    {
        var response = _bookingController.GetBooking(_bookingId);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getResult = response as OkObjectResult;
        var getResultValue = getResult!.Value as BookingModelResponse;

        Assert.AreEqual(_booking.BookerEmail, getResultValue!.BookerEmail);
        Assert.AreEqual(_booking.BookingDate, getResultValue.BookingDate);
        Assert.AreEqual(_booking.ConfirmationDate, getResultValue.ConfirmationDate);
        Assert.AreEqual(_booking.ClosureDate, getResultValue.ClosureDate);
        Assert.AreEqual(_booking.TotalAmount, getResultValue.TotalAmount);
        Assert.AreEqual(_booking.Status.ToString(), getResultValue.Status);
        Assert.AreEqual(_booking.PublicKey, getResultValue.PublicKey);
        Assert.AreEqual(_booking.Details.Count, getResultValue.Details.Count);
    }

    [TestMethod]
    public void CreateChallenge_ReturnCreated()
    {
        var response = _bookingController.CreateChallenge(_bookingId);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
    }

    [TestMethod]
    public void CreateChallenge_ReturnChallengeModelResponse()
    {
        var response = _bookingController.CreateChallenge(_bookingId);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
        var getResult = response as CreatedResult;
        Assert.IsInstanceOfType(getResult!.Value, typeof(ChallengeModelResponse));
    }

    [TestMethod]
    public void CreateChallenge_CallsCreateChallengeFromBookingManager()
    {
        _bookingController.CreateChallenge(_bookingId);

        _mockBookingManager.Verify(m => m.CreateChallenge(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void CreateChallenge_ReturnChallengeModelResponseWithChallenge()
    {
        var response = _bookingController.CreateChallenge(_bookingId);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
        var result = response as CreatedResult;
        var resultValue = result!.Value as ChallengeModelResponse;

        Assert.IsNotNull(resultValue!.Challenge);
    }

    [TestMethod]
    public void CreateChallenge_ReturnChallengeModelResponseWithCorrectChallenge()
    {
        var response = _bookingController.CreateChallenge(_bookingId);

        Assert.IsInstanceOfType(response, typeof(CreatedResult));
        var result = response as CreatedResult;
        var resultValue = result!.Value as ChallengeModelResponse;

        Assert.AreEqual(_challenge, resultValue!.Challenge);
    }

    [TestMethod]
    public void ConfirmChallenge_ReturnOk()
    {
        var response = _bookingController.ConfirmChallenge(_bookingId, _challengeRequest);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }

    [TestMethod]
    public void ConfirmChallenge_ReturnChallengeModelResponse()
    {
        var response = _bookingController.ConfirmChallenge(_bookingId, _challengeRequest);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var getResult = response as OkObjectResult;
        Assert.IsInstanceOfType(getResult!.Value, typeof(ChallengeModelResponse));
    }

    [TestMethod]
    public void ConfirmChallenge_CallsConfirmChallengeFromBookingManager()
    {
        _bookingController.ConfirmChallenge(_bookingId, _challengeRequest);

        _mockBookingManager.Verify(m => m.ConfirmChallenge(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void ConfirmChallenge_ReturnChallengeModelResponseWithCorrectChallenge()
    {
        var response = _bookingController.ConfirmChallenge(_bookingId, _challengeRequest);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        var result = response as OkObjectResult;
        var resultValue = result!.Value as ChallengeModelResponse;

        Assert.AreEqual(_challenge, resultValue!.Challenge);
    }

    [TestMethod]
    public void PickUpBooking_ReturnOk()
    {
        var response = _bookingController.PickUpBooking(_bookingId);

        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }

    [TestMethod]
    public void PickUpBooking_ReturnBookingModelResponse()
    {
        var response = _bookingController.PickUpBooking(_bookingId);

        var getResult = response as OkObjectResult;
        Assert.IsInstanceOfType(getResult!.Value, typeof(BookingModelResponse));
    }

    [TestMethod]
    public void PickUpBooking_CallsPickUpBookingFromBookingManager()
    {
        _bookingController.PickUpBooking(_bookingId);
        _mockBookingManager.Verify(m => m.PickUpBooking(_bookingId), Times.Once);
    }

    [TestMethod]
    public void PickUpBooking_ReturnBookingModelResponseWithCorrectBooking()
    {
        var response = _bookingController.PickUpBooking(_bookingId);

        var result = response as OkObjectResult;
        var resultValue = result!.Value as BookingModelResponse;

        Assert.AreEqual(_bookingId, resultValue!.Id);
        var expectedState = BookingStatus.PickUp.ToString();
        Assert.AreEqual(expectedState, resultValue!.Status);
    }
}