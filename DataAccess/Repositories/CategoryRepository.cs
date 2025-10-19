using DataAccess.Repositories.Impl;
using Domain.DTOs;
using Domain.Entities;

namespace DataAccess.Repositories;

public class CategoryRepository(BookStoreDbContext dbContext) : EfRepository<Category, CategoryDto>(dbContext), ICategoryRepository;