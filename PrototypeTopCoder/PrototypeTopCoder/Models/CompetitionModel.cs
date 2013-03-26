using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PrototypeTopCoder.Models
{
	public enum UserCompetitionState
	{
		NotEnrolled,
		NotStarted,
		InProgress,
		Ended
	}

	public enum CompetitionState
	{
		NotStarted,
		InProgress,
		Ended
	}

    public class CompetitionModel
    {
		public int ID { get; private set; }

		[Display(Name = "Category")]
        public int CategoryId { get; set; }

		[DataType(DataType.DateTime)]
		[Display(Name = "Start date")]
        public DateTime Start { get; set; }

		[DataType(DataType.DateTime)]
		[Display(Name = "End date")]
		public DateTime End { get; set; }
		 
		[Display(Name = "Duration ")]
        public int Duration { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Name")]
		[StringLength(256)]
		public string Name { get; set; }

        private string _Description;
		[DataType(DataType.MultilineText)]
		[Display(Name = "Description")]
		public string Description
        {
            get { return HttpUtility.HtmlDecode(_Description); }
            set { this._Description = value; }
        }

        public MultiSelectList ProblemList { get; set; }

        [Display(Name = "Selected problems")]
        public int[] SelectedProblems { get; set; }

		public CompetitionState State
		{
			get
			{
				return Start > DateTime.Now ? CompetitionState.NotStarted : 
					End > DateTime.Now ? CompetitionState.InProgress : 
					CompetitionState.Ended;
			}
		}

        public CompetitionModel(Competition data)
        {
			this.ID = data.ID;
            this.CategoryId = data.ID;
            this.Start = data.Start;
            this.End = data.End;
            this.Duration = data.Duration;

			this.Name = data.Name;
			this.Description = data.Description;

            var problems = DataHelper.GetTasks(this.ID);
            this.SelectedProblems = problems.Select(x => x.ID).ToArray();
            this.ProblemList = GetProblems(this.SelectedProblems);
        }

        private MultiSelectList GetProblems(int[] selectedProblems)
        {
            var tasks = DataHelper.GetAllTasks();
            return new MultiSelectList(tasks, "id", "Title", selectedProblems);
        }

		public Competition EntityModel
		{
			set
			{
				Competition data = value;

				this.ID = data.ID;
				this.CategoryId = data.ID;
				this.Start = data.Start;
				this.End = data.End;
				this.Duration =  data.Duration;

				this.Name = data.Name;
				this.Description = data.Description;
			}
		}

        public CompetitionModel()
        {
			this.Name = String.Empty;
            this.Start = DateTime.Now;
            this.End = DateTime.Now;

            this.ProblemList = GetProblems(null);
        }
    }
}