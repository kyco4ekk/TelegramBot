using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBot.DataBase;

namespace TelegramBot.Commands
{    
    class FastArithmetic : Commands
    {
        public static int num1;
        public static int num2;
        public static int result;
        public static char[] symb = new char[] { '+', '-' };
        public static char s;
        public static bool mathMarathon = false;
        public static string elapsedTime;
        public static TimeSpan ts;
        private bool cont = true;
        private bool stopContent;
        private Random rand = new Random();
        private Stopwatch stopWatch;
        public Arithmetic arithmetic;

        public override string[] Names { get; set; } = new string[] { "" };

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            arithmetic = new Arithmetic();
            

            if (mathMarathon == true)
            {
                if (message.Text == result.ToString())
                {
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    
                    elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", 
                        ts.Minutes, 
                        ts.Seconds, 
                        ts.Milliseconds / 10);
                    
                    await client.SendTextMessageAsync(message.Chat.Id, $"Все верно! Время решения примера: {elapsedTime}");
                    arithmetic.Connection(e);

                    cont = true;
                }

                if (cont == true)
                {
                    stopContent = true;
                    stopWatch = new Stopwatch();
                    stopWatch.Start();

                    num1 = rand.Next(0, 101);
                    num2 = rand.Next(0, 101);
                    s = symb[rand.Next(0, 2)];                    

                    if (s == '+')
                        result = num1 + num2;
                    else
                        result = num1 - num2;

                    await client.SendTextMessageAsync(message.Chat.Id, $"{num1} {s} {num2}");
                    cont = false;
                }
                if (message.Text != "/stop_fa" && stopWatch.ElapsedTicks > 50000000 && stopContent == true && !char.IsDigit(message.Text[message.Text.Length - 1]))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"Бот обрабатывает только положительные и отрицательные цифры\n\nИспользуй \"/stop_fa\" - для того, чтобы покинуть игру");
                    stopContent = false;
                }
                if (message.Text == "/stop_fa")
                {
                    mathMarathon = false;
                    await client.SendTextMessageAsync(message.Chat.Id, $"Поток примеров приостановлен, возвращайся скорее!");
                    await client.DeleteMessageAsync(message.Chat.Id, message.MessageId - 1);
                    stopContent = false;
                    cont = true;                    
                }                
            }
        }
    }
}
