using AutoMapper;
using DataAccess.Repositories.Impl;
using Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class EfRepository<TEntity, TDto> : IRepository<TEntity, TDto> where TEntity : BaseEntity where TDto : BaseEntity
{
    protected readonly BookStoreDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IMapper DtoMapper;

    protected EfRepository(BookStoreDbContext context, IMapper dtoMapper)
    {
Console.WriteLine("EfRepository constructed");
	    DbContext = context;
        DbSet = context.Set<TEntity>();
        DtoMapper = dtoMapper;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<TDto?> GetByIdAsync(Guid id)
    {
	    return DtoMapper.Map<TDto>(await DbSet.FindAsync(id));
//        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity?> AddAsync(TDto dto)
    {
        var entity = DtoMapper.Map<TEntity>(dto);
        await DbSet.AddAsync(entity);
        try
        {
	        await DbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
	        Console.WriteLine(e);
	        return null;
        }
        return entity;
    }

    public virtual async Task UpdateAsync(TDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
	    var entity = DbSet.Find(id);
	    if (entity is not null)
	    {
		    DbSet.Remove(entity);
		    DbContext.SaveChanges();
	    }
	    return Task.CompletedTask;
    }

    public IQueryable<TEntity> Query()
    {
        throw new NotImplementedException();
    }
}