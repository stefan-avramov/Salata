using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrototypeTopCoder.Models
{

	public class HomeModel
	{
		[Display(Name = "Competitions")]
		public IList<CompetitionModel> Competitions { get; set; }

		[Display(Name = "Enrolled Compettions")]
		public HashSet<int> EnrolledCompetitions { get; set; }
	}
}
