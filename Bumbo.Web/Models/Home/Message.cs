using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bumbo.Web.Models
{
    public class Message
    {
        [Display(Name = "Prioriteit")]
        public Priorities? Priority { get; set; }

        [Display(Name = "Type")]
        public MessageType Type { get; set; }

        [Display(Name = "Titel")]
        public string Title { get; set; }

        [Display(Name = "Beschrijving")]
        public string Content { get; set; }

        [Display(Name = "Locatie")]
        public string Location { get; set; }

        [Display(Name = "Gerelateerde datum")]
        public DateTime RelatedDate { get; set; }

        public string GetFormattedDate()
        {
            if (RelatedDate == null) return null;

            string day = (RelatedDate.Day < 10) ? "0" + RelatedDate.Day.ToString() : RelatedDate.Day.ToString();
            string month = (RelatedDate.Month < 10) ? "0" + RelatedDate.Month.ToString() : RelatedDate.Month.ToString();

            return day + "-" + month + "-" + RelatedDate.Year;
        }

        public enum MessageType
        {
            List,
            Card
        }

        public enum Priorities
        {
            High = 1, // Bootstrap color: Danger
            Medium = 2, // Bootstrap color: Warning
            Low = 3 // Bootstrap color: none/default
        }
    }
}
