using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class WordOccurrenceDTO
    {
        public virtual string Word { get; set; }
        public virtual int Occurrences { get; set; }
        public virtual int OccurrenceHour { get; set; }
        public virtual string SubReddit { get; set; }
    }
}