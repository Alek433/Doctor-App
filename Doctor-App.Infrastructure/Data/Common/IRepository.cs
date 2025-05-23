﻿using Doctor_App.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Infrastructure.Data.Common
{
    public interface IRepository
    {
        IQueryable<T> All<T>(bool tracking = true) where T : class;
        IQueryable<T> AllReadOnly<T>() where T : class;
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T?> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task<int> SaveChangesAsync();
        Task<T?> GetByIdAsync<T>(object id) where T : class;
        Task DeleteAsync<T>(object id) where T : class, new();
        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
    }
}
