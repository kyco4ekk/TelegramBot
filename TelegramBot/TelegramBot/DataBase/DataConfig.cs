using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.DataBase
{
    class DataConfig
    {
        public static readonly string connection = "server=localhost;port=3306;username=root;password=root;database=proger;SSL Mode=None";
    }
}
