using Microsoft.AspNetCore.Mvc;
using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;
using PharmaGo.WebApi.Converters;
using PharmaGo.WebApi.Enums;
using PharmaGo.WebApi.Filters;
using PharmaGo.WebApi.Models.In;
using PharmaGo.WebApi.Models.Out;

namespace PharmaGo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(IBookingManager bookingManager) : ControllerBase
    {
        private readonly IBookingManager _bookingManager = bookingManager;

        [HttpPost]
        public IActionResult CreateBooking([FromBody] BookingModelRequest bookingModel)
        {
            var converter = new ConvertBookingModelRequestToBooking();
            var booking = _bookingManager.Create(converter.Convert(bookingModel), bookingModel.HasPrescriptionAll);
            var response = new BookingModelResponse(booking);
            return Created("", response);
        }

        [HttpGet("{bookingId}")]
        public IActionResult GetBooking([FromRoute] int bookingId)
        {
            var booking = _bookingManager.GetBooking(bookingId);
            var response = new BookingModelResponse(booking);
            return Ok(response);
        }

        [HttpPost("{bookingId}/challenge")]
        public IActionResult CreateChallenge([FromRoute] int bookingId)
        {
            string createdChallenge = _bookingManager.CreateChallenge(bookingId);
            ChallengeModelResponse challengeResponse = new ChallengeModelResponse(createdChallenge);
            return Created("", challengeResponse);
        }

        [HttpPost("{bookingId}/challenge-confirm")]
        public IActionResult ConfirmChallenge([FromRoute] int bookingId, [FromBody] ChallengeModelRequest challengeRequest)
        {
            var challengeResponse = _bookingManager.ConfirmChallenge(bookingId, challengeRequest.Challenge, challengeRequest.Signature);
            return Ok(new ChallengeModelResponse(challengeResponse));
        }

        [HttpPut("{bookingId}/pick-up")]
        public IActionResult PickUpBooking([FromRoute] int bookingId)
        {
            var booking = _bookingManager.PickUpBooking(bookingId);
            var response = new BookingModelResponse(booking);
            return Ok(response);
        }

        [HttpGet("by-pharmacy")]
        [AuthorizationFilter(new string[] { nameof(RoleType.Employee) })]
        public IActionResult GetBookingByPharmacy([FromQuery] int? pharmacyId)
        {
            List<Booking> bookingList;
            if (pharmacyId != null) bookingList = _bookingManager.GetByPharmacyId(pharmacyId.Value);
            else bookingList = _bookingManager.GetAll();
            return Ok(bookingList.Select(b => new BookingModelResponse(b)).ToList());
        }

        [HttpGet]
        public IActionResult GetAllBookings([FromQuery] string? email)
        {
            var bookings = _bookingManager.GetAll(email);
            var response = bookings.Select(booking => new BookingModelResponse(booking)).ToList();
            return Ok(response);
        }

        [HttpPost("{pharmacyId}/approve")]
        [AuthorizationFilter(new string[] { nameof(RoleType.Employee) })]
        public IActionResult ApproveBooking([FromRoute] int pharmacyId,
            [FromBody] BookingEmployeeRequest bookingRequestModel)
        {

            var validEmployeeBooking = _bookingManager.ValidateBookingEmployee(bookingRequestModel.PharmacyId,
                bookingRequestModel.ApproverName, bookingRequestModel.BookingId, bookingRequestModel.Cancel);
            return Ok(new BookingEmployeeResponse(validEmployeeBooking));
        }

        [HttpPost("{pharmacyId}/cancel")]
        [AuthorizationFilter(new string[] { nameof(RoleType.Employee) })]
        public IActionResult CancelBooking([FromRoute] int pharmacyId,
            [FromBody] BookingEmployeeRequest bookingRequestModel)
        {
            var validEmployeeBooking = _bookingManager.ValidateBookingEmployee(bookingRequestModel.PharmacyId,
                bookingRequestModel.ApproverName, bookingRequestModel.BookingId, bookingRequestModel.Cancel);
            return Ok(new BookingEmployeeResponse(validEmployeeBooking));
        }

    }
}
