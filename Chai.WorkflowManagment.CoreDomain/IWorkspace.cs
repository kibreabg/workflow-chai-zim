﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.Objects;

using Chai.WorkflowManagment.CoreDomain.Infrastructure;

namespace Chai.WorkflowManagment.CoreDomain
{
    public interface IWorkspace : IDisposable
    {
        void CommitChanges();
        void Delete<T>(Expression<Func<T, bool>> expression) where T : class;
        void Delete<T>(T item) where T : class;
        void DeleteAll<T>() where T : class;
        T Single<T>(Expression<Func<T, bool>> expression) where T : class;
        T Single<T>(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes) where T : class;
        T Last<T>() where T : class, IEntity;
        IEnumerable<T> All<T>() where T : class;
        IEnumerable<T> All<T>(params Expression<Func<T, object>>[] includes) where T : class;
        IEnumerable<T> All<T>(Expression<Func<T, bool>> expression) where T : class;
        IEnumerable<T> SqlQuery<T>(string sqlquery) where T : class;

        void Add<T>(T item) where T : class;
        void Add<T>(IEnumerable<T> items) where T : class;
        void Update<T>(T item) where T : class;
        int Count<T>() where T : class;

        //void ReloadAll();
        //void ResetDatabase();
        //ObjectContext GetBaseDbContext { get; }        
        void Refresh(IEnumerable collection);
        void Refresh(object item);
        void Refresh(object item, string property);

        int ExecuteFunction(string functionName, params ObjectParameter[] parameters);
    }
}
