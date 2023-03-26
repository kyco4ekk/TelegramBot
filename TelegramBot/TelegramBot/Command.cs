using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace TelegramBot.Commands
{
    public abstract class Commands
    {
        public abstract string[] Names { get; set; }

        [System.Obsolete]
        public abstract void ExecuteAsync(TelegramBotClient client, MessageEventArgs e);        
        
        public bool Contains(string message)
        {
            foreach (var mess in Names)
            {
                if (message.Contains(mess))                
                    return true;                
            }
            return false;
        }

    }
}
