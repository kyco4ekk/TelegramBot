using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBot.Commands
{
    class ArithmeticResults : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/fa_res"};

        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            try
            {
                MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=proger;SSL Mode=None");

                connection.Open();

                MySqlCommand command = new MySqlCommand($"SELECT `userName`, `completeExamp`, `hardestExamp`, `longestTime` FROM `mathusers` WHERE `userId` = {message.From.Id};", connection);
                MySqlDataReader reader = command.ExecuteReader();

                reader.Read();
                string results = $"Имя: {reader[0]} \n\n" +
                        $"Количество выполненных примеров: {reader[1]} \n\n" +
                        $"Самый сложный пример: \"{reader[2]}\" " +
                        $"был выполнен за {reader[3]}";
                await client.SendTextMessageAsync(message.Chat.Id, $"{results}");

                connection.Close();
            }
            catch (Exception)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Не было сыграно ни одной игры! Чтобы запустить \"fast arithmetic\" требуется соответствующая команда из меню: /fa");
            }
        }
    }
}
