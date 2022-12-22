using Microsoft.EntityFrameworkCore;
using SchoolMinimalApi.DTOs;
using SchoolMinimalApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolMinimalApiConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


//app.MapGet("/test", () => "Hello World!");

var minimalApi = app.MapGroup("courses");

// Get all courses
minimalApi.MapGet("/", async (SchoolDbContext dbContext) =>
{
    return Results.Ok(await dbContext.Courses.Select(c => new CourseDto(c)).ToListAsync());
});

// Get a course by Id
minimalApi.MapGet("/{id}", async (int id, SchoolDbContext dbContext) =>
{
    var course = await dbContext.Courses.FindAsync(id);
    if (course == null) return Results.NotFound();
    var courseDto = new CourseDto(course);
    return Results.Ok(courseDto);
});

// Create a course
minimalApi.MapPost("/", async (CourseDto courseDto, SchoolDbContext dbContext) =>
{
    var course = new Course
    {
        Name = courseDto.Name
    };
    dbContext.Courses.Add(course);
    await dbContext.SaveChangesAsync();
    var courseCreatedDto = new CourseDto(course);
    return Results.Created($"/course/{courseCreatedDto.Id}", courseCreatedDto);
});

// Update a course
minimalApi.MapPut("/{id}", async (int id, CourseDto courseDto, SchoolDbContext dbContext) =>
{
    var course = dbContext.Courses.Find(id);
    if (course == null) return Results.NotFound();
    course.Name = courseDto.Name;
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a course

// Get a course by Id
minimalApi.MapDelete("/{id}", async (int id, SchoolDbContext dbContext) =>
{
    var course = dbContext.Courses.Find(id);
    if (course == null) return Results.NotFound();
    dbContext.Courses.Remove(course);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
