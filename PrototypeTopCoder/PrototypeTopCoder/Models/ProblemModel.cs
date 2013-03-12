﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrototypeTopCoder.Models
{
	public enum ProblemType
	{
		SimpleTest = 1,
		//other stuff
	}

	[Serializable]
	public class ProblemModel
	{
		public string Title { get; set; }
		public int ID { get; set; }
		public virtual int Evaluate(object answer) { return 0; }
	}

	[Serializable]
	public class SimpleTestProblemModel : ProblemModel
	{
		public string Question { get; set; }

		public List<string> Options { get; set; }

		public int CorrectAnswer { get; set; }

		public override int Evaluate(object answer)
		{
			return Object.Equals(answer, CorrectAnswer) ? 1 : 0;
		}
	}

}