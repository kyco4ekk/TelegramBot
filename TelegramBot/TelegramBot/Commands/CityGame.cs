using Telegram.Bot;
using Telegram.Bot.Args;
using MySql.Data.MySqlClient;
using TelegramBot.DataBase;
using System;

namespace TelegramBot.Commands
{
    class CityGame : Commands
    {
        CityGameDB cityGameDB = new CityGameDB();
        Random rand = new Random();
        private bool firstTurn = true;
        public static bool cityGame = false;
        long startGameChatId = -1;
        string botCity = "";
        char[] symb = "абвгдежзийклмнопрстуфхцчшщэюя".ToCharArray();
        int num = 0;
        public override string[] Names { get; set; } = new string[] { "" };

        [Obsolete]
        public async override void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            if (e.Message.Text == "/stopCityGame" && cityGame == true)
            {
                cityGame = false;
                firstTurn = true;
                botCity = "";                
                await client.SendTextMessageAsync(e.Message.Chat.Id, $"Игра закончена, бот победил");
            }
            if (cityGame == true)
            {                
                var message = e.Message;                
                string[] exp = message.Text.Split();

                if (firstTurn == true && exp[0] == "/city" && exp.Length > 1)
                {
                    message.Text = exp[1];
                    startGameChatId = message.Chat.Id;
                    firstTurn = false;
                } //Отработка начала игры
                else if (firstTurn == true)
                {
                    message.Text = cityGameDB.GetCity(rand.Next(1, 28));
                    startGameChatId = message.Chat.Id;
                    firstTurn = false;
                }

                string userCity = message.Text;
                if (cityGameDB.ExistCity(message.Text) && message.Chat.Id == startGameChatId)
                {
                    if (cityGameDB.CheckCityOnRepeat(userCity, message.Chat.Id))
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, $"Такой город уже был. Отправь новый!", replyToMessageId: e.Message.MessageId);
                    }
                    else
                    {
                        int index = 1;
                        if (botCity != "" && (botCity[botCity.Length - 1].ToString().ToLower() == "ы" || botCity[botCity.Length - 1].ToString().ToLower() == "ь"))
                            index++;
                        if (botCity != "" && botCity[botCity.Length - index].ToString() != message.Text[0].ToString().ToLower())
    
                            await client.SendTextMessageAsync(message.Chat.Id, $"Нужен город на букву '{botCity[botCity.Length - index].ToString().ToUpper()}'", replyToMessageId: e.Message.MessageId);
                        else
                        {
                            cityGameDB.AddCity(userCity, message.Chat.Id, message.From.Id);

                            int j = 1;
                            if (userCity[userCity.Length - 1].ToString().ToLower() == "ы" || userCity[userCity.Length - 1].ToString().ToLower() == "ь")
                                j++;
                            for (int i = 0; i < symb.Length; i++)
                            {
                                if (userCity[userCity.Length - j].ToString().ToLower() == symb[i].ToString())
                                {
                                    num = i + 1;
                                    j = 1;
                                }
                                
                            }
                            
                            botCity = cityGameDB.GetCity(num);
                            cityGameDB.AddCity(botCity, message.Chat.Id, 1000);

                            await client.SendTextMessageAsync(message.Chat.Id, $"{botCity}", replyToMessageId: e.Message.MessageId);
                        }
                    }
                }
            }
        }
    }
}
