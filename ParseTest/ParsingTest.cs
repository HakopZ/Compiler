using System;
using Xunit;
using Excersize;

namespace ParseTest
{
    public class ParsingTest
    {
        static string path = @"text.txt";
        [Fact]
        public void TestIfParseWorked()
        {
            ParseTry parse = new ParseTry();
            Assert.True(parse.GetParse(path));
        }
    }
}
