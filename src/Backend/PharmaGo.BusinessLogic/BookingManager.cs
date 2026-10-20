using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NSec.Cryptography;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.BusinessLogic;

public class BookingManager(IRepository<Pharmacy> _pharmacysRepository, IRepository<Booking> _bookingRepository, IMemoryCache _memoryCache, IRepository<User> _userRepository, ILogManager _logManager) : IBookingManager
{
    const string validEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
    private static readonly TimeSpan _challengeLifetime = TimeSpan.FromMinutes(5);

    public Booking Create(Booking booking, bool hasPrescriptionAll)
    {
        ValidatePublicKey(booking.PublicKey);
        ValidateSetDates(booking);

        booking.Status = BookingStatus.Pending;
        if (!hasPrescriptionAll) booking.Status = BookingStatus.PendingValidation;

        ValidateEmail(booking.BookerEmail);

        if (booking.Details == null || booking.Details.Count == 0)
        {
            _logManager.Create(LogLevel.Warning, "The list of drugs can't be empty", $"{{ Booking: {booking?.Id} }}");
            throw new InvalidResourceException("The list of drugs can't be empty");
        }

        var totalAmount = 0;
        foreach (var drug in booking.Details)
        {
            var pharmacyId = drug.PharmacyId;
            Pharmacy pharmacy = ValidatePharmacy(pharmacyId);
            var durgFromRepo = ValidateDrug(drug, pharmacy);
            drug.Price = (int)durgFromRepo.Price;
            totalAmount += drug.Price * drug.Quantity;
            drug.PharmacyName = pharmacy.Name;
            drug.Name = durgFromRepo.Name;
        }
        booking.TotalAmount = totalAmount;

        _bookingRepository.InsertOne(booking);
        _bookingRepository.Save();
        return booking;
    }

    public List<Booking> GetAll()
    {
        return _bookingRepository.GetAllByExpression(booking => true).ToList();
    }

    public List<Booking> GetByPharmacyId(int pharmacyId)
    {
        var bookingFromRepo = _bookingRepository.GetAllByExpression(booking => booking.Details.First().PharmacyId == pharmacyId).ToList();
        return bookingFromRepo;
    }

