using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.Repositories.Impl;

public interface ICategoryRepository : IRepository<Category, CategoryDto>;
