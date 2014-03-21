using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LexibotData.Models
{
    public class Config
    {
        [Key]
        public string SettingId { get; set; }
        public string Values { get; set; }

        public static Dictionary<string, string> GetValueDictionary(string values)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            string[] valueArray;
            string[] keyValueArray;

            if (values.Contains(";"))
            {
                valueArray = values.Split(';');
            }
            else
            {
                valueArray = new string[1] { values };
            }

            foreach(string s in valueArray)
            {
                keyValueArray = s.Split('=');
                tempDict.Add(keyValueArray[0], keyValueArray[1]);
            }

            return tempDict;
        }
    }

}