using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSeedForStateLevel.Model
{
	public class Districts
	{
		[Key]
		public int DistrictCode { get; set; }
		public string state { get; set; }  // Add state column in the Districts table
		public string district { get; set; }

		public int StateCode { get; set; }
	}

}