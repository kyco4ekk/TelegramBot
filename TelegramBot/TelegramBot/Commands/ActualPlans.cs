using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBot.DataBase;

namespace TelegramBot.Commands
{
    class ActualPlans : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/планы добавить", "/планы удалить", "/планы обновить", "/планы посмотреть" };

        public async override void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;

            //await client.SendTextMessageAsync(message.Chat.Id, $"{}");

            string[] userRequest = message.Text.Split();
            string text = null, date = null;
            for (int i = 2; i < userRequest.Length; i++)
            {
                if (char.IsDigit(userRequest[i][0]))
                    date = userRequest[i];
                else
                    text += userRequest[i] + " ";                
            }
            if(text != null)
                text = text.Substring(0, text.Length - 1);

            ActualPlansDB plan = new ActualPlansDB();
            try
            {
                if (userRequest[1].ToLower().Replace(" ", "") == "добавить")
                {
                    plan.SetData(text, date);
                    await client.SendTextMessageAsync(message.Chat.Id, $"Added success!");
                }
                else if (userRequest[1].ToLower().Replace(" ", "") == "удалить")
                {
                    plan.DeletedData(date);
                    await client.SendTextMessageAsync(message.Chat.Id, $"Deleted success!");
                }
                else if (userRequest[1].ToLower().Replace(" ", "") == "обновить")
                {
                    plan.UpdateDeadLinesTime();
                    await client.SendTextMessageAsync(message.Chat.Id, $"Updated!");
                }
                else if(userRequest[1].ToLower().Replace(" ", "") == "посмотреть")
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"{plan.LookAllDeadLines()}");
                }
                else
                    await client.SendTextMessageAsync(message.Chat.Id, $"Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
