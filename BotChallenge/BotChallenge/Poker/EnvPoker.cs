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

                r = rPlus++;
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

            static int rPlus = 0;
            int r;

            public string GetName() { return GetType().Name + r; }
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
            int bettingRound = 0;

            while (true)
            {
                //Big Round start
                Console.Write("\n_________________________________________________\nBetting Round " + (bettingRound + 1) + ":\n");

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

                    Console.Write("\n_____________________\nBots: ");
                    
                    for (int i = 0; i < bots.Length; i++)
                    {
                        if (bots[i].ingame)
                        {
                            if (bots[i].money > 0)
                                Console.ForegroundColor = ConsoleColor.Gray;
                            else
                                Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.Write(bots[i].cards[0] + " " + bots[i].cards[1] + "    ");
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\nTable: ");


                    for (int i = 0; i < cardsOnTable.Count; i++)
                    {
                        //Console.ForegroundColor = cardsOnTable[i].GetColor();
                        Console.Write(cardsOnTable[i] + " ");
                    }
                    Console.WriteLine();
                    Console.Write("Pot:  " + bots.Sum(f => f.moneySetTotal));

                    Console.WriteLine("\n");

                    if (botsIngame.Count > 1)
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

                                    Console.Write(botsIngame[turn].GetName() + " bet " + botsIngame[turn].moneySetRound);
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
                                    Console.WriteLine(botsIngame[turn].GetName() + " left");


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
                                    Console.WriteLine(botsIngame[turn].GetName() + " checked");
                                }
                            }

                            turn++;
                            if (turn == botsIngame.Count)
                                turn = 0;
                            if (turnIndex == 1)
                                turnIndex = 2;
                            else if (turnIndex == 2)
                                turnIndex = 0;

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

                if (botsIngame.Count == 1)
                {
                    //  give last bot all money (not more, than he bet)
                    for (int i = 0; i < bots.Length; i++)
                    {
                        int get = Math.Min(bots[i].moneySetTotal, botsIngame[0].moneySetTotal);
                        botsIngame[0].money += get;
                        bots[i].moneySetTotal -= get;
                    }
                }
                else
                {
                    //check who has the bast cards
                    // TODO
                    
                    for (int i = 0; i < botsIngame.Count; i++)
                    {
                        for (int j = 0; j < checks.Length; j++)
                        {
                            botsIngame[i].score = checks[j](cardsOnTable.Concat(botsIngame[i].cards).ToArray());

                            if (botsIngame[i].score != -1)
                            {
                                botsIngame[i].check = j;
                                break;
                            }
                        }
                    }

                    List<Bot> best = new List<Bot>() { botsIngame[0] };
                    for (int i = 1; i < botsIngame.Count; i++)
                    {
                        if (botsIngame[i].check < best[0].check)
                            best = new List<Bot>() { botsIngame[i] };
                        else if (botsIngame[i].check == best[0].check)
                        {
                            if (botsIngame[i].score > best[0].score)
                            {
                                best = new List<Bot>() { botsIngame[i] };
                            }
                            else if (botsIngame[i].score == best[0].score)
                            {
                                best.Add(botsIngame[i]);
                            }
                        }
                    }

                    //best = bots.Skip(1).ToList();

                    for (int i = 0; i < bots.Length; i++)
                    {
                        bots[i].check = 0;
                    }
                    for (int i = 0; i < best.Count; i++)
                    {
                        best[i].check = 1;
                    }

                    best = best.OrderByDescending(f => f.moneySetTotal).ToList();

                    int upper = 0;
                    int upperCount;
                    int part;
                    for (int i = 1; i < best.Count; i++)
                    {
                        if (best[i].moneySetTotal < best[upper].moneySetTotal)
                        {
                            // getting money round
                            int ueberschuss = 0;
                            int winDistance = bots[upper].moneySetTotal - best[i].moneySetTotal;
                            // collecting money
                            for (int j = 0; j < bots.Length; j++)
                            {
                                if (bots[j].check == 0 && bots[j].moneySetTotal > best[i].moneySetTotal)
                                {
                                    int get = bots[j].moneySetTotal - best[i].moneySetTotal;
                                    bots[j].moneySetTotal -= get;
                                    ueberschuss += get;
                                }
                            }

                            // serving money
                            upperCount = i - upper;
                            part = ueberschuss / upperCount;
                            for (int k = i - 1; k >= upper; k--)
                            {
                                best[k].money += part;
                                ueberschuss -= part;
                                best[k].moneySetTotal -= winDistance;
                                best[k].money += winDistance;
                            }
                            //bots[upper].money += ueberschuss; // rest if not divisionable
                            ueberschuss = 0;
                        }
                    }

                    // serving money
                    upperCount = best.Count;
                    part = bots.Sum(f => f.moneySetTotal) / upperCount;
                    for (int i = 0; i < bots.Length; i++)
                    {
                        bots[i].moneySetTotal = 0;
                    }
                    for (int i = 0; i < best.Count; i++)
                    {
                        best[i].money += part;
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

                Console.ReadLine();

                bettingRound++;
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
            return Math.Max(cards[5].Number, cards[6].Number);
        }
    }
}
