using MySql.Data.MySqlClient;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBot.Commands
{
    class TimeTableLessons : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/расписание" };

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            string time = "";

            if(DateTime.Now.Hour > 14)
               time = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            else
                time = DateTime.Now.DayOfWeek.ToString();
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=proger;SSL Mode=None");

            connection.Open();

            MySqlCommand command = new MySqlCommand($"SELECT lesson FROM lessons WHERE day IN('{time}') ORDER BY `lessons`.`lessonNum` ASC", connection);
            MySqlDataReader reader = command.ExecuteReader();

            //await client.SendTextMessageAsync(message.Chat.Id, $"{reader[0]}");
            int i = 1;
            string str = "";

            switch (time)
            {
                case "Monday":
                    str += $"*Понедельник*\n\n";
                    break;
                case "Tuesday":
                    str += $"*Вторник*\n\n";
                    break;
                case "Wednesday":
                    str += $"*Среда*\n\n";
                    break;
                case "Thursday":
                    str += $"*Четверг*\n\n";
                    break;
                case "Friday":
                    str += $"*Floppa Friday*\n\n";
                    break;
                case "Saturday":
                    str += $"*Суббота*\n\n";
                    break;
                case "Sunday":
                    str += $"*Воскресенье*\n\n";
                    break;
                default:
                    break;
            } //Translation into Russian (костыли)

            while (reader.Read())
            {
                str += $"{i}. " + reader[0].ToString() + "\n";
                i++;
            }

            try
            {
                await client.SendTextMessageAsync(message.Chat.Id, str);
            }
            catch (Exception)
            {

            }             

            reader.Close();
            connection.Close();
        }
    }
}
