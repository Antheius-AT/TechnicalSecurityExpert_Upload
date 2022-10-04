//-----------------------------------------------------------------------
// <copyright file="GameServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Giessrigl Christian.</author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using NUnit.Framework;
    using SignalRServices.ConcreteServices;
    using SignalRServices.ServiceData;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class GameServiceTests
    {
        private GameService service;

        [SetUp]
        public void Setup()
        {
            this.service = new GameService();
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetGameStoreData")]
        public bool Store_Game_Return_If_Game_Created(string gameID, int turntime)
        {
            try
            {
                GameData data = new GameData(turntime);
                service.StoreGame(gameID, data);

                if (service.games.TryGetValue(gameID, out GameData receivedData))
                    Assert.AreEqual(data, receivedData);
                else
                    return false;

                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetGameTerminateData")]
        public bool Stores_Game_Returns_If_Game_Can_Terminate(string gameID, GameData data, string playerID)
        {
            service.StoreGame(gameID, data);

            try
            {
                return service.TerminateGameAsync(gameID, playerID).Result;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetCurrentPlayerData")]
        public string Get_Current_Player_Return_Current_Player_Name(string gameID, GameData data)
        {
            service.StoreGame(gameID, data);
            return service.GetCurrentPlayer(gameID).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "ChangeCurrentPlayerData")]
        public string Change_Current_Player_Return_Current_Player_Name(string gameID, GameData data)
        {
            service.StoreGame(gameID, data);
            service.ChangeCurrentPlayerAsync(gameID);
            return service.GetCurrentPlayer(gameID).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetTurnTImeData")]
        public int Get_TurnTime_Returns_TurnTime_In_Seconds(string gameID, GameData data)
        {
            service.StoreGame(gameID, data);
            return service.GetGameTurnTime(gameID).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetIsPlayerOfGameData")]
        public bool Check_If_User_Is_Player_Of_Game(string gameID, GameData data, string playerName)
        {
            service.StoreGame(gameID, data);
            return service.IsPlayerOfGameAsync(gameID, playerName).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetIsPlayersTurnData")]
        public bool Check_If_It_Is_The_Players_Turn_Of_Game(string gameID, GameData data, string playerName)
        {
            service.StoreGame(gameID, data);
            return service.IsPlayersTurnAsync(gameID, playerName).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetVerifyPlayerData")]
        public bool Verifies_The_Player_For_This_Game_Returns_If_User_Can_Be_Player(string gameID, GameData data, string playerID, string playerName)
        {
            service.StoreGame(gameID, data);
            if (service.VerifyPlayerAsync(gameID, playerID, playerName).Result)
            {
                return service.IsPlayerOfGameAsync(gameID, playerName).Result;
            }
            else
            {
                return false;
            }
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetMakeMoveData")]
        public bool Makes_Game_Moves_Returns_If_The_Last_Executed_Game_Move_Is_Valid(string gameID, GameData data, string playerID, string playerName, int[] columns)
        {
            service.StoreGame(gameID, data);
            for (int i = 0; i < columns.Length - 1; i++)
            {
                try
                {
                    service.MakeGameMoveAsync(gameID, playerID, playerName, columns[i]);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            try
            {
                return service.MakeGameMoveAsync(gameID, playerID, playerName, columns[^1]).Result.IsValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetIsGameFullData")]
        public bool Verifies_Players_And_Checks_If_Game_Has_Enough_Players(string gameID, GameData data, string playerID, string[] playerNames)
        {
            service.StoreGame(gameID, data).GetAwaiter().GetResult();

            for (int i = 0; i < playerNames.Length; i++)
            {
                this.service.VerifyPlayerAsync(gameID, playerID, playerNames[i]).GetAwaiter().GetResult();
            }

            return this.service.IsGameFull(gameID).Result;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetCurrentGameStateData")]
        public int Makes_Some_Moves_Returns_Number_Of_Valid_Moves(string gameID, GameData data, string playerID, string playerName, int[] columns)
        {
            service.StoreGame(gameID, data);

            for (int i = 0; i < columns.Length; i++)
            {
                service.MakeGameMoveAsync(gameID, playerID, playerName, columns[i]);
            }

            return service.GetCurrentGameStateAsync(gameID).Result.ValidGameMovesDone.Count;
        }

        [Test, TestCaseSource(typeof(GameServiceTestData), "GetNonCurrentPlayersData")]
        public List<string> Stores_Game_With_Players_Returns_Players_Who_Are_Not_The_Current_Player(string gameID, GameData data)
        {
            service.StoreGame(gameID, data);
            return service.GetNonCurrentPlayers(gameID).Result;
        }
    }

    public static class GameServiceTestData
    {
        /// <summary>
        /// Params: (string gameID, int turnTime)
        /// </summary>
        public static IEnumerable GetGameStoreData
        {
            get
            {
                yield return new TestCaseData("123", 60).Returns(true);

                yield return new TestCaseData(string.Empty, 5).Returns(false);
                yield return new TestCaseData("345", -4).Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game, string playerID)
        /// </summary>
        public static IEnumerable GetGameTerminateData
        {
            get
            {
                yield return new TestCaseData("234", new GameData(5) { PlayerID = "abc" }, "abc").Returns(true);
                yield return new TestCaseData("123", new GameData(5) { PlayerID = string.Empty }, string.Empty).Returns(true);

                yield return new TestCaseData("123", new GameData(5) { PlayerID = string.Empty }, "abs").Returns(false);
                yield return new TestCaseData("345", new GameData(5) { PlayerID = "12334" }, "abc").Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with set CurrentPlayer)
        /// </summary>
        public static IEnumerable GetCurrentPlayerData
        {
            get
            {
                yield return new TestCaseData("234", new GameData(5)).Returns(string.Empty);
                yield return new TestCaseData("234", new GameData(5) { CurrentPlayer = "Christian" }).Returns("Christian");
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with set CurrentPlayer)
        /// </summary>
        public static IEnumerable ChangeCurrentPlayerData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5) 
                {   
                    Players = new string[] { "Christian", "Gregor" },
                    CurrentPlayer = "Christian"
                }).Returns("Gregor");

                yield return new TestCaseData("234", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" },
                    CurrentPlayer = "Gregor"
                }).Returns("Christian");
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game)
        /// </summary>
        public static IEnumerable GetTurnTImeData
        {
            get
            {
                yield return new TestCaseData("abs", new GameData(50)).Returns(50);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with specified Players, string possiblePlayer)
        /// </summary>
        public static IEnumerable GetIsPlayerOfGameData
        {
            get
            {
                yield return new TestCaseData("abs", new GameData(5) 
                {
                    Players = new string[] {"Christian", "Gregor"}
                }, "Christian").Returns(true);

                yield return new TestCaseData("abs", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" }
                }, "Gregor").Returns(true);

                yield return new TestCaseData("abs", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" }
                }, "Tamara").Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with specified CurrentPlayer, string possiblePlayer)
        /// </summary>
        public static IEnumerable GetIsPlayersTurnData
        {
            get
            {
                yield return new TestCaseData("abs", new GameData(5)
                {
                    CurrentPlayer = "Christian"
                }, "Christian").Returns(true);

                yield return new TestCaseData("abs", new GameData(5)
                {
                    CurrentPlayer = "Christian"
                }, "Gregor").Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with specified Players, string playerID, string possiblePlayer)
        /// </summary>
        public static IEnumerable GetVerifyPlayerData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5)
                {
                    PlayerID = "abc"
                }, "abc", "Christian").Returns(true);

                yield return new TestCaseData("234", new GameData(5)
                {
                    PlayerID = "abc"
                }, "cdf", "Gregor").Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with specified Players, string playerID, string possiblePlayer, int[] where marks should be placed)
        /// </summary>
        public static IEnumerable GetMakeMoveData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5) 
                {
                    PlayerID = "abc",
                    Players = new string[] {"Gregor", "Chrisitan"},
                    CurrentPlayer = "Christian",
                }, "abc", "Christian", new int[] {0}).Returns(true);

                yield return new TestCaseData("234", new GameData(5)
                {
                    PlayerID = "abc",
                    Players = new string[] { "Gregor", "Chrisitan" },
                    CurrentPlayer = "Christian",
                }, "abc", "Christian", new int[] { -4 }).Returns(false);

                yield return new TestCaseData("345", new GameData(5)
                {
                    PlayerID = "abc",
                    Players = new string[] { "Gregor", "Chrisitan" },
                    CurrentPlayer = "Christian",
                }, "abc", "Tamara", new int[] { 0 }).Returns(false);


                yield return new TestCaseData("456", new GameData(5)
                {
                    PlayerID = "abc",
                    Players = new string[] { "Gregor", "Chrisitan" },
                    CurrentPlayer = "Christian",
                }, "abc", "Christian", new int[] { 8 }).Returns(false);


                yield return new TestCaseData("567", new GameData(5)
                {
                    PlayerID = "abc",
                    Players = new string[] { "Gregor", "Chrisitan" },
                    CurrentPlayer = "Christian",
                }, "abc", "Christian", new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }).Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, Gamedata game with specified playerID, string playerID, string[] list of players)
        /// </summary>
        public static IEnumerable GetIsGameFullData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5)
                {
                    PlayerID = "abc"
                }, "abc", new string[] { "Christian", "Gregor" }).Returns(true);


                yield return new TestCaseData("234", new GameData(5)
                {
                    PlayerID = "abc"
                }, "abc", new string[] { "Christian", "Gregor", "Tamara" }).Returns(true);


                yield return new TestCaseData("345", new GameData(5)
                {
                    PlayerID = "abc"
                }, "abc", new string[] { "Christian" }).Returns(false);


                yield return new TestCaseData("456", new GameData(5)
                {
                    PlayerID = "abc"
                }, "xyz", new string[] { "Christian", "Gregor" }).Returns(false);


                yield return new TestCaseData("567", new GameData(5)
                {
                    PlayerID = "abc"
                }, "abc", new string[] { }).Returns(false);
            }
        }

        /// <summary>
        /// Params: (string gameID, GameData data, string playerID, string playerName, int[] columns: to be executed moves)
        /// </summary>
        public static IEnumerable GetCurrentGameStateData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5)
                {
                    Players = new string[] {"Christian", "Gregor"},
                    CurrentPlayer = "Christian",
                    PlayerID = "abc"
                }, "abc", "Christian", new int[] { 0, 1, 8, 2, 0, 0, 0, 0, 0, 0 }).Returns(8);

                yield return new TestCaseData("123", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" },
                    CurrentPlayer = "Gregor",
                    PlayerID = "abc"
                }, "abc", "Christian", new int[] { 0, 1, 8, 2, 0, 0, 0, 0, 0, 0 }).Returns(0);

                yield return new TestCaseData("123", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" },
                    CurrentPlayer = "Christian",
                    PlayerID = "abc"
                }, "xyz", "Christian", new int[] { 0, 1, 8, 2, 0, 0, 0, 0, 0, 0 }).Returns(0);

            }
        }

        /// <summary>
        /// Params: (string gameID, GameData data with specified playerID, players and currentplayer)
        /// </summary>
        public static IEnumerable GetNonCurrentPlayersData
        {
            get
            {
                yield return new TestCaseData("123", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor" },
                    CurrentPlayer = "Christian",
                    PlayerID = "abc"
                }).Returns(new List<string> { "Gregor" });

                yield return new TestCaseData("234", new GameData(5)
                {
                    Players = new string[] { "Christian", "Gregor", "Tamara", "Dawid" },
                    CurrentPlayer = "Christian",
                    PlayerID = "abc"
                }).Returns(new List<string> { "Gregor", "Tamara", "Dawid" });

                yield return new TestCaseData("345", new GameData(5)
                {
                    PlayerID = "abc"
                }).Returns(new List<string> { });
            }
        }
    }
}