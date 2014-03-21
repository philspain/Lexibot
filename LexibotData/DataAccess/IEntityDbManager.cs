using System;
using System.Collections.Generic;

using LexibotData.Models;

namespace LexibotData.DataAccess
{
    public interface IEntityDbManager
    {
        void AddParsedThread(string link);
        void AddWordOccurrences(Dictionary<string, int> wordOccurrences, string subReddit, DateTime createdUTC);
        List<LexibotData.Models.WordOccurrenceDTO> GetWordOccurrences();
        void AddConfig(string settingId, string settingValue);
        Config GetCookieConfig();
        bool IsThreadParsed(string linkId);
        int ThreadOccurrences(string linkId);
    }
}
