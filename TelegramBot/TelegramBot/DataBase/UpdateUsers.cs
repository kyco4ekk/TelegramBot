using MySql.Data.MySqlClient;
using System;
using Telegram.Bot.Args;

namespace TelegramBot.DataBase
{
    class UpdateUsers
    {
        [Obsolete]
        public void Connection(MessageEventArgs e)
        {
            var message = e.Message;

            MySqlConnection connection = new MySqlConnection(DataConfig.connection);

            /*
            -----------------------------------

            if true => result (for userId) ++ 
            else => create new userId and result = 1 

            -----------------------------------
            */

            if (CheckUser(message.From.Id, connection))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"UPDATE `results` SET `result` = `result` + 1 WHERE `userId` = {message.From.Id};", connection);

                command.ExecuteNonQuery();
                connection.Close();
            } 
            else
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("INSERT INTO `results` (`userName`, `userId`, `result`) VALUES (@uN, @uI, @res);", connection);

                command.Parameters.Add("@uN", MySqlDbType.VarChar).Value = message.From.FirstName;
                command.Parameters.Add("@uI", MySqlDbType.VarChar).Value = message.From.Id;
                command.Parameters.Add("@res", MySqlDbType.Int32).Value = 1;

                command.ExecuteNonQuery();
                connection.Close();                          
            }
        }

        //Have userId in table ? 
        private bool CheckUser(long userId, MySqlConnection connection)
        {            
            
                MySqlCommand command = new MySqlCommand($"SELECT id FROM results WHERE userId = {userId};", connection);

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

