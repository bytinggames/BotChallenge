using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class JulianOld : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();//
        int i = 0;

        string juliansRandom = "34j8ajw39hghqaesrugyoawaefw34fjkrgegau<eifh<a7e4hfaesu43w9aufjjauoe498araf7y8zoertzhaesifhsaeftgaes874atzoes8r7a348e<gaetut4sr78gfzyaertegiurtgwse54jg89seh45tgs89eh5t98th4etdorjg";

        public JulianOld()
        {
            //Start

            i = r % juliansRandom.Length;
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            //Loop
            Decision d;

            d = (Decision)(juliansRandom[i] % 3);

            

            i++;
            if (i == juliansRandom.Length)
                i = 0;
            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
