using ConsoleApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication.Data;

public class HighSchoolDBContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Faculty> Faculties { get; set; }

    /// <inheritdoc />
    protected HighSchoolDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public HighSchoolDBContext(
        DbContextOptions<HighSchoolDBContext> options,
        IConfiguration configuration
    ) : base(options)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("HighSchoolDB"));
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<Faculty>(entity =>
        // {
        //     entity.Property(faculty => faculty.FacultyId).UseIdentityColumn();
        //     entity.Property(faculty => faculty.Name).IsRequired().HasMaxLength(25);
        // });
        modelBuilder.Entity<Faculty>().ToTable("Faculty");
        modelBuilder.Entity<Faculty>().Property(f => f.FacultyId).UseIdentityColumn();
        modelBuilder.Entity<Faculty>().Property(f => f.Name).IsRequired().HasMaxLength(25);
        // modelBuilder.Entity<Employee>().Property(e => e.PersonId).UseIdentityColumn();
        // modelBuilder
        //     .Entity<Employee>()
        //     .Property(e => e.HireDate)
        //     .HasDefaultValueSql("(getdate())")
        //     .HasConversion(
        //         new ValueConverter<DateOnly, DateTime>(
        //             only => only.ToDateTime(new TimeOnly()),
        //             dateTime => DateOnly.FromDateTime(dateTime)
        //         )
        //     );
        // modelBuilder
        //     .Entity<Grade>()
        //     .Property(g => g.DateGraded)
        //     .HasDefaultValueSql("(getdate())")
        //     .HasConversion(
        //         new ValueConverter<DateOnly, DateTime>(
        //             only => only.ToDateTime(new TimeOnly()),
        //             dateTime => DateOnly.FromDateTime(dateTime)
        //         )
        //     );
    }
}
