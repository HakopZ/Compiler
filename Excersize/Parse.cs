using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Excersize
{
    public class Parse
    {
        public enum KeyWords
        {
            KeyWord,
            Operators,
            Punctuation,
            Identifier,
            WhiteSpace
        }
        public Dictionary<string, KeyWords> dictionary = new Dictionary<string, KeyWords>();
        public List<KeyValuePair<string, KeyWords>> pairs = new List<KeyValuePair<string, KeyWords>>();
        
        
        

        public Parse()
        {
           
            AddToDictionary();
        }
        void AddToDictionary()
        {
            dictionary.Add("public", KeyWords.KeyWord);
            dictionary.Add("static", KeyWords.KeyWord);
            dictionary.Add("class", KeyWords.KeyWord);
            dictionary.Add("int", KeyWords.KeyWord);
            dictionary.Add("var ", KeyWords.KeyWord);
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
            dictionary.Add(" ", KeyWords.WhiteSpace);
            dictionary.Add("    ", KeyWords.Punctuation);
        }
        public bool GetParse(string path)
        {
            ReadOnlySpan<string> AllText = new ReadOnlySpan<string>(File.ReadAllLines(path));
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
                        if (a == AllText[i].Length-1)
                        {
                            pairs.Add(new KeyValuePair<string, KeyWords>(character.ToString(), dictionary[character.ToString()]));

                        }
                        else 
                        {
                            string temp = "";
                            if (a <= AllText[i].Length - 5)
                            {
                                temp = word.Slice(a, 4).ToString();
                                if(dictionary.ContainsKey(temp))
                                {
                                    pairs.Add(new KeyValuePair<string, KeyWords>(temp, dictionary[temp]));
                                    a += 3;
                                    continue;
                                }
                            }
                            temp = character.ToString() + word.Slice(a + 1, 1).ToString();
                            if(dictionary.ContainsKey(temp))
                            {
                                pairs.Add(new KeyValuePair<string, KeyWords>(temp, dictionary[temp]));
                                if (temp == "//")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                pairs.Add(new KeyValuePair<string, KeyWords>(character.ToString(), dictionary[character.ToString()]));
                            }
                        }
                        
                        name = "";
                        continue;

                    }
                    name += character.ToString();
                    if (dictionary.ContainsKey(name))
                    {
                        
                        pairs.Add(new KeyValuePair<string, KeyWords>(name, dictionary[name]));
                        name = "";
                    }
                }
                //last item in string
                if (name != "")
                {
                    pairs.Add(new KeyValuePair<string, KeyWords>(name, KeyWords.Identifier));
                }
                pairs.Add(new KeyValuePair<string, KeyWords>("\\n", KeyWords.Punctuation));

            }
            if(pairs.Count == 0)
            {
                return false;
            }
            foreach (var item in pairs)
            {
                if (item.Key == "    ")
                {
                    t += $"\\t : {item.Value}\n";
                }
                else
                {
                    t += $"{item.Key} : {item.Value}\n";
                }
            }
            File.WriteAllText(@"Output.txt", t);
            return true;
        }

    }
}
