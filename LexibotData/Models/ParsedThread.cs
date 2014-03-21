using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class ParsedThread
    {
        [Key]
        public string ThreadId { get; set; }
        public DateTime ParseDate { get; set; }
    }
}