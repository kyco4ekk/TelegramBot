using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.DataBase
{
    class ActualPlansDB
    {
        private MySqlConnection connection = new MySqlConnection(DataConfig.connection);
        public void SetData(string message, string date)
        { 
           connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO `deadlinesdb` (`Number`, `ProblemName`, `TotalTime`, `DateAdded`) VALUES (@NU, @PN, @TT, @DA);", connection);

            command.Parameters.Add("@NU", MySqlDbType.Int32).Value = Number() + 1;
            command.Parameters.Add("@PN", MySqlDbType.VarChar).Value = message;

            string[] engDate;
            if (date != null)
            {
                engDate = date.Split('.');
                command.Parameters.Add("@DA", MySqlDbType.Date).Value = $"{engDate[2]}-{engDate[1]}-{engDate[0]}";
                command.Parameters.Add("@TT", MySqlDbType.VarChar).Value = TimeInterval(date);
            }
            else
            {
                command.Parameters.Add("@DA", MySqlDbType.Date).Value = null;
                command.Parameters.Add("@TT", MySqlDbType.VarChar).Value = "-";
            }

            command.ExecuteNonQuery();

            connection.Close();
        }
        public void DeletedData(string deadLine)
        {
            connection.Open();

            int num = int.Parse(deadLine);

            MySqlCommand command = new MySqlCommand($"DELETE FROM deadlinesdb WHERE Number = {num}", connection);
            command.ExecuteNonQuery();

            UpdateDataAfterDeleted(num);

            connection.Close();
        }

        private static string TimeInterval(string mess)
        {
            string[] time = mess.Split('.');
            DateTime date = new DateTime(int.Parse(time[2]), int.Parse(time[1]), int.Parse(time[0]));

            string days = ((date - DateTime.Now).Days + 1).ToString();
            return days;
        }

        private int Number()
        {
            if (NullChecker())
            {
                MySqlCommand command = new MySqlCommand($"SELECT Number FROM deadlinesdb ORDER BY Number DESC LIMIT 1", connection);
                int i = Convert.ToInt32(command.ExecuteScalar());
                return i;
            }
            else
                return 0;
        }

        private bool NullChecker()
        {
            MySqlCommand command = new MySqlCommand($"SELECT COUNT(*) FROM deadlinesdb", connection);
            return Convert.ToBoolean(command.ExecuteScalar());
        }
        private void UpdateDataAfterDeleted(int num)
        {
            MySqlCommand command = new MySqlCommand($"UPDATE deadlinesdb SET Number = Number - 1 WHERE Number > {num}", connection);
            command.ExecuteNonQuery();
        }
        private int CountPositionOnDB()
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand($"SELECT COUNT(*) FROM deadlinesdb", connection);
            int N = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return N;
        }
        public void UpdateDeadLinesTime()
        {
            int N = CountPositionOnDB();

            connection.Open();

            for (int i = 1; i < N + 1; i++)
            {
                //SELECT lesson FROM lessons WHERE day IN('{time}') ORDER BY `lessons`.`lessonNum` ASC
                MySqlCommand command1 = new MySqlCommand($"SELECT DateAdded, TotalTime FROM deadlinesdb WHERE Number = {i}", connection);
                MySqlDataReader reader = command1.ExecuteReader();

                string str = "", symb = "";
                while (reader.Read())
                {
                    str += $"{reader[0]}";
                    symb += $"{reader[1]}";
                }
                reader.Close();

                if (symb == "-")
                    continue;

                str = str.Remove(10);

                //UPDATE catalog_product_entity_decimal SET value= 1222 WHERE entity_id= 759
                MySqlCommand command2 = new MySqlCommand($"UPDATE deadlinesdb SET TotalTime = {TimeInterval(str)} WHERE Number = {i}", connection);
                command2.ExecuteNonQuery();


            }
            connection.Close();   
        }
        public string LookAllDeadLines()
        {           
            if (CountPositionOnDB() != 0)
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"SELECT ProblemName, TotalTime FROM deadlinesdb ORDER BY deadlinesdb.Number ASC", connection);
                int i = 1;

                MySqlDataReader reader = command.ExecuteReader();

                string str = "";
                while (reader.Read())
                {
                    if (reader[1].ToString() != "-")
                        str += $"{i++}. {reader[0]} - срок {reader[1]}\n";
                    else
                        str += $"{i++}. {reader[0]}\n";
                }
                reader.Close();

                return str;
            }
            else
                return "Вы выполнили все планы!";
        }
    }
}
