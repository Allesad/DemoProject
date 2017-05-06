using System;
using Xunit;

namespace YumaPos.Core.Tests
{
    public class SampleTest
    {
        [Fact]
        public void CheckTest()
        {
            Assert.Equal("It works!", Sample.Ping());
        }
    }
}
