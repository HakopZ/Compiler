using System;
using Xunit;
using Excersize;

namespace ParseTest
{
    public class ParsingTest
    {
        static string path = @"D:\Visual Studio 2019 Projects\MakeParse\Excersize\text.txt";
        [Fact]
        public void TestIfParseWorked()
        {
            Parse parse = new Parse();
            Assert.True(parse.GetParse(path));
        }
    }
}
