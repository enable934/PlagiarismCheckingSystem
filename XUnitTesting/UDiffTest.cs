using DiffMatchPatch;
using System;
using System.Linq;
using Xunit;

namespace XUnitTesting
{
    public class UDiffTest
    {
        [Theory]
        [InlineData("Another-string")]
        [InlineData("Another-strings")]
        [InlineData("Another-one-string")]
        [InlineData("Another-one")]
        public void EqualTwoStringTest(string str)
        {
            //Arrange
            var dmp = new diff_match_patch();
            var anotherString = "Another-string";

            //Act
            var diffs = dmp.diff_main(str, anotherString);
            var equals = diffs.Where((Diff elem) => (elem.operation == Operation.EQUAL && !string.IsNullOrWhiteSpace(elem.text)));
            var deletes = diffs.Where((Diff elem) => (elem.operation == Operation.DELETE && !string.IsNullOrWhiteSpace(elem.text)));
            var inserts = diffs.Where((Diff elem) => (elem.operation == Operation.INSERT && !string.IsNullOrWhiteSpace(elem.text)));
            var equalsLength = equals.Aggregate(0, (acc, x) => acc + x.text.Length);
            var deletesLength = deletes.Aggregate(0, (acc, x) => acc + x.text.Length);
            var insertsLength = inserts.Aggregate(0, (acc, x) => acc + x.text.Length);

            //Assert
            Assert.True(equalsLength > deletesLength + insertsLength);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("hello_world")]
        [InlineData("qwerty-one-string")]
        public void NotEqualTwoStringTest(string str)
        {
            //Arrange
            var dmp = new diff_match_patch();
            var anotherString = "Another-string";

            //Act
            var diffs = dmp.diff_main(str, anotherString);
            var equals = diffs.Where((Diff elem) => (elem.operation == Operation.EQUAL && !string.IsNullOrWhiteSpace(elem.text)));
            var deletes = diffs.Where((Diff elem) => (elem.operation == Operation.DELETE && !string.IsNullOrWhiteSpace(elem.text)));
            var inserts = diffs.Where((Diff elem) => (elem.operation == Operation.INSERT && !string.IsNullOrWhiteSpace(elem.text)));
            var equalsLength = equals.Aggregate(0, (acc, x) => acc + x.text.Length);
            var deletesLength = deletes.Aggregate(0, (acc, x) => acc + x.text.Length);
            var insertsLength = inserts.Aggregate(0, (acc, x) => acc + x.text.Length);

            //Assert
            Assert.True(equalsLength < deletesLength + insertsLength);
        }

        [Theory]
        [InlineData("String")]
        public void HtmlOutputEqualTest(string str)
        {
            //Arrange
            var dmp = new diff_match_patch();
            var anotherString = "String";

            //Act
            var diffs = dmp.diff_main(str, anotherString);
            var html = dmp.diff_prettyHtml(diffs);

            //Assert
            Assert.Contains("span", html);
            Assert.DoesNotContain("del", html);
            Assert.DoesNotContain("ins", html);
            Assert.Equal("<span>String</span>", html);
        }

        [Theory]
        [InlineData("Btring")]
        [InlineData("Strinf")]
        [InlineData("Stsing")]
        public void HtmlOutputNotEqualDelInsTest(string str)
        {
            //Arrange
            var dmp = new diff_match_patch();
            var anotherString = "String";

            //Act
            var diffs = dmp.diff_main(str, anotherString);
            var html = dmp.diff_prettyHtml(diffs);

            //Assert
            Assert.Contains("del", html);
            Assert.Contains("ins", html);
        }
    }
}
