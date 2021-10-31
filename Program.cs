using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ChampionsLeagueDraw
{
    class Program
    {
        static readonly int bagNumber = 4;
        static readonly int bagCapacity = 8;
        static readonly int groupNumber = 8;
        static readonly int groupCapacity = 4;
        static readonly int maxGoal = 9;
        static readonly Random rand = new Random();
        static readonly string[,] teamAndNation = new string[32, 2] { { "Bayern Munich", "Almanya" }, { "FK Buducnost Podgorica", "Karadağ" }, { "Real Madrid", "İspanya" }, { "BSC Young Boys", "İsviçre" }, { "Juventus", "İtalya" }, { "Paris Saint-Germain", "Fransa" }, { "Zenit", "Rusya" }, { "Porto", "Portekiz" }, { "Barcelona", "İspanya" }, { "FK Senica", "Slovakya" }, { "AZ Alkmaar", "Hollanda" }, { "Manchester United", "İngiltere" }, { "Borussia Dortmund", "Almanya" }, { "Shakhtar Donetsk", "Ukrayna" }, { "Chelsea", "İngiltere" }, { "Ajax", "Hollanda" }, { "Dynamo Kiev", "Ukrayna" }, { "Red Bull Salzburg", "Avusturya" }, { "NK Celje", "Slovenya" }, { "FK Astana", "Kazakistan" }, { "Olympiacos", "Yunanistan" }, { "Lazio", "İtalya" }, { "Krasnodar", "Rusya" }, { "Ferençvaroş", "Macaristan" }, { "Lokomotiv Moskova", "Rusya" }, { "Marseille", "Fransa" }, { "Club Brugge", "Belçika" }, { "FK Partizan", "Sırbistan" }, { "Galatasaray", "Türkiye" }, { "Midtjylland", "Danimarka" }, { "Rennes", "Fransa" }, { "Ferencváros", "Macaristan" } };

        static void Main(string[] args) //...
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            List<Team> teams = new List<Team>();
            teams = CreateTeamList(teamAndNation);

            SortedList<int, List<Team>> bag = new SortedList<int, List<Team>>(CreateBag(teams));
            PrintTeams(bag, "Torbalar");

            SortedList<int, List<Team>> groups = new SortedList<int, List<Team>>(CreateRandomGroups(bag));
            PrintTeams(groups, "Gruplar");

            Match(groups);

            SortedList<int, List<Team>> fixture = new SortedList<int, List<Team>>(OrderTeamGroups(groups));
            PrintTeams(fixture, "Fikstür");

            SortedList<int, List<Team>> elimination = new SortedList<int, List<Team>>(EliminationTeams(fixture));
            PrintTeams(elimination, "Üst tur takımları");

            watch.Stop();
            Console.WriteLine($"Geçen Süre: {watch.ElapsedMilliseconds} ms");
        }


        private static void PrintTeams(SortedList<int, List<Team>> teams, string message) // takımları yazdırır
        {
            for (int i = 0; i < teams.Keys.Count; i++)
            {
                Console.WriteLine(message + " " + (i + 1) + "--------------");
                foreach (var item in teams[i])
                {
                    Console.WriteLine("takım ismi= {0}, ülke ismi= {1}, puanı= {2}, attığı gol={3}", item.TeamName, item.Nation, item.Point, item.Scored);
                }
            }
            Console.WriteLine("\n");
        }

        public static SortedList<int, List<Team>> EliminationTeams(SortedList<int, List<Team>> groups) //gruplardaki takımların eleme işlemi
        {
            SortedList<int, List<Team>> eliminatedTeams = new SortedList<int, List<Team>>();
            List<Team> selectedGroupList = new List<Team>();
            for (int p = 0; p < groupNumber; p++)
            {
                selectedGroupList = groups[p];

                for (int i = 0; i < groupCapacity; i++)
                {
                    if (selectedGroupList[i].Point == selectedGroupList[i + 1].Point)
                    {
                        if (selectedGroupList[i].Scored == selectedGroupList[i + 1].Scored)
                        {
                            if ((selectedGroupList[i].Scored - selectedGroupList[i].UnScored) == (selectedGroupList[i + 1].Scored - selectedGroupList[i + 1].UnScored))
                            {
                                bool coin = Convert.ToBoolean(rand.Next(0, 2));
                                if (coin)
                                    eliminatedTeams.Add(p, selectedGroupList.Take(2).ToList());
                                else
                                    eliminatedTeams.Add(p, selectedGroupList.Take(2).Reverse().ToList());
                            }
                            else
                            {
                                eliminatedTeams.Add(p, selectedGroupList.Take(2).OrderByDescending(x => x.Scored).ThenByDescending(x => x.Scored - x.UnScored).ToList());
                                break;
                            }
                        }
                        else
                        {
                            eliminatedTeams.Add(p, selectedGroupList.Take(2).OrderByDescending(x => x.Scored).ToList());
                            break;
                        }
                    }
                    else
                    {
                        eliminatedTeams.Add(p, selectedGroupList.Take(2).ToList());
                        break;
                    }
                }
            }
            return (eliminatedTeams);
        }

        public static SortedList<int, List<Team>> OrderTeamGroups(SortedList<int, List<Team>> groups) //gruptaki takımları sıralar
        {
            SortedList<int, List<Team>> orderedTeams = new SortedList<int, List<Team>>();

            for (int i = 0; i < groups.Keys.Count(); i++)
            {
                orderedTeams.Add(i, groups[i].OrderByDescending(x => x.Point).ToList());
            }
            return (orderedTeams);
        }

        public static void Match(SortedList<int, List<Team>> groups) //Maçları yap
        {
            int homeGaol = 0;
            int awayGoal = 0;

            for (int p = 0; p < groupNumber; p++)
            {
                for (int i = 0; i < groupCapacity; i++)
                {
                    for (int z = 0; z < groupCapacity; z++)
                    {
                        if (z != i)
                        {
                            awayGoal = rand.Next(0, maxGoal);
                            groups[p][z].Scored += awayGoal;
                            groups[p][i].UnScored += awayGoal;

                            homeGaol = rand.Next(0, maxGoal);
                            groups[p][i].Scored += homeGaol;
                            groups[p][z].UnScored += homeGaol;

                            if (homeGaol > awayGoal)
                            {
                                groups[p][i].Point += 3;
                                Console.WriteLine("Ev sahibi {0}, {1} ile oynadığı maçı {2} - {3} kazandı. ", groups[p][i].TeamName, groups[p][z].TeamName, homeGaol, awayGoal);
                            }
                            else if (awayGoal > homeGaol)
                            {
                                groups[p][z].Point += 3;
                                Console.WriteLine("Ev sahibi {0}, {1} ile oynadığı maçı {2} - {3} kaybetti. ", groups[p][i].TeamName, groups[p][z].TeamName, homeGaol, awayGoal);

                            }
                            else
                            {
                                groups[p][i].Point += 1;
                                groups[p][z].Point += 1;
                                Console.WriteLine("Ev sahibi {0}, {1} ile oynadığı maçta {2} - {3} berabere kaldı. ", groups[p][i].TeamName, groups[p][z].TeamName, homeGaol, awayGoal);
                            }
                        }
                    }
                }
            }
        }

        public static SortedList<int, List<Team>> CreateBag(List<Team> teams) // torba oluştur
        {
            SortedList<int, List<Team>> bag = new SortedList<int, List<Team>>();
            for (int i = 0; i < bagNumber; i++)
            {
                bag.Add(i, teams.GetRange((i * bagCapacity), bagCapacity));
            }
            return (bag);

        }

        public static SortedList<int, List<Team>> CreateRandomGroups(SortedList<int, List<Team>> bag) // torbadan random gruplarını seç ve ata
        {
            SortedList<int, List<Team>> groups = new SortedList<int, List<Team>>();
            Team selectedTeam = new Team();
            int randomTeamGet = 0; // torbadan çekilecek takımın indexi
            int randomTeamSet = 0; // takımın atılacağı grubun indexi
            bool inHand = false;

            for (int p = 0; p < groupCapacity; p++)
            {
                for (int i = 0; i < groupNumber; i++)
                {
                    randomTeamSet = rand.Next(0, groupNumber);

                    if (!inHand)
                    {
                        randomTeamGet = rand.Next(0, groupNumber - i);
                        selectedTeam = bag[p][randomTeamGet];
                        bag[p].Remove(selectedTeam);
                    }

                    if (p == 0)
                    {
                        if (!groups.Keys.Contains(randomTeamSet))
                        {
                            groups.Add(randomTeamSet, new List<Team>() { selectedTeam });
                            inHand = false;
                        }
                        else
                        {
                            i--;
                            inHand = true;
                        }
                    }

                    else
                    {
                        if ((groups[randomTeamSet].Count() <= p) && (!groups[randomTeamSet].Any(x => x.Nation == selectedTeam.Nation)))
                        {
                            groups[randomTeamSet].Add(selectedTeam);
                            inHand = false;
                        }
                        else
                        {
                            i--;
                            inHand = true;
                        }
                    }
                }
            }
            return (groups);

        }

        private static Team CreateTeam(string _teamName, string _nationName) //takım oluştur
        {
            Team team = new Team()
            {
                TeamName = _teamName,
                Nation = _nationName
            };
            return (team);
        }

        private static List<Team> CreateTeamList(string[,] teamAndNation) //takım listesi oluştur
        {
            List<Team> fillTeams = new List<Team>();
            for (int i = 0; i < (teamAndNation.Length / 2); i++)
            {
                fillTeams.Add(CreateTeam(teamAndNation[i, 0], teamAndNation[i, 1]));
            }
            return (fillTeams);
        }

    }
}

