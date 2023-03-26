using MySql.Data.MySqlClient;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBot.Commands
{
    class FindResultsRandomNumbersGame : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/результаты", "/results", "/game results", "/результаты игры" };

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;

            await client.SendTextMessageAsync(e.Message.Chat.Id, "Результаты пользователей (топ 5)");

            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=proger;SSL Mode=None");

            connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT userName, result FROM results ORDER BY result DESC LIMIT 5", connection);            
            MySqlDataReader reader = command.ExecuteReader();

            int i = 1;
            while (reader.Read())
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"{i}. {reader[0]}  |* {reader[1]} *|");
                i++;
            }
                      
            reader.Close();
            connection.Close();        
        }
    }
}
