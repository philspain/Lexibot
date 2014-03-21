using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class Word
    {
        [Key]
        public string Token { get; set; }
        public string Definition { get; set; }
        public DateTime DateAdded { get; set; }
    }
}