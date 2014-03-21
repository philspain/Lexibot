using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class ParsedComment
    {
        [Key]
        public string CommentId { get; set; }
        public DateTime ParseDate { get; set; }
    }
}