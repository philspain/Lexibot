using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class Comment
    {
        [Key]
        public string ThingId { get; set; }
        public string Text { get; set; }
        public string PermaLink { get; set; }
        public DateTime CreatedUTC { get; set; }
        public string SubReddit { get; set; }
    }
}