using System;
using System.Collections.Generic;

namespace LexibotLogic.Parsers
{
    public interface IWordParser
    {
        Dictionary<string, int> GetWordOccurrences(List<string> words);
        List<string> GetWordsInComment(string text);
    }
}
