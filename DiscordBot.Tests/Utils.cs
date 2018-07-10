using DiscordBot.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscordBot.Tests
{
    [TestClass]
    public class Utils
    {
        [TestMethod]
        public void NotifyDictonary_PropertyChangedTest()
        {
            NotifyDictonary<int, object> dictonary = new NotifyDictonary<int, object>();
            dictonary.Add(123, new object());
            dictonary.Add(321, new object());
            dictonary.Add(11, new object());
            dictonary.Add(120, new object());
            int index = 0;
            dictonary.ValueUpdated += (p) =>
            {
                index = p;
            };
            dictonary[11] = new object();

            if(index != 11)
            {
                Assert.Fail();
            }
        }
    }
}
