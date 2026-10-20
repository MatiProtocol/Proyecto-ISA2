using Microsoft.EntityFrameworkCore;
using PharmaGo.IDataAccess;
using System.Linq.Expressions;
using PharmaGo.Domain.Enums;
using PharmaGo.IBusinessLogic;


namespace PharmaGo.DataAccess.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly PharmacyGoDbContext _context;
        protected readonly ILogManager _logManager;

        public BaseRepository(PharmacyGoDbContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }

        public virtual IEnumerable<T> GetAllByExpression(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _context.Set<T>().Where(expression);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public virtual T GetOneByExpression(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _context.Set<T>().FirstOrDefault(expression);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public virtual void InsertOne(T elem)
        {
            try
            {
                _context.Set<T>().Add(elem);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to prepare insert in repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public virtual void DeleteOne(T elem)
        {
            try
            {
                _context.Set<T>().Remove(elem);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to prepare delete in repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public void UpdateOne(T elem)
        {
            try
            {
                _context.Set<T>().Update(elem);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to prepare update in repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public virtual bool Exists(T elem)
        {
            try
            {
                T? _elem = _context.Set<T>().Find(elem);
                if (_elem is null) return false;
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Critical, "Failed to persist changes to database", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public bool Exists(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _context.Set<T>().Any(expression);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }

        public virtual IEnumerable<T> GetAllBasicByExpression(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _context.Set<T>().Where(expression);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }


        public virtual T GetOneDetailByExpression(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _context.Set<T>().FirstOrDefault(expression);
            }
            catch (Exception ex)
            {
                _logManager.Create(LogLevel.Error, "Failed to read from repository", $"{{ Entity: \"{typeof(T).Name}\", Error: \"{ex.Message}\" }}");
                throw;
            }
        }
    }
}
