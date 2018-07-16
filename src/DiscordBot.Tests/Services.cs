using DiscordBot.Services;
using DiscordBot.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscordBot.Tests
{
    [TestClass]
    public class Services
    {
        [TestMethod]
        public void FileConfigManager_InitTest()
        {
            var manager = new FileConfigManager("stas");
            var s = manager.ConfigNames;
        }
    }
}
