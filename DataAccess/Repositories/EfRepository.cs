using AutoMapper;
using DataAccess.Repositories.Impl;
using Domain.Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class EfRepository<TEntity, TDto> : IRepository<TEntity, TDto> where TEntity : BaseEntity where TDto : BaseEntity
{
    protected readonly BookStoreDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    protected EfRepository(BookStoreDbContext context)
    {
Console.WriteLine("EfRepository constructed");
	    DbContext = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
//        return await DbContext.Set<TEntity>().ToListAsync();
        return await DbSet.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<TEntity?> AddAsync(TDto dto)
    {
       throw new NotImplementedException();
    }

    public Task UpdateAsync(TDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
	    var book = DbSet.Find(id);
	    if (book is not null)
	    {
		    DbSet.Remove(book);
		    DbContext.SaveChanges();
	    }
	    return Task.CompletedTask;
    }

    public IQueryable<TEntity> Query()
    {
        throw new NotImplementedException();
    }
}