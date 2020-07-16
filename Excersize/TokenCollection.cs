using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excersize
{
    public class TokenCollection : ICollection<Token> 
    {
        Token[] tokens = new Token[0];
        //public int Count = 0;

        public bool IsReadOnly => throw new NotImplementedException();



        public int Count
            => tokens.Length;
            
        public TokenCollection()
        {

        }
        public TokenCollection(IEnumerable<Token> enumerable)
        {
            tokens = enumerable.ToArray();
        }
        public Token this[int index]
        {
            get
            {
                return tokens[index];
            }
            set
            {
                tokens[index] = value;
            }
        }
        public void Add(Token item)
        {
            Token[] temp = new Token[tokens.Length+1];
            for (int i = 0; i < tokens.Length; i++)
            {
                temp[i] = tokens[i];
            }
            temp[temp.Length - 1] = item;
            tokens = temp;
        }

        public void Clear()
        {
            tokens = new Token[0];
        }

        public bool Contains(Token item)
        {
            foreach (var token in tokens)
            {
                if (token.Lexeme == item.Lexeme)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Contains(string Lexeme)
        {
            foreach(var token in tokens)
            {
                if (token.Lexeme == Lexeme) return true;
            }
            return false;
        }
        public void CopyTo(Token[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Token> GetEnumerator()
        {
            foreach(var token in tokens)
            {
                yield return token;
            }
        }

        public bool Remove(Token item)
        {
            Token[] temp = new Token[tokens.Length-1];
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == item) continue;
                try
                {
                    temp[i] = tokens[i];
                }
                catch
                {
                    return false;
                }
            }
            tokens = temp;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
