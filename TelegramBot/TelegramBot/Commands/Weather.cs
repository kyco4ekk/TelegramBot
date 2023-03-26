using System;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading.Tasks;

namespace TelegramBot.Commands
{
    class Weather : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/погода" };

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            var mess = message.Text.Split(' ');

            if (message.Text.Length < 8)
                await client.SendTextMessageAsync(message.Chat.Id, "Местность не указана");
            else
                await client.SendTextMessageAsync(message.Chat.Id, GetWeather(mess[1]));
        }

        public string GetWeather(string Title)
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                string url = $"http://api.openweathermap.org/data/2.5/find?q={Title}&units=metric&appid=c40c6db29a7c53a8aef9fc04127c29d6";

                string data = httpClient.GetStringAsync(url).Result;

                dynamic r = JObject.Parse(data);

                return $"{r.list[0].main.temp}°C | {r.list[0].weather[0].main}";
            }
            catch (Exception)
            {
                return "Ошибка запроса";
            }
        }       
    }
}
