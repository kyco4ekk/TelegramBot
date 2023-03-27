using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Collections.Generic;
using TelegramBot.DataBase;

namespace TelegramBot.Commands
{
    class Program
    {
        private const string firstMessage = "Доступный список команд\n" +
            "1.1 /fa\n" +
            "1.2 /stop_fa\n" +
            "1.3 /fa_results\n" +
            "2.1 /city\n" +
            "2.2 /stopCityGame\n" +
            "3 /weather [указать город]*\n" +
            "*[ ] - означает, что внутри скобок нужно указать требуемый запрос\n(скобки прописывать не нужно)\n" +
            "4 /m [указать пример формата:\n(ax + by)^n]\n" +
            "5 /game\n";
        
        private static TelegramBotClient client;
        private static List<Commands> commands;
        private static DB db;
        private static CityGameDB cityGameDB = new CityGameDB();
        public static Random rand;
        public static bool check = false;
        public static bool scPlay = false;        
        public static int counter = 0;
        public static int number = 0;        

        [Obsolete]        
        static void Main(string[] args)
        {
            rand = new Random();
            db = new DB();
            client = new TelegramBotClient(Config.Token);
            commands = new List<Commands>
            {
                new Weather(),
                new MathPowerEquations(),
                new RandomNumbersGame(),
                new FindResultsRandomNumbersGame(),
                new TimeTableLessons(),
                new FastArithmetic(),
                new ArithmeticResults(),
                new ScandinavianBattle(),
                new CityGame(),
                new ActualPlans()
            };           

            client.StartReceiving();
            client.OnMessage += OnMessageHandler;           
            Console.WriteLine("Bot started");
            Console.ReadLine();
            client.StartReceiving();
        }        
        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {            
            var message = e.Message;

            if (message.Text != null)
            {               
                if(message.Text == "еее кочевники")
                    await client.SendTextMessageAsync(message.Chat.Id, "еее\U0001F918");
                if (message.Text == "/start")
                    await client.SendTextMessageAsync(message.Chat.Id, $"{firstMessage}");
                try
                {
                    db.Connection(e);
                }
                catch (Exception)
                {

                } //adding to the database

                Console.WriteLine($"Пришло новое сообщение от {message.From.FirstName} {message.From.LastName} | {message.Chat.Username} с текстом: \"{message.Text}\" из группы {message.Chat.Title}{message.Chat.FirstName} получено в {DateTime.Now.Hour.ToString("00")}:{DateTime.Now.Minute.ToString("00")}:{DateTime.Now.Second.ToString("00")}");

                try
                {
                    if (message.Text == "бан")
                    {
                        await client.KickChatMemberAsync(message.Chat.Id, message.ReplyToMessage.From.Id);
                    }                       
                }
                catch (Exception)
                {
                        await client.SendTextMessageAsync(message.Chat.Id, "бан?");
                } //kiked in chat

                if (message.Text == "/game")
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Игра \"Угадай число\" началась! (1-10)");
                    number = rand.Next(1, 11);                   
                    check = true;

                    Console.WriteLine(number);
                }

                if (message.Text == "/fa")
                    FastArithmetic.mathMarathon = true;

                if (message.Text == "/Battle")
                    scPlay = true;

                string[] str = message.Text.Split();
                if (str[0] == "/city" && CityGame.cityGame == false)
                {
                    cityGameDB.CleanDB();
                    CityGame.cityGame = true;
                }

                foreach (var comm in commands)
                {
                    if (comm.Contains(message.Text))                   
                        comm.ExecuteAsync(client, e);                                            
                }                
            }
        }
    }
}
