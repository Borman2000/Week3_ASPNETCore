using AutoMapper;
using Domain.Base;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
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
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
	    return DtoMapper.Map<TEntity>(await DbSet.FindAsync(id));
//        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity?> AddAsync(TEntity dto)
    {
        var entity = DtoMapper.Map<TEntity>(dto);
        await DbSet.AddAsync(entity);
        try
        {
	        await DbContext.Commit();
        }
        catch (Exception e)
        {
	        Console.WriteLine(e);
	        return null;
        }
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity dto)
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