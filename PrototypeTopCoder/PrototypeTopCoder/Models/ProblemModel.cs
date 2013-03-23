using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrototypeTopCoder.Models
{
	public enum ProblemType
	{
		None = 0,
		SimpleTestQuestion = 1,
		ComplexTextQuestion = 2,
		//other stuff
	}

	[Serializable]
	public class ProblemModel
	{
		public string Title { get; set; }
		public int ID { get; set; }
		public virtual int Evaluate(object answer) { return 0; }
		public virtual ProblemType Type
		{
			get
			{
				return ProblemType.None;
			}
        }

        private string _Question;
        public string Question
        {
            get { return HttpUtility.HtmlDecode(_Question); }
            set { _Question = value; }
        }
	}

	[Serializable]
	public class SimpleTestProblemModel : ProblemModel
	{
		public List<string> Options { get; set; }

		public int CorrectAnswer { get; set; }

		public override int Evaluate(object answer)
		{
			return Object.Equals(answer, CorrectAnswer) ? 1 : 0;
		}

		public override ProblemType Type
		{
			get
			{
				return ProblemType.SimpleTestQuestion;
			}
		}
	}

	[Serializable]
	public class ComplexTestProblemModel : ProblemModel
	{
		public List<string> Options { get; set; }

		public List<int> CorrectAnswers { get; set; }

		public override int Evaluate(object answer)
		{
			List<int> answers = answer as List<int>;
			if (answers != null)
			{
				int corrects = answers.Where(x => CorrectAnswers.Contains(x)).Count();
				int incorrect = answers.Where(x => !CorrectAnswers.Contains(x)).Count();
				return Math.Max(0, corrects - incorrect);
			}

			return 0;
		}

		public override ProblemType Type
		{
			get
			{
				return ProblemType.ComplexTextQuestion;
			}
		}
	}

}