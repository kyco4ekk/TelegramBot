using MySql.Data.MySqlClient;
using System;
using Telegram.Bot.Args;

namespace TelegramBot.DataBase
{
    class DB
    {        
        [Obsolete]
        public void Connection(MessageEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(DataConfig.connection);

            connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO `tgusers` (`userFirstName`, `userId`, `message`, `chatTitle`, `dateTime`) VALUES (@uFN, @uI, @mess, @chT, @dt);", connection);

            var message = e.Message;
         
            command.Parameters.Add("@uFN", MySqlDbType.VarChar).Value = message.From.FirstName;            
            command.Parameters.Add("@uI", MySqlDbType.VarChar).Value = message.From.Id;
            command.Parameters.Add("@mess", MySqlDbType.Text).Value = message.Text;
            command.Parameters.Add("@chT", MySqlDbType.VarChar).Value = GetChatTitle(e);
            command.Parameters.Add("@dt", MySqlDbType.VarChar).Value = DateTime.Now;

            command.ExecuteNonQuery();
            connection.Close();

            /*
            Queryes queryes = new Queryes();

            MySqlCommand command = new MySqlCommand("INSERT INTO `tgusers` (`id`, `message`, `dateTime`) VALUES (@id, @mess, @dt);", queryes.getConnection());

            var message = e.Message;
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = message.From.FirstName;
            command.Parameters.Add("@mess", MySqlDbType.Text).Value = message.Text;
            command.Parameters.Add("@dt", MySqlDbType.VarChar).Value = DateTime.Now;

            queryes.openConnection();

            Console.WriteLine("Принято без ошибок!");

            queryes.closeConnection();
            */
        }
        [Obsolete]
        private string GetChatTitle(MessageEventArgs e)
        {
            return e.Message.Chat.Title ?? e.Message.Chat.FirstName;
        }
        
    }
}
