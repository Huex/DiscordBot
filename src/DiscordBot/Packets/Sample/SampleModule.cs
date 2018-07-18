using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Packets.Sample
{
    /// <summary>
    /// В модуле желательно описывать только взаимодествие с дискордом,
    /// так как при выполнении команды, каждый раз создается новый экземпляр этого класса,
    /// так устроен Discord.Net.Commands :^)
    /// </summary>
    [Name("Sample")]
    public class SampleModule : ModuleBase
    {
        private SampleService _service;

        public SampleModule(SampleService service)
        {
            _service = service;
        }

        [Name("ToLog"), Command("ToLog")]
        public async Task ToLogAsync(params string[] message)
        {
            await _service.ToLogAsync(String.Join(" ", message));
            await this.Context.Channel.SendMessageAsync("Yes i did this :^)");
        }

        [Name("ToFile"), Command("ToFile")]
        public async Task ToFile(params string[] message)
        {
            _service.WriteToFile(String.Join(" ", message));
            await this.Context.Channel.SendMessageAsync("Well well i did this :^/ ");
        }

        [Name("FromFile"), Command("FromFile")]
        public async Task FromFile()
        {
            var res = _service.GetFromFileAsync();
            if (res != null)
            {
                await this.Context.Channel.SendMessageAsync($"Look at that: {_service.GetFromFileAsync()} prety nice ;^) ");
            }
            else
            {
                await this.Context.Channel.SendMessageAsync($"Sorry i cant ((((( ");
            }
        }

    }
}
