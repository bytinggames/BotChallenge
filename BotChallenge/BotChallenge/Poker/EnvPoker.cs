using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BotChallenge.Poker
{
    public enum KA
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    public class EnvPoker : Env
    {
        public abstract class Bot
        {
            internal int money = 100;
            internal int moneySetRound;
            internal int moneySetTotal;

            internal Card[] cards;

            internal bool ingame;
            internal float score;
            internal int check;

            public bool GetIngame()
            {
                return ingame;
            }

            protected Card GetCard(int index)
            {
                if (index < 0)
                    index = 0;
                if (index > 1)
                    index = 1;
                return cards[index];
            }

            protected EnvPoker env;

            public int GetMoney()
            {
                return money; 
            }

            public int GetMoneySet()
            {
                return moneySetRound;
            }

            internal void Initialize(Bot[] enemies, EnvPoker env)
            {
                this.enemies = enemies;
                this.env = env;

                id = idPlus++;
            }

            protected Bot[] enemies;

            internal int GetInternalAction(int minimum)
            {
                int addMinimum = minimum - moneySetRound;

                int action = GetAction(addMinimum);

                if (action < -1)
                    action = -1;

                //if (smallBlind && action == -1)
                //{
                //    action = addMinimum;
                //}

                if (action != -1 && action < addMinimum)
                {
                    action = addMinimum;
                }

                if (action > money)
                    action = money;

                return action;
            }
            protected abstract int GetAction(int addMinimum);

            static int idPlus = 0;
            internal int id;

            public string GetName() { return GetType().Name + id; }
        }
        
        Bot[] bots;

        Random rand = constRand;

        Func<Card[], float>[] checks;

        //int moneyPot;
        public int GetMoneyPot()
        {
            return bots.Sum(f => f.moneySetTotal);
        }
        
        public EnvPoker(Type[] botTypes)
        {
            bots = GetBots<Bot>(botTypes);

            for (int i = 0; i < bots.Length; i++)
            {
                List<Bot> botsList = bots.ToList();
                botsList.RemoveAt(i);
                bots[i].Initialize(botsList.ToArray(), this);
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;


            checks = new Func<Card[], float>[]
            {
                CheckStraightFlush,
                CheckFourOfAKind,
                CheckFullHouse,
                CheckFlush,
                CheckThreeOfAKind,
                CheckTwoPair,
                CheckPair,
                CheckHighestCard,
            };
        }



    List<Card> cards;

        List<Card> cardsOnTable;

        public Card GetCardOnTable(int index)
        {
            return cardsOnTable[index];
        }
        public Card[] GetCardsOnTable()
        {
            return cardsOnTable.ToArray();
        }


        enum Symbol
        {
            Karo,
            Herz,
            Pik,
            Kreuz
        }

        internal override float[] Loop()
        {
            string output = "";
            for (int i = 0; i < bots.Length; i++)
            {
                output += bots[i].GetName();
                if (i + 1 < bots.Length)
                    output += " VS ";
            }
            Console.WriteLine(output);


            // Game start
            int gameRound = 0;

            while (true)
            {
                //Big Round start
                Console.ReadLine();
                Console.Write("\n_________________________________________________\nGame Round " + (gameRound + 1) + ":\n");

                Console.Write("\nMoney: ");

                for (int i = 0; i < bots.Length; i++)
                {
                    Console.Write(bots[i].money + "      ");
                }

                cardsOnTable = new List<Card>();
                cards = new List<Card>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        cards.Insert(constRand.Next(cards.Count), new Card(j + 2, i));
                    }
                }

                for (int i = 0; i < bots.Length; i++)
                {
                    bots[i].cards = new Card[2];
                    bots[i].cards[0] = cards[0]; cards.RemoveAt(0);
                    bots[i].cards[1] = cards[0]; cards.RemoveAt(0);

                    bots[i].ingame = true;
                }

                List<Bot> botsIngame = bots.ToList();

                int smallBlind = 1;

                int turn = 0;
                Bot lastRaised = bots[(turn + 2)%bots.Length];
                for (int round = 0; round < 4; round++)
                {
                    //Small Round start

                    Console.ReadLine();
                    Console.Write("\n_____________________\nBots: ");
                    
                    for (int i = 0; i < bots.Length; i++)
                    {
                        if (bots[i].money == 0 && bots[i].moneySetRound == 0 && bots[i].moneySetTotal == 0)
                            bots[i].ingame = false;

                        if (bots[i].ingame)
                        {
                            if (bots[i].money > 0)
                                Console.ForegroundColor = ConsoleColor.White;
                            else
                                Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.Write(bots[i].cards[0] + " " + bots[i].cards[1] + "    ");
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\t\tTable: ");


                    for (int i = 0; i < cardsOnTable.Count; i++)
                    {
                        //Console.ForegroundColor = cardsOnTable[i].GetColor();
                        Console.Write(cardsOnTable[i] + " ");
                    }
                    Console.WriteLine("\t\t\t\tPot:  " + bots.Sum(f => f.moneySetTotal));

                    Console.Write("\t");
                    for (int i = 0; i < bots.Length; i++)
                    {
                        if (bots[i].ingame)
                        {
                            if (bots[i].money > 0)
                                Console.ForegroundColor = ConsoleColor.White;
                            else
                                Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.Write(bots[i].money + "\t");
                    }
                    Console.WriteLine();

                    Console.Write("\t");
                    for (int i = 0; i < bots.Length; i++)
                    {
                        if (bots[i].ingame)
                        {
                            if (bots[i].money > 0)
                                Console.ForegroundColor = ConsoleColor.White;
                            else
                                Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.Write(bots[i].moneySetTotal + "\t");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;


                    Console.WriteLine("\n" + new string('.', 30));
                    Console.ReadLine();

                    if (botsIngame.Count(f => f.money > 0) > 1)
                    {
                        int minimum = round == 0 ? smallBlind : 0; // TODO: small blind can bet too next round!
                        int turnIndex = 1;
                        bool forced = false;
                        do
                        {
                            if (botsIngame[turn].money > 0)
                            {
                                int action;
                                forced = false;

                                if (round == 0 && (turnIndex == 1 || turnIndex == 2))
                                {
                                    action = turnIndex * smallBlind;
                                    if (action > botsIngame[turn].money)
                                        action = botsIngame[turn].money;

                                    forced = true;
                                }
                                else
                                {
                                    action = botsIngame[turn].GetInternalAction(minimum);
                                }


                                if (action > 0)
                                {

                                    botsIngame[turn].moneySetRound += action;
                                    botsIngame[turn].money -= action;

                                    Console.Write(new string('\t', botsIngame[turn].id + 1) + botsIngame[turn].moneySetRound);
                                    //Console.Write(botsIngame[turn].GetName() + " bet " + botsIngame[turn].moneySetRound);
                                    if (botsIngame[turn].money == 0)
                                        Console.WriteLine(" ALL IN!");
                                    else
                                        Console.WriteLine();

                                    if (botsIngame[turn].moneySetRound > minimum)
                                    {
                                        minimum = botsIngame[turn].moneySetRound;
                                        if (!forced)
                                            lastRaised = botsIngame[turn];
                                    }

                                }
                                else if (action < 0)
                                {
                                    Console.WriteLine(new string('\t', botsIngame[turn].id + 1) + "X");
                                    //Console.WriteLine(botsIngame[turn].GetName() + " left");


                                    if (botsIngame[turn] == lastRaised)
                                    {
                                        if (turn == 0)
                                            lastRaised = botsIngame[botsIngame.Count - 1];
                                        else
                                            lastRaised = botsIngame[turn - 1];
                                    }


                                    botsIngame[turn].ingame = false;
                                    botsIngame.RemoveAt(turn--);
                                }
                                else
                                {
                                    Console.WriteLine(new string('\t', botsIngame[turn].id + 1) + "-");
                                    //Console.WriteLine(botsIngame[turn].GetName() + " checked");
                                }
                            }
                            else if (botsIngame[turn].moneySetTotal == 0 && botsIngame[turn].moneySetRound == 0)
                            {
                                botsIngame[turn].ingame = false;
                            }

                            turn++;
                            if (turn == botsIngame.Count)
                                turn = 0;
                            if (turnIndex == 1)
                                turnIndex = 2;
                            else if (turnIndex == 2)
                                turnIndex = 0;

                            Console.ReadLine();
                        } while (forced || botsIngame[turn] != lastRaised && botsIngame.Count > 1);


                        for (int i = 0; i < bots.Length; i++)
                        {
                            //moneyPot += bots[i].moneySetRound;
                            bots[i].moneySetTotal += bots[i].moneySetRound;
                            bots[i].moneySetRound = 0;
                        }
                    }

                    if (round == 0)
                    {
                        cardsOnTable.AddRange(cards.Take(3));
                        cards.RemoveRange(0, 3);
                    }
                    else if (round == 1 || round == 2)
                    {
                        cardsOnTable.Add(cards[0]);
                        cards.RemoveAt(0);
                    }
                }


                Console.Write("\t");
                for (int i = 0; i < bots.Length; i++)
                {
                    if (bots[i].ingame)
                    {
                        if (bots[i].money > 0)
                            Console.ForegroundColor = ConsoleColor.White;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                    Console.Write(bots[i].moneySetTotal + "\t");
                }
                Console.ForegroundColor = ConsoleColor.Gray;


                Console.WriteLine("\n" + new string('.', 30));
                Console.WriteLine(new string('\t', 12) + "Pot:  " + bots.Sum(f => f.moneySetTotal));


                if (botsIngame.Count == 1)
                {
                    //  give last bot all money (not more, than he bet)
                    int getFromEachPlayerMax = botsIngame[0].moneySetTotal;
                    int getTotal = 0;
                    for (int i = 0; i < bots.Length; i++)
                    {
                        int get = Math.Min(bots[i].moneySetTotal, getFromEachPlayerMax);
                        getTotal += get;
                        bots[i].moneySetTotal -= get;
                    }

                    Console.WriteLine("GAIN: ");
                    Console.Write(new string('\t', botsIngame[0].id + 1) + "+"+getTotal);

                    botsIngame[0].money += getTotal;
                    getTotal = 0;
                    Console.WriteLine();
                }
                else
                {
                    //check who has the bast cards
                    // TODO
                    // TODO: all in can't get more than he set (distribute rest to... dunno?)
                    
                    for (int i = 0; i < botsIngame.Count; i++)
                    {
                        for (int j = 0; j < checks.Length; j++)
                        {
                            botsIngame[i].score = checks[j](cardsOnTable.Concat(botsIngame[i].cards).ToArray());

                            if (botsIngame[i].score != -1)
                            {
                                botsIngame[i].check = j;

                                Console.WriteLine(new string('\t', botsIngame[i].id + 1) + checks[j].Method.Name.Remove(0, "Check".Length) + " (" + botsIngame[i].score + ")");
                                break;
                            }
                        }
                    }
                    Console.WriteLine();
                    Console.ReadLine();

                    List<List<Bot>> best = new List<List<Bot>>();
                    best.Add(new List<Bot>() { botsIngame[0] });
                    for (int i = 1; i < botsIngame.Count; i++)
                    {
                        int j;
                        for (j = 0; j < best.Count; j++)
                        {
                            if (botsIngame[i].check < best[j][0].check)
                            {
                                best.Insert(j, new List<Bot>() { botsIngame[i] });
                                break;
                            }
                            else if (botsIngame[i].check == best[j][0].check)
                            {
                                if (botsIngame[i].score > best[j][0].score)
                                {
                                    best.Insert(j, new List<Bot>() { botsIngame[i] });
                                    break;
                                }
                                else if (botsIngame[i].score == best[j][0].score)
                                {
                                    best[j].Add(botsIngame[i]);
                                    break;
                                }
                            }
                        }
                        if (j == best.Count)
                            best.Add(new List<Bot>() { botsIngame[i] });
                    }


                    List<Bot> botsMoneyOrder = botsIngame.OrderBy(f => f.moneySetTotal).ToList();

                    List<Tuple<int, List<Bot>>> pots = new List<Tuple<int, List<Bot>>>();

                    for (int i = 0; i < botsMoneyOrder.Count; i++)
                    {
                        int moneySet = botsMoneyOrder[i].moneySetTotal;
                        if (moneySet > 0)
                        {
                            int potMoney = 0;
                            Tuple<int, List<Bot>> pot = new Tuple<int, List<Bot>>(0, new List<Bot>());
                            for (int j = i; j < botsMoneyOrder.Count; j++)
                            {
                                int botPotMoney = Math.Min(botsMoneyOrder[j].moneySetTotal, moneySet);
                                potMoney += botPotMoney;

                                botsMoneyOrder[j].moneySetTotal -= botPotMoney;

                                pot.Item2.Add(botsMoneyOrder[j]);
                            }

                            //for (int j = 0; j < bots.Length; j++)
                            //{
                            //    potMoney += Math.Min(moneySet;
                            //    bots[j].moneySetTotal -= moneySet;
                            //}
                            pot = new Tuple<int, List<Bot>>(potMoney, pot.Item2);
                            pots.Add(pot); // TODO
                        }
                    }

                    int potNr = 0;
                    foreach (var pot in pots)
                    {
                        int win = -1;
                        for (int i = 0; win == -1 && i < best.Count; i++)
                        {
                            for (int i2 = 0; win == -1 && i2 < best[i].Count; i2++)
                            {
                                for (int j = 0; j < pot.Item2.Count; j++)
                                {
                                    if (best[i][i2] == pot.Item2[j])
                                    {
                                        win = i;
                                        break;
                                    }
                                }
                            }
                        }

                        int winnerInsidePot = best[win].Count(f => pot.Item2.Contains(f));

                        int part = pot.Item1 / winnerInsidePot;
                        int overflow = pot.Item1 % winnerInsidePot;

                        Console.WriteLine("Pot Battle " + (potNr + 1) + " ("+pot.Item1+"):");
                        for (int i = 0; i < best[win].Count; i++)
                        {
                            if (pot.Item2.Contains(best[win][i]))
                            {
                                best[win][i].money += part;
                                Console.WriteLine(new string('\t', best[win][i].id + 1) + "+" + part);
                            }
                        }

                        potNr++;
                    }
                }

                for (int i = 0; i < bots.Length; i++)
                {
                    bots[i].money += bots[i].moneySetTotal;
                    bots[i].moneySetTotal = 0;
                }
                //moneyPot = 0;

                // TODO get only the money you bet
                // TODO split money on equal cards

                gameRound++;
            }


            float[] scores = new float[bots.Length];
            //for (int i = 0; i < scores.Length; i++)
            //{
            //    scores[i] = bots[i].score;
            //}
            return scores;
        }
        
        public float CheckStraightFlush(Card[] cards)
        {
            if (CheckFlush(cards) > 0f)
            {
                float score = CheckStraight(cards);
                return score;
            }
            return -1f;
        }

        public float CheckFourOfAKind(Card[] cards)
        {
            int[] numberCount = new int[13];

            for (int i = 0; i < cards.Length; i++)
            {
                numberCount[cards[i].Number - 2]++;
            }

            for (int i = numberCount.Length - 1; i >= 0; i--)
            {
                if (numberCount[i] >= 4)
                    return i;
            }

            return -1f;
        }

        public float CheckFullHouse(Card[] cards)
        {
            int[] numberCount = new int[13];
            
            for (int i = 0; i < cards.Length; i++)
            {
                numberCount[cards[i].Number - 2]++;
            }

            int three = -1, two = -1;
            for (int i = numberCount.Length - 1; i >= 0; i--)
            {
                if (three == -1 && numberCount[i] >= 3)
                    three = i;
                else if (numberCount[i] == 2)
                    two = i;
            }

            if (two == -1 || three == -1)
                return -1;
            else
                return three + two / 100f;
        }

        public float CheckFlush(Card[] cards)
        {
            int[] symbolCount = new int[4];
            int[] highest = new int[4];
            for (int i = 0; i < cards.Length; i++)
            {
                symbolCount[cards[i].Symbol]++;
                highest[cards[i].Symbol] = cards[i].Number;
            }

            for (int i = 0; i < symbolCount.Length; i++)
            {
                if (symbolCount[i] >= 5)
                    return highest[i]; // TODO: CHECK bei gleichstand nächst höchste karte
            }
            return -1f;
        }

        public float CheckStraight(Card[] cards)
        {
            return -1f;
            //TODO
            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = i + 1; j < cards.Length; j++)
                {
                    for (int k = j + 1; k < cards.Length; k++)
                    {
                        if (cards[i].Number == cards[j].Number && cards[i].Number == cards[k].Number)
                        {
                            return cards[i].Number;
                        }
                    }
                }
            }
            return -1;
        }

        public float CheckThreeOfAKind(Card[] cards)
        {
            int[] numberCount = new int[13];

            for (int i = 0; i < cards.Length; i++)
            {
                numberCount[cards[i].Number - 2]++;
            }

            for (int i = numberCount.Length - 1; i >= 0; i--)
            {
                if (numberCount[i] >= 3)
                    return i;
            }

            return -1f;
        }

        public float CheckTwoPair(Card[] cards)
        {
            int p1 = -1;
            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = i + 1; j < cards.Length; j++)
                {
                    if (cards[i].Number == cards[j].Number)
                    {
                        if (p1 == -1)
                            p1 = i;
                        else
                        {
                            return Math.Max(cards[p1].Number, cards[i].Number) + Math.Min(cards[p1].Number, cards[i].Number) / 100f;
                        }
                    }
                }
            }
            return -1;
        }

        public float CheckPair(Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = i + 1; j < cards.Length; j++)
                {
                    if (cards[i].Number == cards[j].Number)
                    {
                        return cards[i].Number;
                    }
                }
            }
            return -1;
        }

        public float CheckHighestCard(Card[] cards)
        {
            return Math.Max(cards[5].Number, cards[6].Number) + Math.Min(cards[5].Number, cards[6].Number) / 100f;
        }
    }
}
