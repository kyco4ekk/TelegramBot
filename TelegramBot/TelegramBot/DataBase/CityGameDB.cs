using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.DataBase
{
    class CityGameDB
    {
        private MySqlConnection connection = new MySqlConnection(DataConfig.connection);

        public string GetCity(int idSymb) //Get random city in dataBase
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand($"SELECT City FROM cityalphabetgame WHERE idSymb = {idSymb} ORDER BY RAND() LIMIT 1", connection);

            string randomCity = command.ExecuteScalar().ToString();

            connection.Close();

            return randomCity;
        }

        public bool ExistCity(string city) //Check on "true city"
        {
            city = CorrectWordFormat(city);

            connection.Open();

            MySqlCommand command = new MySqlCommand($"SELECT EXISTS(SELECT id FROM cityalphabetgame WHERE City = '{city}')", connection);
            bool exist = Convert.ToBoolean(command.ExecuteScalar());

            connection.Close();

            return exist;
        }        

        public bool CheckCityOnRepeat(string city, long chatId)
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand($"SELECT EXISTS(SELECT id FROM repeatcity WHERE city = '{city}' AND chatId = {chatId})", connection);
            bool exist = Convert.ToBoolean(command.ExecuteScalar());

            connection.Close();

            return exist;
        } 

        public void AddCity(string city, long chatId, long userId)
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO `repeatcity` (`city`, `userId`, `chatId`) VALUES (@ci, @uI, @chI);", connection);
            
            command.Parameters.Add("@ci", MySqlDbType.VarChar).Value = city;
            command.Parameters.Add("@uI", MySqlDbType.Int64).Value = userId;
            command.Parameters.Add("@chI", MySqlDbType.Int64).Value = chatId;            

            command.ExecuteNonQuery();
            connection.Close();
        } 

        public void CleanDB()
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE `repeatcity`", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        private string CorrectWordFormat(string word)
        {
            string[] words = word.Split().ToArray();

            string reg = "";
            for (int i = 0; i < words.Length; i++)
            {
                for (int j = 0; j < words[i].Length; j++)
                {
                    if (j == 0)
                        reg += words[i][j].ToString().ToUpper();
                    else
                        reg += words[i][j].ToString().ToLower();
                }
                if (i + 1 != words.Length)
                    reg += " ";
            }

            return reg;
        }
    }
}
