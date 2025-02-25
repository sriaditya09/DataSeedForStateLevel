using System.ComponentModel.DataAnnotations;

namespace DataSeedForStateLevel.Model
{
	public class State
	{
		[Key]
		public int StateCode { get; set; }
		public string state { get; set; } 
		public int CountryCode { get; set; }

		public List<Districts> districts { get; set; }

	}
}
