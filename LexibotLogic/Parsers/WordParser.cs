using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using LexibotData.Models;
using LexibotData.DataAccess;
using LexibotLogic.DIContainer;

namespace LexibotLogic.Parsers
{
    public class WordParser : IWordParser
    {
        IEntityDbManager _dbManager;
        char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        // reduce number of words that are not worth reporting on
        static string[] _stopWords = new string[] {"a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", 
            "aren't", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", 
            "could", "couldn't", "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", 
            "from", "further", "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", 
            "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", 
            "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor", 
            "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own", "same", 
            "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's", "the", 
            "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", 
            "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll", "we're", "we've", 
            "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", 
            "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves"};

        public WordParser() :
            this(DependencyFactory.Resolve<IEntityDbManager>())
        { }

        WordParser(IEntityDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public List<string> GetWordsInComment(string text)
        {
            text = text.ToLower();
            List<string> words = new List<string>();
            
            if (!String.IsNullOrEmpty(text))
            {
                StringBuilder builder = new StringBuilder();
                int textLength = text.Length;
                bool wordStarted = false;

                for(int i = 0; i < text.Length; i++)
                {
                    char currChar = text[i];
                    char nextChar = (i + 1) < textLength ? text[i + 1] : '*';

                    if (wordStarted)
                    {
                        if (letters.Contains(currChar))
                        {
                            builder.Append(currChar);
                        }
                        else if (currChar == '\'' && letters.Contains(nextChar))
                        {
                            builder.Append(currChar);
                        }
                        else if (currChar == '-' && letters.Contains(nextChar))
                        {
                            builder.Append(currChar);
                        }
                        else if (builder.Length > 0 && 
                                !_stopWords.Contains(builder.ToString()))
                        {
                            words.Add(builder.ToString());
                            builder.Clear();
                            wordStarted = false;
                        }
                    }
                    else
                    {
                        if(letters.Contains(currChar))
                        {
                            builder.Append(currChar);
                            wordStarted = true;
                        }
                    }
                }
            }

            return words;
        }

        public Dictionary<string, int> GetWordOccurrences(List<string> words)
        {
            Dictionary<string, int> wordOccurrences = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (!wordOccurrences.ContainsKey(word))
                {
                    wordOccurrences[word] = 1;
                }
                else
                {
                    wordOccurrences[word]++;
                }
            }

            return wordOccurrences;
        }
    }
}