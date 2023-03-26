using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace TelegramBot.Commands
{
    class MathPowerEquations : Commands
    {
        public override string[] Names { get; set; } = new string[] { "/m" };

        [Obsolete]
        public override async void ExecuteAsync(TelegramBotClient client, MessageEventArgs e)
        {
            var message = e.Message;
            try
            {
                string[] expr = message.Text.Split();
                await client.SendTextMessageAsync(message.Chat.Id, Expand(expr[1]));
            }
            catch (Exception)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Что-то пошло не так");
            }
        }

        private static readonly Regex re = new Regex(@"\((-?\d*)([a-z])([\+\-]\d+)\)\^(\d+)");

        private readonly List<List<long>> nka = new List<List<long>>();
        private string Expand(string expr)
        {
            Match m = re.Match(expr);

            string sa = m.Groups[1].Value;
            int a = ("".Equals(sa) ? 1 : ("-".Equals(sa) ? -1 : int.Parse(sa)));

            string x = m.Groups[2].Value;

            string sb = m.Groups[3].Value;
            int b = "".Equals(sb) ? 0 : int.Parse(sb);

            string se = m.Groups[4].Value;
            int exp = "".Equals(se) ? 1 : int.Parse(se);
            if (exp == 0)
                return "1";

            if (exp == 1)
                return sa + x + sb;

            if (b == 0)
            {
                long coeff = (long)Math.Pow(a, exp);
                return (coeff == 1 ? "" : (coeff == -1 ? "-" : coeff.ToString())) + x + "^" + exp;
            }

            List<long> binoms = new List<long>();
            for (int i = 0; i <= exp; ++i)
                binoms.Add(nk(exp, i));

            long coeff1 = (long)Math.Pow(a, exp);
            StringBuilder terms = new StringBuilder();
            for (int i = exp; i >= 0; --i)
            {

                long coeff = coeff1 * binoms[i];

                if (i != exp && coeff > 0)
                    terms.Append('+');

                if (coeff < 0)
                    terms.Append('-');

                if ((coeff != 1 && coeff != -1) || i == 0)
                    terms.Append(coeff > 0 ? coeff : -coeff);

                if (i > 0)
                    terms.Append(x);

                if (i > 1)
                    terms.Append("^" + i);

                coeff1 = coeff1 / a * b;
            }

            return terms.ToString();
        }        
        private long nk(int n, int k)
        {

            if (n == 0 || k == 0)
                return 1;

            if (k == 1)
                return n;

            if (n - k < k)
                return nk(n, n - k);

            for (int i = nka.Count; i <= n; ++i)
                nka.Add(new List<long>());

            List<long> ns = nka[n];
            for (int i = ns.Count; i <= k; ++i)
                ns.Add(0L);

            if (ns[k] != 0)
                return ns[k];
            else
            {
                long b = nk(n - 1, k - 1) + nk(n - 1, k);
                ns[k] = b;
                return b;
            }
        }
    }
}