    public bool ValidateBookingEmployee(int pharmacyId, string name, int bookingId, bool cancel)
    {
        var valid = ValidateEmployee(name, pharmacyId);
        if (!valid) return false;
        var bookingList = GetByPharmacyId(pharmacyId);
        if (bookingList.Count == 0) return false;
        foreach (var booking in bookingList)
        {
            if (booking.Id != bookingId) continue;
            if (booking.Status != BookingStatus.Pending) continue;
            if (cancel)
            {
                booking.Status = BookingStatus.Caneceled;
                booking.ClosureDate = new DateTimeOffset(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                booking.Status = BookingStatus.Confirmed;
                booking.ConfirmationDate = new DateTimeOffset(DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            _bookingRepository.UpdateOne(booking);
            _bookingRepository.Save();
            return valid;
        }
        return false;
    }

    public bool ValidateEmployee(string name, int pharmacyId)
    {
        return _userRepository.Exists(usr =>
            usr.UserName == name && usr.Pharmacy.Id != null && usr.Pharmacy.Id == pharmacyId);
    }

    public Booking GetBooking(int bookingId)
    {
        var booking = _bookingRepository.GetOneByExpression(b => b.Id == bookingId);
        if (booking is null)
        {
            _logManager.Create(LogLevel.Warning, $"Booking {bookingId} don't exist", $"{{ BookingId: {bookingId} }}");
            throw new ResourceNotFoundException($"Booking {bookingId} don't exist");
        }

        return booking;
    }

    public Booking PickUpBooking(int bookingId)
    {
        var booking = GetBooking(bookingId);
        EnsureBookingIsConfirmed(booking);

        booking.Status = BookingStatus.PickUp;
        booking.ClosureDate = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        _bookingRepository.UpdateOne(booking);
        _bookingRepository.Save();

        return booking;
    }

    public List<Booking> GetAll(string? email)
    {
        if (email != null)
        {
            ValidateEmail(email);

            var bookings = _bookingRepository.GetAllByExpression(b => b.BookerEmail == email);
            return bookings.ToList();
        }
        else
        {
            var bookings = _bookingRepository.GetAllByExpression(b => true);
            return bookings.ToList();
        }
    }

    private void ValidateEmail(string email)
    {
        Regex rgEmail = new(validEmail);
        if (string.IsNullOrEmpty(email) || !rgEmail.IsMatch(email))
        {
            _logManager.Create(LogLevel.Warning, "Invalid Email", $"{{ Email: \"{email}\" }}");
            throw new InvalidResourceException("Invalid Email");
        }
    }

    private Pharmacy ValidatePharmacy(int pharmacyId)
    {
        if (pharmacyId <= 0)
        {
            _logManager.Create(LogLevel.Warning, "Pharmacy Id is a mandatory field", $"{{ PharmacyId: {pharmacyId} }}");
            throw new ResourceNotFoundException($"Pharmacy Id is a mandatory field");
        }

        var pharmacy = _pharmacysRepository.GetOneByExpression(x => x.Id == pharmacyId);

        if (pharmacy is null)
        {
            _logManager.Create(LogLevel.Warning, "Pharmacy not found", $"{{ PharmacyId: {pharmacyId} }}");
            throw new ResourceNotFoundException($"Pharmacy {pharmacyId} not found");
        }

        return pharmacy;
    }

    private Drug ValidateDrug(BookingDetail drug, Pharmacy pharmacy)
    {
        if (drug.Quantity <= 0)
        {
            _logManager.Create(LogLevel.Warning, "Quantity must be greater than 0", $"{{ DrugCode: \"{drug?.Code}\", PharmacyId: {pharmacy?.Id} }}");
            throw new InvalidResourceException("Quantity must be greater than 0");
        }

        string drugCode = drug.Code;
        var drugFromRepository = pharmacy.Drugs.FirstOrDefault(x => x.Code == drugCode && !x.Deleted);
        if (drugFromRepository is null)
        {
            _logManager.Create(LogLevel.Warning, "Drug not found in pharmacy", $"{{ DrugCode: \"{drugCode}\", Pharmacy: \"{pharmacy?.Name}\" }}");
            throw new ResourceNotFoundException($"Drug {drugCode} not found in Pharmacy {pharmacy.Name}");
        }

        if (drug.Quantity > drugFromRepository.Stock)
        {
            _logManager.Create(LogLevel.Warning, "Requested quantity exceeds available stock", $"{{ DrugCode: \"{drugCode}\", Requested: {drug.Quantity}, Available: {drugFromRepository.Stock} }}");
            throw new InvalidResourceException("The selected Quantity is more than available");
        }

        return drugFromRepository;
    }

    private void ValidateSetDates(Booking booking)
    {
        DateTimeOffset bookingDate;
        bool parsed = DateTimeOffset.TryParseExact(booking.BookingDate, "yyyy-MM-ddTHH:mm:ss.fffZ", null, DateTimeStyles.AssumeUniversal, out bookingDate);

        if (!parsed)
        {
            _logManager.Create(LogLevel.Warning, "The booking date must has the format 'yyyy-MM-ddTHH:mm:ss.fffZ'", "{ PublicKey: " + booking.BookingDate + "}");

            throw new FormatException("The booking date must has the format 'yyyy-MM-ddTHH:mm:ss.fffZ'");
        }

        var actualDate = DateTimeOffset.Now;

        if (bookingDate > actualDate) booking.BookingDate = actualDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

        booking.ConfirmationDate = "";
        booking.ClosureDate = "";
    }

    private void ValidatePublicKey(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            _logManager.Create(LogLevel.Warning, "The public key must not be null", "{ PublicKey: null }");

            throw new InvalidResourceException("The public key must not be null");
        }

        if (publicKey.Length != 44)
        {
            _logManager.Create(LogLevel.Warning, "The public key must be 44 characters", "{ PublicKey: " + publicKey + " }");

            throw new InvalidResourceException("The public key must be 44 characters");
        }
    }

    public string CreateChallenge(int bookingId)
    {
        var booking = GetBooking(bookingId);
        EnsureBookingIsConfirmed(booking);

        string nonce = Guid.NewGuid().ToString("N")[..8];
        string timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        string challenge = $"{bookingId}:{timeStamp}:{nonce}";

        try
        {
            _memoryCache.Set($"challenge_{bookingId}", challenge, _challengeLifetime);
        }
        catch (Exception ex)
        {
            _logManager.Create(LogLevel.Error, "Failed to store challenge in cache", $"{{ BookingId: {bookingId}, Error: \"{ex.Message}\" }}");
            throw;
        }

        return challenge;
    }

    public string ConfirmChallenge(int bookingId, string challenge, string signature)
    {
        ChallengeIsStored(bookingId, challenge);
        var booking = GetBooking(bookingId);
        EnsureBookingIsConfirmed(booking);

        var expectedPublicKeyLength = 32;
        var publicKeyBytes = ValidateBase64AndLength(booking.PublicKey, expectedPublicKeyLength, "public key");

        var expectedSignatureLength = 64;
        var signatureBytes = ValidateBase64AndLength(signature, expectedSignatureLength, "signature");

        try
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            var publicKeyImported = PublicKey.Import(algorithm, publicKeyBytes, KeyBlobFormat.RawPublicKey);
            byte[] challengeBytes = Encoding.UTF8.GetBytes(challenge);
            var isValid = algorithm.Verify(publicKeyImported, challengeBytes, signatureBytes);

            if (!isValid)
            {
                _logManager.Create(LogLevel.Warning, "Signature verification failed", $"{{ BookingId: {bookingId} }}");
                throw new InvalidResourceException("The provided private key is not valid for this reservation");
            }
        }
        catch (InvalidResourceException) { throw; }
        catch (Exception ex)
        {
            _logManager.Create(LogLevel.Error, "Error during signature verification", $"{{ BookingId: {bookingId}, Error: \"{ex.Message}\" }}");
            throw;
        }

        try
        {
            _memoryCache.Remove($"challenge_{bookingId}");
        }
        catch (Exception ex)
        {
            _logManager.Create(LogLevel.Error, "Failed to remove challenge from cache", $"{{ BookingId: {bookingId}, Error: \"{ex.Message}\" }}");
            throw;
        }

        return challenge;
    }

    private void ChallengeIsStored(int bookingId, string challenge)
    {
        try
        {
            if (!_memoryCache.TryGetValue($"challenge_{bookingId}", out string storedChallenge))
            {
                _logManager.Create(LogLevel.Warning, "No stored challenge was found for this booking", $"{{ BookingId: {bookingId} }}");
                throw new ResourceNotFoundException("No stored challenge was found for this booking");
            }
            if (storedChallenge != challenge)
            {
                _logManager.Create(LogLevel.Warning, "The provided challenge does not match the stored challenge", $"{{ BookingId: {bookingId} }}");
                throw new ResourceNotFoundException("The provided challenge does not match the stored challenge");
            }
        }
        catch (ResourceNotFoundException) { throw; }
        catch (Exception ex)
        {
            _logManager.Create(LogLevel.Error, "Failed to read challenge from cache", $"{{ BookingId: {bookingId}, Error: \"{ex.Message}\" }}");
            throw;
        }
    }

    private void EnsureBookingIsConfirmed(Booking booking)
    {
        var status = booking.Status;
        if (status != BookingStatus.Confirmed)
        {
            _logManager.Create(LogLevel.Warning, "The booking must be in confirmed status", $"{{ BookingId: {booking?.Id}, CurrentStatus: \"{status}\" }}");
            throw new ResourceNotFoundException("The booking must be in confirmed status");
        }
    }

    private byte[] ValidateBase64AndLength(string elementToValidate, int expectedLength, string element)
    {
        byte[] elementBytes;
        try
        {
            elementBytes = Convert.FromBase64String(elementToValidate);
            if (elementBytes.Length != expectedLength)
            {
                _logManager.Create(LogLevel.Warning, $"The {element} format is invalid", $"{{ ProvidedLength: {elementBytes.Length}, ExpectedLength: {expectedLength} }}");
                throw new InvalidResourceException($"The {element} format is invalid");
            }
        }
        catch
        {
            _logManager.Create(LogLevel.Warning, $"The {element} format is invalid", $"{{ Element: \"{element}\" }}");
            throw new InvalidResourceException($"The {element} format is invalid");
        }

        return elementBytes;
    }
}