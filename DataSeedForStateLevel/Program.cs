using DataSeedForStateLevel.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddSwaggerGen();  // Add this line to configure Swagger

builder.Services.AddScoped<DatabaseSeed>(); // Register the DatabaseSeed class

var app = builder.Build();

// Seed the database
if (args.Length > 0)
{
	var command = args[0].ToLower();
	if (command == "databaseseed")
	{
		using var scope = app.Services.CreateScope();
		var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeed>();
		await seeder.Run();
		Console.WriteLine("Database Seeding Done");

		return;
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();  // Enable Swagger middleware
	app.UseSwaggerUI();  // Enable Swagger UI
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
