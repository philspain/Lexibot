using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LexibotData.Models
{
    public class LexibotEntities : DbContext
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<BannedWord> BannedWords { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ParsedThread> ParsedThreads { get; set; }
        public DbSet<WordOccurrence> WordOccurrences { get; set; }
        private static LexibotEntities _dbContext;
    }
}