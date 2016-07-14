using EmployeePortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EmployeePortal.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal DbSet<T> dbSet;
        internal ApplicationDbContext dbContext;

        public Repository(ApplicationDbContext db)
        {
            dbContext = db;
            dbSet = dbContext.Set<T>();
        }

        public T GetEntity(Expression<Func<T, bool>> predicate)
        {
            return dbContext.Set<T>().AsQueryable().Where(predicate).FirstOrDefault();
        }

        public IEnumerable<T> GetEntities(Expression<Func<T, bool>> predicate)
        {
            return dbContext.Set<T>().AsQueryable().Where(predicate).ToList();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
