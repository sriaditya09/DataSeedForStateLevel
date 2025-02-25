using DataSeedForStateLevel.Data;
using DataSeedForStateLevel.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseSeed
{
	private readonly DataDbContext context;

	public DatabaseSeed(DataDbContext context)
	{
		this.context = context;
	}

	public async Task Run()
	{
		await Seed();
	}

	public async Task Seed()
	{
		if (!await context.States.AnyAsync())
		{
			// Load states data from State.json
			var stateData = LoadStatesFromFile("State.json");

			// Load districts data from Districts.json
			var districtData = LoadDistrictsFromFile("Districts.json");

			if (stateData != null && districtData != null)
			{
				Console.WriteLine("States and Districts loaded separately:");

				// Add states data to States table
				await context.States.AddRangeAsync(stateData);
				await context.SaveChangesAsync(); // Save States first

				Console.WriteLine("States data seeded successfully.");

				// Now, add districts data to Districts table
				foreach (var districtInfo in districtData)
				{
					var state = stateData.FirstOrDefault(s => s.state == districtInfo.state);

					if (state != null)
					{
						// Loop through each district in the state's district list
						foreach (var districtName in districtInfo.districts)
						{
							var district = new Districts
							{
								district = districtName,
								state = districtInfo.state, // Add state directly here
								StateCode = state.StateCode // Associate district with StateCode
							};

							// Add district to DbContext
							await context.District.AddAsync(district);
						}

						Console.WriteLine($"Districts for state '{state.state}' added successfully.");
					}
					else
					{
						Console.WriteLine($"State '{districtInfo.state}' not found.");
					}
				}

				// Save districts after associating them with states
				await context.SaveChangesAsync();

				Console.WriteLine("Districts data seeded successfully.");
			}
			else
			{
				Console.WriteLine("Failed to load data from JSON files.");
			}
		}
		else
		{
			Console.WriteLine("Database already contains data.");
		}
	}

	// Load states from JSON file
	public List<State> LoadStatesFromFile(string filePath)
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		var stateFile = Path.Combine(currentDirectory, "Json", filePath);

		if (File.Exists(stateFile))
		{
			return JsonConvert.DeserializeObject<List<State>>(File.ReadAllText(stateFile));
		}
		else
		{
			Console.WriteLine($"State file '{filePath}' does not exist.");
			return null;
		}
	}

	// Load districts from JSON file
	public List<DistrictInfo> LoadDistrictsFromFile(string filePath)
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		var districtFile = Path.Combine(currentDirectory, "Json", filePath);

		if (File.Exists(districtFile))
		{
			return JsonConvert.DeserializeObject<List<DistrictInfo>>(File.ReadAllText(districtFile));
		}
		else
		{
			Console.WriteLine($"District file '{filePath}' does not exist.");
			return null;
		}
	}

	// Method to return district data in desired format
	public async Task<List<DistrictOutput>> GetDistrictOutput()
	{
		// Join the District and State tables based on StateCode and return the necessary fields
		var districtOutput = await context.District
			.Join(context.States,
				d => d.StateCode,
				s => s.StateCode,
				(d, s) => new DistrictOutput
				{
					state = s.state,
					district = d.district,
					DistrictCode = d.DistrictCode,
					StateCode = d.StateCode
				})
			.ToListAsync();

		return districtOutput;
	}
}

public class DistrictInfo
{
	public string state { get; set; }
	public List<string> districts { get; set; }
}

public class DistrictOutput
{
	public string state { get; set; }
	public string district { get; set; }
	public int DistrictCode { get; set; }
	public int StateCode { get; set; }
}
