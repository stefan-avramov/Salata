using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrototypeTopCoder.Models
{
	public enum ProblemType
	{
		None = 0,
		SimpleTestQuestion = 1,
		ComplexTextQuestion = 2,
        HumanGradableQuestion = 3,
		//other stuff
	}

	public interface IProblemAnswer
	{
		//to whomever might be this needed
	}

	[Serializable]
	public class SimpleProblemAnswer : IProblemAnswer
	{
		public int Answer { get; set; }
	}

	[Serializable]
	public class ComplexProblemAnswer : IProblemAnswer
	{
		public int[] Answers { get; set; }
	}

    [Serializable]
    public class HumanGradableAnswer : IProblemAnswer
    {
        private string _Answer;
        public string Answer 
        {
            get { return HttpUtility.HtmlDecode(_Answer); }
            set { _Answer = value; }
        }
        public int Score { get; set; }
    }

	[Serializable]
	public class ProblemModel
	{
		public string Title { get; set; }
		public int ID { get; set; }
		public virtual int Evaluate(IProblemAnswer answer) { return 0; }
		public virtual ProblemType Type
		{
			get
			{
				return ProblemType.None;
			}
        }

        private string question;
        public string Question
        {
            get { return HttpUtility.HtmlDecode(question); }
            set { question = value; }
        }
	}

	[Serializable]
	public class SimpleTestProblemModel : ProblemModel
	{
		public List<string> Options { get; set; }

        [Display(Name="Correct Answer")]
		public int CorrectAnswer { get; set; }

		public override int Evaluate(IProblemAnswer answer)
		{
			SimpleProblemAnswer sans = answer as SimpleProblemAnswer; 
			return sans != null ? sans.Answer == CorrectAnswer ? 1 : 0 : 0;
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

		public override int Evaluate(IProblemAnswer answer)
		{
			ComplexProblemAnswer answers = answer as ComplexProblemAnswer;
			if (answers != null)
			{
				int corrects = answers.Answers.Where(x => CorrectAnswers.Contains(x)).Count();
				int incorrect = answers.Answers.Where(x => !CorrectAnswers.Contains(x)).Count();
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

    [Serializable]
    public class HumanGradableProblemModel : ProblemModel
    {
        public override int Evaluate(IProblemAnswer answer)
        {
            HumanGradableAnswer ans = answer as HumanGradableAnswer;
            return ans == null ? 0 : ans.Score;
        }

        public override ProblemType Type
        {
            get
            {
                return ProblemType.HumanGradableQuestion;
            }
        }
    }

}