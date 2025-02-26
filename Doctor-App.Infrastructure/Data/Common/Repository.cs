using Doctor_App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Infrastructure.Data.Common
{
    public class Repository : IRepository
    {
        private readonly DoctorAppDbContext _context;
        public Repository(DoctorAppDbContext context)
        {
            _context = context;
        }
        private DbSet<T> DbSet<T>() where T : class
        {
            return _context.Set<T>();
        }
        public async Task AddAsync<T>(T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await SaveChangesAsync();
        }

        public IQueryable<T> All<T>(bool tracking = true) where T : class
        {
            return tracking ? DbSet<T>() : DbSet<T>().AsNoTracking();
        }

        public IQueryable<T> AllReadOnly<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task DeleteAsync<T>(object id) where T : class, new()
        {
            /*T? entity = await GetByIdAsync<T>(id);

            if (entity != null)
            {
                DbSet<T>().Remove(entity);
            }*/
            var entity = new T(); // Create an empty entity
            _context.Entry(entity).Property("Id").CurrentValue = id; // Set only the ID
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }

        public async Task<T?> GetByIdAsync<T>(object id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
