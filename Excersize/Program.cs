using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Excersize
{
    class Program
    {

        enum KeyWords
        {
            KeyWord,
            Operators,
            Punctuation,
            Identifier
        }


        static void Main(string[] args)
        {
            Dictionary<string, KeyWords> dictionary = new Dictionary<string, KeyWords>();
            List<KeyValuePair<string, KeyWords>> pairs = new List<KeyValuePair<string, KeyWords>>();
            dictionary.Add("public", KeyWords.KeyWord);
            dictionary.Add("static", KeyWords.KeyWord);
            dictionary.Add("class", KeyWords.KeyWord);
            dictionary.Add("int", KeyWords.KeyWord);
            dictionary.Add("string", KeyWords.KeyWord);
            dictionary.Add("//", KeyWords.Punctuation);
            dictionary.Add("return", KeyWords.KeyWord);
            dictionary.Add("{", KeyWords.Punctuation);
            dictionary.Add("}", KeyWords.Punctuation);
            dictionary.Add(";", KeyWords.Punctuation);
            dictionary.Add("(", KeyWords.Punctuation);
            dictionary.Add(")", KeyWords.Punctuation);
            dictionary.Add("+", KeyWords.Operators);
            dictionary.Add("-", KeyWords.Operators);
            dictionary.Add("=", KeyWords.Operators);

            string[] text = File.ReadAllLines(@"\\GMRDC1\Folder Redirection\Hakop.Zarikyan\Documents\Visual Studio 2019\Projects\Excersize\Excersize\text.txt");

            ReadOnlySpan<string> AllText = new ReadOnlySpan<string>(text);
            string t = "";
            AllText.Trim(" ");
            for (int i = 0; i < AllText.Length; i++)
            {
                var word = AllText[i].AsSpan();
                string name = "";

                for (int a = 0; a < AllText[i].Length; a++)
                {
                    var character = word.Slice(a, 1);

                    if (dictionary.ContainsKey(character.ToString()))
                    {
                        pairs.Add(new KeyValuePair<string, KeyWords>(character.ToString(), dictionary[character.ToString()]));
                        pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));
                        name = "";
                    }
                    name += character.ToString();
                    
                    if (dictionary.ContainsKey(name))
                    {
                        pairs.Add(new KeyValuePair<string, KeyWords>(name, dictionary[name]));
                        name = "";
                    }
                    if (character.IsWhiteSpace() && name.Length > 0 && name != " ") 
                    {
                        pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));
                        name = "";
                    }
                    if(name == " ")
                    {
                        name = default;
                    }


                }

                pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));

            }

        }
    }

}
