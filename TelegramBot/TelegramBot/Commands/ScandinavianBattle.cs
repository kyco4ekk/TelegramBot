using MySql.Data.MySqlClient;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.DataBase;
using TelegramBot.Interfaces;

namespace TelegramBot.Commands
{   
    class Barbarian : Skills
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public string Name { get; } = "Barbarian";

        public Barbarian(int health, int damage)
        {
            Health = health;
            Damage = damage;
        }
    }
    class Outlaw : Skills
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public string Name { get; } = "Outlaw";
        public Outlaw(int health, int damage)
        {
            Health = health;
            Damage = damage;
        }        
    }
    class Archer : Skills
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public string Name { get; } = "Archer";
        public Archer(int health, int damage)
        {
            Health = health;
            Damage = damage;
        }        
    }

    class ScandinavianBattle : Commands
    {
        private Random rand = new Random();
        private ScandinavianGameDB scDB = new ScandinavianGameDB();
        private MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=proger;SSL Mode=None");
        private int counter = 0;
        private bool check = false;
        Skills[] sk;

        public override string[] Names { get; set; } = new string[] { "" };

        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            Skills[] sk = { new Barbarian(40, 10), new Outlaw(30, 14), new Archer(25, 16) };
            if (Program.scPlay == true)
            {
                var message = e.Message;
                int firstTurn = rand.Next(0, 2);
                int countTurns = 0;

                InlineKeyboardMarkup myInlineKeyboard1 = new InlineKeyboardMarkup(

                            new InlineKeyboardButton[][]
                            {
                            new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithCallbackData("Barbarian","0barbarian"),
                                InlineKeyboardButton.WithCallbackData("Outlow","1outlow"),
                                InlineKeyboardButton.WithCallbackData("Archer","2archer")
                            }
                            });
                InlineKeyboardMarkup myInlineKeyboard2 = new InlineKeyboardMarkup(

                                        new InlineKeyboardButton[][]
                                        {
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Атаковать","attack"),
                                        InlineKeyboardButton.WithCallbackData("Отдыхать","pass"),
                                    }
                                        });

                if (counter == 0)
                    await client.SendTextMessageAsync(message.Chat.Id, "Воины для выбора:", replyMarkup: myInlineKeyboard1);

                client.OnCallbackQuery += async (object sc, CallbackQueryEventArgs ev) =>
                {
                    if (CountCols(connection) < 2 && Program.scPlay == true)
                    {
                        scDB.Connection(ev, connection);
                        if (CountCols(connection) == 2)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, $"Игра началась! {GetNameUser(connection)[firstTurn]} сделает первый ход!");
                            await client.SendTextMessageAsync(message.Chat.Id, "Сделать ход:", replyMarkup: myInlineKeyboard2);
                            check = true;
                        }
                    }                    

                    if (ev.CallbackQuery.From.Id.ToString() == GetUserId(connection)[firstTurn] && check == true && Program.scPlay == true)
                    {
                        int[] arr = GetNumUser(connection).ToArray();

                        int i = arr[firstTurn];
                        int j = arr[0] + arr[1] - arr[firstTurn];

                        if (ev.CallbackQuery.Data == "attack")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, $"{GetNameUser(connection)[firstTurn]} атаковал при помощи {LookName(sk[j])}. Остаток жизни {LookName(sk[i])} - {AttackTurn(sk, i, j)}");
                        }
                        else if (ev.CallbackQuery.Data == "pass")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, $"{GetNameUser(connection)[firstTurn]} отдохнул.  XP у {LookName(sk[j])} увеличилось, теперь - {PassTurn(sk, j)}");                        
                        }
                        else
                        {
                            if (firstTurn == 0)
                                firstTurn++;
                            else
                                firstTurn--;
                        }

                        if (firstTurn == 0)
                            firstTurn++;
                        else
                            firstTurn--;

                        if (LookHealth(sk[i]) <= 0)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, $"Победитель определен");

                            connection.Open();
                            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE `scusers`;", connection);
                            command.ExecuteNonQuery();
                            connection.Close();

                            Program.scPlay = false;                           
                        }
                    }
                };
            }
        }
        private int LookHealth(Skills skills)
        {
            return skills.Health;           
        }
        private int LookDamage(Skills skills)
        {
            return skills.Damage;
        }
        private string LookName(Skills skills)
        {
            return skills.Name;
        }

        // i = атака на кого | j = атака кем
        private int AttackTurn(Skills[] sk, int i, int j) 
        {
            sk[i].Health -= sk[j].Damage;
            return sk[i].Health > 0 ? sk[i].Health : 0;
        }

        // i = этому воину увеличивается здоровье на 20%
        private int PassTurn(Skills[] sk, int i)
        {
            sk[i].Health = (int)(sk[i].Health * 1.2);
            return sk[i].Health;
        }

        //Count users in game ?
        private int CountCols(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM scusers;", connection);

            connection.Open();

            int i = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return i;
        }

        private string[] GetNameUser(MySqlConnection connection)
        {

            MySqlCommand command = new MySqlCommand("SELECT userName FROM scusers;", connection);

            connection.Open();
            
            MySqlDataReader reader = command.ExecuteReader();

            int i = 0;
            string[] item = new string[2];

            while (reader.Read())
            {
                item[i] = reader[0].ToString();
                i++;
            }

            reader.Close();
            connection.Close();
            return item;
        }
        private int[] GetNumUser(MySqlConnection connection)
        {

            MySqlCommand command = new MySqlCommand("SELECT numWarrior FROM scusers;", connection);       

            connection.Open();
            
            MySqlDataReader reader = command.ExecuteReader();

            int i = 0;
            int[] item = new int[2];    

            while (reader.Read())
            {
                item[i] = Convert.ToInt32(reader[0]);
                i++;
            }

            reader.Close();
            connection.Close();
            return item;
        }

        private string[] GetUserId(MySqlConnection connection)
        {

            MySqlCommand command = new MySqlCommand("SELECT userId FROM scusers;", connection);

            connection.Open();
            
            MySqlDataReader reader = command.ExecuteReader();

            int i = 0;
            string[] item = new string[2];

            while (reader.Read())
            {
                item[i] = reader[0].ToString();
                i++;
            }

            reader.Close();
            connection.Close();
            return item;
        }        
    }
}
