using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrototypeTopCoder.Models
{
	public class InsideCompetitionModel
	{
		public TimeSpan TimeLeft { get; set; }
		public List<ProblemModel> Problems { get; set; }
        public List<IProblemAnswer> Answers { get; set; }
		 
		public InsideCompetitionModel()
		{
			Problems = new List<ProblemModel>();
            Answers = new List<IProblemAnswer>();
		}
	}
}