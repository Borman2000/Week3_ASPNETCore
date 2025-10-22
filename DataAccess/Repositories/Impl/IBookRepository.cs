using Domain.DTOs;
using Domain.Entities;

namespace DataAccess.Repositories.Impl;

public interface IBookRepository : IRepository<Book, BookDto>;
