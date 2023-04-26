using ConsoleApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication.Data;

public class HighSchoolDBContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Employee> Employees { get; set; } = null!;

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Class> Classes { get; set; } = null!;

    public DbSet<Position> Positions { get; set; } = null!;

    public DbSet<StudentTeacher> StudentTeachers { get; set; } = null!;

    public DbSet<StudentCourse> StudentCourses { get; set; } = null!;

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

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("Faculty");
            entity.Property(faculty => faculty.Name).IsRequired().HasMaxLength(25);
            // entity
            //     .HasMany<Employee>(faculty => faculty.Employees!)
            //     .WithOne(employee => employee.Faculty);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");
            //entity.Property(emp => emp.EmployeeId).UseIdentityColumn();
            entity
                .Property(employee => employee.PersonalIdentityNumber)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(employee => employee.FirstName).IsRequired().HasMaxLength(25);
            entity.Property(employee => employee.LastName).IsRequired().HasMaxLength(25);
            entity
                .Property(emp => emp.HireDate)
                .HasDefaultValueSql("(getdate())")
                .HasConversion(
                    new ValueConverter<DateOnly, DateTime>(
                        only => only.ToDateTime(new TimeOnly()),
                        dateTime => DateOnly.FromDateTime(dateTime)
                    )
                );
            entity.Property(emp => emp.Salary).HasColumnType("smallmoney");
            //entity.HasOne<Faculty>(emp => emp.Faculty).WithMany(faculty => faculty.Employees!);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable(nameof(Student));
            //entity.Ignore(student => student.Grades);
            //entity.Ignore(student => student.Teachers);
            entity
                .HasMany<Employee>(student => student.Teachers!)
                .WithMany()
                .UsingEntity<StudentTeacher>(
                    l =>
                        l.HasOne<Employee>(teacher => teacher.Teacher)
                            .WithMany()
                            .HasForeignKey(e => e.TeacherID),
                    r =>
                        r.HasOne<Student>(studentTeacher => studentTeacher.Student)
                            .WithMany(student => student.StudentTeachers!)
                            .HasForeignKey(studentTeacher => studentTeacher.StudentId)
                );
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable(nameof(Course));
            entity.Property(course => course.Name).IsRequired().HasMaxLength(25);
            entity.Property(course => course.Active).IsRequired();
            entity.HasOne<Employee>(course => course.Teacher).WithMany().HasForeignKey("TeacherId");
            entity.HasOne<Faculty>(course => course.Faculty).WithMany();
#pragma warning disable CS8621, CS8622, CS8634
            entity
                .HasMany(course => course.Students)
                .WithMany(student => student!.Courses!)
                .UsingEntity<StudentCourse>();
#pragma warning restore CS8621, CS8622, CS8634
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable(nameof(Class));
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable(nameof(Position));
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable(nameof(Grade));
            entity.Property(grade => grade.GradeValue).IsRequired();

            entity
                .Property(g => g.DateGraded)
                .HasDefaultValueSql("(getdate())")
                .HasConversion(
                    new ValueConverter<DateOnly, DateTime>(
                        only => only.ToDateTime(new TimeOnly()),
                        dateTime => DateOnly.FromDateTime(dateTime)
                    )
                );
            entity
                .HasOne<Employee>(grade => grade.Teacher)
                .WithMany()
                .HasForeignKey("TeacherId")
                .IsRequired();

            entity.HasOne<Student>(grade => grade.Student).WithMany(student => student.Grades!);
            entity.HasOne<Course>(grade => grade.Course).WithMany();
        });

        // Here I used a view as a join entity between Student and Teacher
        // instead of creating a join table in the database
        // It's just an experiment too see if it works. It does, but it's
        // almost certainly not a good idea to do this in a real application.
        modelBuilder
            .Entity<StudentTeacher>()
            .ToView("StudentTeacherView")
            .HasKey("StudentTeacherId");
        modelBuilder
            .Entity<StudentTeacher>()
            .Property(steacher => steacher.StudentId)
            .HasColumnName("StudentId");
        modelBuilder
            .Entity<StudentTeacher>()
            .Property(steacher => steacher.TeacherID)
            .HasColumnName("TeacherId");

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.ToTable(nameof(StudentCourse));
        });
    }
}
