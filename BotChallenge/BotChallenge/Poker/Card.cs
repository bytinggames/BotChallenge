using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChallenge.Poker
{
    public class Card
    {

        static readonly string[] numbers = new string[]
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10", "B", "D", "K", "A"
        };
        static readonly char[] symbols = new char[] {
            '♦','♥','♠','♣'
        };

        int number; // 11 = B, 12 = D, 13 = K, 14 = A
        int symbol; // 0 = 

        internal Card(int number, int symbol)
        {
            this.number = number;
            this.symbol = symbol;
        }

        public int Symbol => symbol;
        public int Number => number;

        internal ConsoleColor GetColor()
        {
            switch (symbol)
            {
                case 0: return ConsoleColor.Red;
                case 1: return ConsoleColor.Red;
                case 2: return ConsoleColor.White;
                default: return ConsoleColor.White;
            }
        }

        public override string ToString()
        {
            return numbers[number - 2] + symbols[symbol];
        }
    }
}
