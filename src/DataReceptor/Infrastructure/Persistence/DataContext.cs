using Microsoft.EntityFrameworkCore;

namespace DataReceptor.Infrastructure.Persistence;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    
}