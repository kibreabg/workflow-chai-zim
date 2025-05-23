﻿using Chai.WorkflowManagment.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;


namespace Chai.WorkflowManagment.CoreDomain.Infrastructure
{
    public class EFWorkspace : IWorkspace
    {
        private readonly BaseDbContext _context;

        public EFWorkspace(BaseDbContext context)
        {
            _context = context;
        }

        public void CommitChanges()
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    _context.SaveChanges();
                    scope.Complete();
                }
                catch (DbUpdateException ex)
                {
                    ExceptionUtility.LogException(ex, ex.Source);
                    ExceptionUtility.NotifySystemOps(ex, ex.Source);
                    UpdateException updateException = (UpdateException)ex.InnerException;
                    SqlException sqlException = (SqlException)updateException.InnerException;


                    foreach (SqlError error in sqlException.Errors)
                    {
                        // TODO: Do something with your errors
                    }
                }
                catch (Exception ex)
                {
                    ExceptionUtility.LogException(ex, ex.Source);
                    ExceptionUtility.NotifySystemOps(ex, ex.Source);
                }
            }

        }

        public void Refresh(IEnumerable collection)
        {
            _context.Refresh(collection);
        }

        public void Refresh(object item, string property)
        {
            _context.LoadProperty(item, property);
        }

        public void Refresh(object item)
        {
            _context.Refresh(item);
        }

        public void Delete<T>(Expression<Func<T, bool>> expression) where T : class
        {
            foreach (var item in _context.Set<T>().Where(expression))
                Delete(item);
        }

        public void Delete<T>(T item) where T : class
        {
            _context.Set<T>().Remove(item);
        }

        public void DeleteAll<T>() where T : class
        {
            foreach (var item in _context.Set<T>())
                Delete(item);
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return _context.Set<T>().SingleOrDefault(expression);
        }

        public T Single<T>(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes == null || includes.Length < 1)
                return _context.Trackable<T>().Where(expression).SingleOrDefault();
            var result = includes.Aggregate(_context.Trackable<T>(), (current, include) => current.Include(include)).Where(expression);
            return result.SingleOrDefault();
        }

        public T Last<T>() where T : class, IEntity
        {
            return _context.Set<T>().OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public IEnumerable<T> All<T>() where T : class
        {
            return _context.Set<T>().ToList();
        }

        public IEnumerable<T> All<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            return includes.Aggregate(_context.Trackable<T>(), (current, include) => current.Include(include));
        }

        public IEnumerable<T> All<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return _context.Set<T>().Where(expression);
        }

        public IEnumerable<T> SqlQuery<T>(string sqlquery) where T : class
        {
            return _context.Set<T>().SqlQuery(sqlquery);
        }

        public void Add<T>(T item) where T : class
        {
            _context.Set<T>().Add(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class
        {
            foreach (var item in items)
                Add(item);
        }

        public void Update<T>(T item) where T : class
        {
            var idf = item as IEntity;
            if (idf != null && idf.Id == 0)
                Add(item);
        }

        public int Count<T>() where T : class
        {
            return _context.Set<T>().Count();
        }

        public int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
        {
            return _context.ExecuteFunction(functionName, parameters);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
