using MySql.Data.MySqlClient;
using System;
using Telegram.Bot.Args;
using TelegramBot.Commands;

namespace TelegramBot.DataBase
{
    class Arithmetic
    {
        [Obsolete]
        public void Connection(MessageEventArgs e)
        {
            var message = e.Message;

            MySqlConnection connection = new MySqlConnection(DataConfig.connection);

            if(CheckUser(message.From.Id, connection))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"UPDATE `mathusers` SET `completeExamp` = `completeExamp` + 1 WHERE `userId` = {message.From.Id};", connection);

                MySqlCommand command1 = new MySqlCommand($"SELECT Ticks FROM `mathusers` WHERE userId = {message.From.Id};", connection);                
                long i = Convert.ToInt32(command1.ExecuteScalar());

                //"UPDATE `mathusers` SET Address = @add, City = @cit Where FirstName = @fn and LastName = @add"
                if (FastArithmetic.ts.Ticks > i)
                {            
                    MySqlCommand command2 = new MySqlCommand($"UPDATE `mathusers` SET `hardestExamp` = @hE, `longestTime` = @lT, `Ticks` = @Ti WHERE `userId` = @uI;", connection);
                    
                    command2.Parameters.Add("@hE", MySqlDbType.VarChar).Value = $"{FastArithmetic.num1} {FastArithmetic.s} {FastArithmetic.num2}";
                    command2.Parameters.Add("@uI", MySqlDbType.VarChar).Value = message.From.Id;
                    command2.Parameters.Add("@lT", MySqlDbType.VarChar).Value = FastArithmetic.elapsedTime;
                    command2.Parameters.Add("@Ti", MySqlDbType.VarChar).Value = FastArithmetic.ts.Ticks;

                    command2.ExecuteNonQuery();
                }

                command.ExecuteNonQuery();
                command1.ExecuteNonQuery();
                connection.Close();
            }   
            else
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("INSERT INTO `mathusers` (`userName`, `userId`, `completeExamp`, `hardestExamp`, `longestTime`, `Ticks`) VALUES (@uN, @uI, @cE, @hE, @lT, @Ti);", connection);

                command.Parameters.Add("@uN", MySqlDbType.VarChar).Value = message.From.FirstName;
                command.Parameters.Add("@uI", MySqlDbType.VarChar).Value = message.From.Id;
                command.Parameters.Add("@cE", MySqlDbType.Int32).Value = 1;
                command.Parameters.Add("@hE", MySqlDbType.VarChar).Value = $"{FastArithmetic.num1} {FastArithmetic.s} {FastArithmetic.num2}";
                command.Parameters.Add("@lT", MySqlDbType.VarChar).Value = FastArithmetic.elapsedTime;
                command.Parameters.Add("@Ti", MySqlDbType.VarChar).Value = FastArithmetic.ts.Ticks;

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        //Have userId in table ? 
        private bool CheckUser(long userId, MySqlConnection connection)
        {

            MySqlCommand command = new MySqlCommand($"SELECT id FROM mathusers WHERE userId = {userId};", connection);

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
