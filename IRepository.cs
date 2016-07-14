using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EmployeePortal.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetEntity(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetEntities(Expression<Func<T, bool>> predicate);
    }
}