using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;

using LexibotData.Models;

namespace LexibotData.DataAccess
{
    public class EntityDbManager : IEntityDbManager
    {
        // Db entity controller
        LexibotEntities _dbEntities;

        public EntityDbManager(LexibotEntities dbEntities)
        {
            _dbEntities = new LexibotEntities();
        }

        void AddWordOccurrence(string word, string occurrenceId, int occurrence, DateTime createdUTC, string subReddit)
        {
            WordOccurrence wordOccurrence = _dbEntities.WordOccurrences.FirstOrDefault(w => w.WordOccurrenceId == occurrenceId);

            if (wordOccurrence != null)
            {
                wordOccurrence.Occurrences += occurrence;
                _dbEntities.Entry<WordOccurrence>(wordOccurrence).State = EntityState.Modified;
            }
            else
            {
                wordOccurrence = new WordOccurrence() { Word = word, 
                    WordOccurrenceId = occurrenceId, 
                    OccurrenceDate = createdUTC, 
                    Occurrences = occurrence,
                    SubReddit = subReddit};

                _dbEntities.WordOccurrences.Add(wordOccurrence);
            }  
        }

        public void AddWordOccurrences(Dictionary<string, int> wordOccurrences, string subReddit, DateTime createdUTC)
        {

            foreach (string word in wordOccurrences.Keys)
            {
                string occurrenceId = word + subReddit + createdUTC;
                this.AddWordOccurrence(word, occurrenceId, wordOccurrences[word], createdUTC, subReddit);
            }

            _dbEntities.SaveChanges();
        }

        public void AddParsedThread(string link)
        {
            _dbEntities.ParsedThreads.Add(new ParsedThread { ThreadId = link, ParseDate = DateTime.Now });
            _dbEntities.SaveChanges();
        }

        public List<WordOccurrenceDTO> GetWordOccurrences() {
            string[] topWords = 
                _dbEntities.WordOccurrences
                   .GroupBy(o => o.Word)
                   .OrderByDescending(g => g.Count())
                   .Select(g => g.Key)
                   .Take(20)
                   .ToArray<string>();

            List<WordOccurrenceDTO> occurrences =
                 _dbEntities.WordOccurrences
                                .OrderBy(o => o.Word)
                                .Select(o => new WordOccurrenceDTO
                                    {
                                        Word = o.Word,
                                        OccurrenceHour = o.OccurrenceDate.Hour,
                                        Occurrences = o.Occurrences,
                                        SubReddit = o.SubReddit
                                    }
                                )
                                .Where(o => topWords.Contains(o.Word))
                                .ToList();

            return occurrences;
        }

        public void AddConfig(string settingId, string settingValue)
        {
            _dbEntities.Configs.Add(new Config { SettingId = settingId, Values = settingValue });
            _dbEntities.SaveChanges();
        }

        public Config GetCookieConfig()
        {
            return _dbEntities.Set<Config>().Find("CookieValue");
        }

        public bool IsThreadParsed(string linkId)
        {
            return _dbEntities.ParsedThreads
                                  .Select(t => t.ThreadId)
                                  .Contains(linkId);
        }

        public int ThreadOccurrences(string linkId)
        {
            return _dbEntities.ParsedThreads
                                .Where(p => p.ThreadId == linkId)
                                .Select(p => p.ThreadId)
                                .Count();
        }
    }
}