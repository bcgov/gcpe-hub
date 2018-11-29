using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.Hub
{
    using System.Web;
    using System.IO;
    using Telerik.Web.UI.Dictionaries;
    using Gcpe.Hub.Data.Entity;

    public class DatabaseCustomDictionarySource : ICustomDictionarySource
    {
        public string DictionaryPath { get; set; }

        public string Language { get; set; }

        public string CustomAppendix { get; set; }

        IEnumerable<string> words = null;
        int currentWordPosition = 0;

        public string ReadWord()
        {
            //first use load words into collection
            //select * from CustomDictionaryWords where user='_customAppendix' and language='_language'
            words = new List<string>();
            (words as List<string>).Add("kwijibo");

            //return a string containing a word from the custom dictionary 
            //return null when there are no more words in the custom dictionary 
            string returnword = null;
            if (currentWordPosition < words.Count())
            {
                returnword = words.ElementAt(currentWordPosition);
                currentWordPosition++;
            }
            else
            {
                currentWordPosition = 0;
            }
            return returnword;
        }

        public void AddWord(string word)
        {
            //add to database here
        }
    }
}