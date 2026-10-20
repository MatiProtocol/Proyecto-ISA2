using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Domain.SearchCriterias;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.BusinessLogic
{
    public class StockRequestManager : IStockRequestManager
    {
        private readonly IRepository<StockRequest> _stockRequestRepository;
        private readonly IRepository<User> _employeeRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<Session> _sessionRepository;
        private readonly ILogManager _logManager;


        public StockRequestManager(IRepository<StockRequest> stockRequestRepository,
            IRepository<User> employeeRepository, IRepository<Drug> drugRepository, IRepository<Session> sessionRepository, ILogManager logManager)
        {
            _stockRequestRepository = stockRequestRepository;
            _employeeRepository = employeeRepository;
            _drugRepository = drugRepository;
            _sessionRepository = sessionRepository;
            _logManager = logManager;
        }

        public bool ApproveStockRequest(int id)
        {
            var stockRequest = _stockRequestRepository.GetOneByExpression(s => s.Id == id);
            if (stockRequest == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid stock request", $"{{ StockRequestId: {id} }}");
                throw new InvalidResourceException("Invalid stock request.");
            }
            if (stockRequest != null)
            {
                if (stockRequest.Status == Domain.Enums.StockRequestStatus.Approved)
                {
                    _logManager.Create(LogLevel.Warning, "Stock request already approved", $"{{ StockRequestId: {id}, CurrentStatus: \"{stockRequest.Status}\" }}");
                    throw new InvalidResourceException("Stock request already approved.");
                }
                if (stockRequest.Status == Domain.Enums.StockRequestStatus.Rejected)
                {
                    _logManager.Create(LogLevel.Warning, "Stock request already rejected", $"{{ StockRequestId: {id}, CurrentStatus: \"{stockRequest.Status}\" }}");
                    throw new InvalidResourceException("Stock request already rejected.");
                }
            }

            foreach (var stockRequestDetail in stockRequest.Details)
            {
                if (stockRequestDetail.Drug.Deleted == true)
                {
                    _logManager.Create(LogLevel.Warning, "Stock request has deleted drugs included", $"{{ StockRequestId: {id}, DrugId: {stockRequestDetail.Drug.Id}, DrugCode: \"{stockRequestDetail.Drug.Code}\" }}");
                    throw new InvalidResourceException("Stock request has deleted drugs included.");
                }
                var drug = _drugRepository.GetOneByExpression(d => d.Id == stockRequestDetail.Drug.Id);
                drug.Stock += stockRequestDetail.Quantity;
                _drugRepository.UpdateOne(drug);
            }

            stockRequest.Status = Domain.Enums.StockRequestStatus.Approved;
            _stockRequestRepository.UpdateOne(stockRequest);

            _drugRepository.Save();
            _stockRequestRepository.Save();

            return true;
        }

        public bool RejectStockRequest(int id)
        {
            var stockRequest = _stockRequestRepository.GetOneByExpression(s => s.Id == id);
            if (stockRequest == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid stock request", $"{{ StockRequestId: {id} }}");
                throw new InvalidResourceException("Invalid stock request.");
            }
            if (stockRequest != null)
            {
                if (stockRequest.Status == Domain.Enums.StockRequestStatus.Approved)
                {
                    _logManager.Create(LogLevel.Warning, "Stock request already approved", $"{{ StockRequestId: {id}, CurrentStatus: \"{stockRequest.Status}\" }}");
                    throw new InvalidResourceException("Stock request already approved.");
                }
                if (stockRequest.Status == Domain.Enums.StockRequestStatus.Rejected)
                {
                    _logManager.Create(LogLevel.Warning, "Stock request already rejected", $"{{ StockRequestId: {id}, CurrentStatus: \"{stockRequest.Status}\" }}");
                    throw new InvalidResourceException("Stock request already rejected.");
                }
            }

            stockRequest.Status = Domain.Enums.StockRequestStatus.Rejected;
            _stockRequestRepository.UpdateOne(stockRequest);
            _stockRequestRepository.Save();

            return true;
        }

        public StockRequest CreateStockRequest(StockRequest stockRequest, string token)
        {
            User existEmployee = null;
            if (stockRequest.Details == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid stock details", $"{{ Token: \"{token}\", Details: null }}");
                throw new InvalidResourceException("Invalid stock details.");
            }
            if (stockRequest.Details.Count == 0)
            {
                _logManager.Create(LogLevel.Warning, "Invalid stock details", $"{{ Token: \"{token}\", DetailsCount: 0 }}");
                throw new InvalidResourceException("Invalid stock details.");
            }

            var session = _sessionRepository.GetOneByExpression(session => session.Token == new Guid(token));
            if (session == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid session from employee", $"{{ Token: \"{token}\" }}");
                throw new InvalidResourceException("Invalid session from employee.");
            }
            var userId = session.UserId;

            existEmployee = _employeeRepository.GetOneDetailByExpression(u => u.Id == userId);
            if (existEmployee == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid employee", $"{{ UserId: {userId} }}");
                throw new InvalidResourceException("Invalid employee.");
            }
            stockRequest.Employee = existEmployee;

            foreach (StockRequestDetail item in stockRequest.Details)
            {
                var drug = _drugRepository.GetOneByExpression(d => d.Code == item.Drug.Code && d.Pharmacy.Id == existEmployee.Pharmacy.Id);
                if (drug == null)
                {
                    _logManager.Create(
                        LogLevel.Warning,
                        "Stock request has invalid drug",
                        $"{{ UserId: {existEmployee?.Id}, PharmacyId: {existEmployee?.Pharmacy?.Id}, DrugCode: \"{item?.Drug?.Code}\" }}");
                    throw new InvalidResourceException("Stock request has invalid drug.");
                }

                item.Drug = drug;
            }

            stockRequest.Status = Domain.Enums.StockRequestStatus.Pending;
            _stockRequestRepository.InsertOne(stockRequest);
            _stockRequestRepository.Save();

            return stockRequest;
        }

        public IEnumerable<StockRequest> GetStockRequestsByEmployee(string token, StockRequestSearchCriteria searchCriteria)
        {
            if (string.IsNullOrEmpty(token.ToString()))
            {
                _logManager.Create(LogLevel.Warning, "Invalid employee", $"{{ Token: \"{token}\" }}");
                throw new InvalidResourceException("Invalid employee.");
            }
            var session = _sessionRepository.GetOneByExpression(session => session.Token == new Guid(token));
            if (session == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid employee", $"{{ Token: \"{token}\" }}");
                throw new InvalidResourceException("Invalid employee.");
            }
            searchCriteria.EmployeeId = session.UserId;

            var stockRequests = _stockRequestRepository.GetAllBasicByExpression(searchCriteria.Criteria());

            return stockRequests;
        }

        public IEnumerable<StockRequest> GetStockRequestsByOwner(string token)
        {

            if (string.IsNullOrEmpty(token.ToString()))
            {
                _logManager.Create(LogLevel.Warning, "Invalid owner", $"{{ Token: \"{token}\" }}");
                throw new InvalidResourceException("Invalid owner.");
            }
            var session = _sessionRepository.GetOneByExpression(session => session.Token == new Guid(token));
            if (session == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid owner", $"{{ Token: \"{token}\" }}");
                throw new ResourceNotFoundException("Invalid owner.");
            }

            var userId = session.UserId;
            User user = _employeeRepository.GetOneDetailByExpression(u => u.Id == userId);
            if (user == null)
            {
                _logManager.Create(LogLevel.Warning, "Invalid user", $"{{ UserId: {userId} }}");
                throw new ResourceNotFoundException("Invalid user.");
            }
            var pharmacyId = user.Pharmacy.Id;
            var stockRequests = _stockRequestRepository.GetAllBasicByExpression(s => s.Employee.Pharmacy.Id == pharmacyId);

            return stockRequests;

        }
    }
}

