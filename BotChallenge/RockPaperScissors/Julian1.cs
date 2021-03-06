﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Julian1 : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();//
        int i = 0;

        int[] count = new int[3];


        string juliansRandom = "34j8ajw39hghqaesrugyoawaefw34fjkrgegau<eifh<a7e4hfaesu43w9aufjjauoe498araf7y8zoertzhaesifhsaeftgaes874atzoes8r7a348e<gaetut4sr78gfzyaertegiurtgwse54jg89seh45tgs89eh5t98th4etdorjg";

        bool lastWin;

        public Julian1()
        {
            //Start

            i = 0;
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            //Loop
            Decision d;

            d = (Decision)(juliansRandom[(i + r) % juliansRandom.Length] % 3);


            if (i > 0)
            {
                count[(int)this.enemy.LastDecision]++;

                if (i > 10)
                {
                    if (count.Count(f => f == 0) == 2)
                    {
                        int onlyToken = count[0] > 0 ? 0 : count[1] > 0 ? 1 : 2;

                        d = (Decision)(((int)enemy.LastDecision + 1) % 3);
                    }
                }
            }

            i++;

            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
