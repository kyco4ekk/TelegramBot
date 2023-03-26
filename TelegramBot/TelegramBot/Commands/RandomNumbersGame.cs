using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBot.DataBase;

namespace TelegramBot.Commands
{
    class RandomNumbersGame : Commands
    {
        public override string[] Names { get; set; } = new string[] { "" };
        public UpdateUsers update;

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            update = new UpdateUsers();

            if (Program.check == true) //if user wrote "/game"
            {
                Program.counter++; //count messages before "/game"
                if (message.Text == Program.number.ToString()) 
                {                   
                    Program.check = false;
                    await client.SendTextMessageAsync(message.Chat.Id, $"{message.From.FirstName} {message.From.LastName} угадал число {Program.number}");
                    update.Connection(e); 
                }
                else if(Program.counter > 25)       
                {
                    Program.counter = 0;
                    await client.SendTextMessageAsync(message.Chat.Id, $"Игра окончена, никто не угадал число {Program.number}");
                }
            }
        }
    }
}
