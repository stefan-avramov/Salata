using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PrototypeTopCoder.Models
{
    public class CompetitionModel
    {
		public int ID { get; private set; }

		[Display(Name = "Category")]
        public int CategoryId { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Start date")]
        public DateTime Start { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "End date")]
		public DateTime End { get; set; }
		 
		[Display(Name = "Duration ")]
        public int Duration { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Name")]
		[StringLength(256)]
		public string Name { get; set; }

		[DataType(DataType.MultilineText)]
		[Display(Name = "Description")]
		public string Description { get; set; }

        public CompetitionModel(Competition data)
        {
			this.ID = data.ID;
            this.CategoryId = data.ID;
            this.Start = data.Start;
            this.End = data.End;
            this.Duration = data.Duration;

			this.Name = data.Name;
			this.Description = data.Description;
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
        }
    }
}