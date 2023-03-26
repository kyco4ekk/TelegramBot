using MySql.Data.MySqlClient;
using System;
using Telegram.Bot.Args;

namespace TelegramBot.DataBase
{
    class ScandinavianGameDB
    {
        public void Connection(CallbackQueryEventArgs ev, MySqlConnection connection)
        {          
            if (CheckUser(ev.CallbackQuery.From.Id, connection))
            {
                Console.WriteLine("Произошла ошибка. Возможно все места заняты, либо вы уже в игре");
            }
            else
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("INSERT INTO `scusers` (`userName`, `userId`, `numWarrior`) VALUES (@uN, @uI, @nW);", connection);
                command.Parameters.Add("@uN", MySqlDbType.VarChar).Value = ev.CallbackQuery.From.FirstName;
                command.Parameters.Add("@uI", MySqlDbType.VarChar).Value = ev.CallbackQuery.From.Id.ToString();

                int number = Convert.ToInt32(ev.CallbackQuery.Data[0].ToString());
                command.Parameters.Add("@nW", MySqlDbType.Int32).Value = number;

                command.ExecuteNonQuery();
                connection.Close();

                Console.WriteLine($"Пользователь {ev.CallbackQuery.From.FirstName} выбрал воина {ev.CallbackQuery.Data.Substring(1, ev.CallbackQuery.Data.Length - 1)}");
            }

        }
        //Have userId in table ? 
        private bool CheckUser(long userId, MySqlConnection connection)
        {

            MySqlCommand command = new MySqlCommand($"SELECT id FROM scusers WHERE userId = {userId};", connection);

            connection.Open();

            int i = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            if (i == 0)
                return false;
            else
                return true;
        }          
    }
}
