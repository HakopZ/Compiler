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
            dictionary.Add("/", KeyWords.Operators);
            dictionary.Add(",", KeyWords.Punctuation);
            string[] text = File.ReadAllLines(@"D:\Visual Studio 2019 Projects\MakeParse\Excersize\text.txt");

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
                        if (name != null && name.Length > 0)
                        {
                            pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));
                        }
                        pairs.Add(new KeyValuePair<string, KeyWords>(character.ToString(), dictionary[character.ToString()]));
                        name = "";
                        continue;
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
                    if (name == " ")
                    {
                        name = default;
                    }


                }
                if (name != "")
                {
                    pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));
                }

            }
            foreach (var item in pairs)
            {
                t += $"{item.Key} : {item.Value}\n";
            }
            File.WriteAllText(@"Output.txt", t);
        }
    }

}
