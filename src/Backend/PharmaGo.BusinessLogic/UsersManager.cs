using PharmaGo.Domain.Entities;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;
using System.Text.RegularExpressions;
using PharmaGo.Domain.Enums;

namespace PharmaGo.BusinessLogic
{
    public class UsersManager : IUsersManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Invitation> _invitationRepository;
        private readonly ILogManager _logManager;

        public UsersManager(IRepository<User> repository, IRepository<Invitation> invitationRepository, ILogManager logManager)
        {
            _userRepository = repository;
            _invitationRepository = invitationRepository;
            _logManager = logManager;
        }

        public User CreateUser(string UserName, string UserCode, string Email, string Password, string Address, DateTime RegistrationDate)
        {
            string validUserCode = @"^[0-9]{6}$";
            Regex rgUserCode = new(validUserCode);
            string validEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex rgEmail = new(validEmail);
            string validPassword = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&.*-]).{8,}$";
            Regex rgPassword = new(validPassword);

            if (String.IsNullOrEmpty(UserName))
            {
                _logManager.Create(LogLevel.Warning, "Invalid Username", $"{{ UserName: \"{UserName}\" }}");
                throw new InvalidResourceException("Invalid Username");
            }
            if (String.IsNullOrEmpty(UserCode) || !rgUserCode.IsMatch(UserCode))
            {
                _logManager.Create(LogLevel.Warning, "Invalid UserCode", $"{{ UserName: \"{UserName}\", UserCode: \"{UserCode}\" }}");
                throw new InvalidResourceException("Invalid UserCode");
            }
            if (String.IsNullOrEmpty(Email) || !rgEmail.IsMatch(Email))
            {
                _logManager.Create(LogLevel.Warning, "Invalid Email", $"{{ UserName: \"{UserName}\", Email: \"{Email}\" }}");
                throw new InvalidResourceException("Invalid Email");
            }
            if (String.IsNullOrEmpty(Password) || !rgPassword.IsMatch(Password))
            {
                _logManager.Create(LogLevel.Warning, "Invalid Password", $"{{ UserName: \"{UserName}\", Email: \"{Email}\" }}");
                throw new InvalidResourceException("Invalid Password");
            }
            if (String.IsNullOrEmpty(Address))
            {
                _logManager.Create(LogLevel.Warning, "Invalid Address", $"{{ UserName: \"{UserName}\", Email: \"{Email}\" }}");
                throw new InvalidResourceException("Invalid Address");
            }

            Invitation invitation = _invitationRepository.GetOneDetailByExpression(x => x.UserName.ToLower() == UserName.ToLower() && x.UserCode == UserCode && x.IsActive);
            if (invitation == null)
            {
                _logManager.Create(LogLevel.Warning, "Invitation not found or is not currently active", $"{{ UserName: \"{UserName}\", UserCode: \"{UserCode}\" }}");
                throw new ResourceNotFoundException("Invitation not found or is not currently active");
            }
            User exists = _userRepository.GetOneByExpression(u => u.UserName.ToLower() == UserName.ToLower());
            if (exists != null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid Username, Username already exists", $"{{ UserName: \"{UserName}\", ExistingUserId: {exists.Id} }}");
                throw new InvalidResourceException("Invalid Username, Username already exists");
            }
            exists = _userRepository.GetOneByExpression(u => u.Email.ToLower() == Email.ToLower());
            if (exists != null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid Email, Email already exists", $"{{ Email: \"{Email}\", ExistingUserId: {exists.Id} }}");
                throw new InvalidResourceException("Invalid Email, Email already exists");
            }

            User user = new User
            {
                UserName = UserName,
                Email = Email,
                Address = Address,
                Password = Password,
                RegistrationDate = RegistrationDate,
                Pharmacy = invitation.Pharmacy,
                Role = invitation.Role
            };
            _userRepository.InsertOne(user);

            invitation.IsActive = false;
            _invitationRepository.UpdateOne(invitation);

            _userRepository.Save();
            _invitationRepository.Save();

            return user;

        }
    }

}
