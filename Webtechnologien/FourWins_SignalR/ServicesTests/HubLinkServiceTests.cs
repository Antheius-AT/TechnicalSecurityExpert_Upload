 //-----------------------------------------------------------------------
// <copyright file="HubLinkServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>   </author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using NUnit.Framework;
    using SharedData.SharedHubData.ConcreteTypes;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.ConcreteServices;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;
    using SignalRServices.ServiceData;
    using System.Collections;

    public class HubLinkServiceTests
    {
        private DefaultHubLinkService service;

        [SetUp]
        public void Setup()
        {
            IObjectMapService<string, GameData> mapService = new GenericMappingService<string, GameData>();
            this.service = new DefaultHubLinkService(mapService);
        }

        [Test, TestCaseSource(typeof(HubLinkTestData), "CreateGameData")]
        public bool Creates_New_Game_Returns_If_Game_Is_Stored_In_MapService(int turnTime)
        {
            var createdGameData = service.CreateNewGameAsync(turnTime);
            if (service.GameDataMap.DoesEntryExistAsync(createdGameData.Result.GameID).Result)
            {
                var gameData = service.GameDataMap.GetValueAsync(createdGameData.Result.GameID).Result;
                if (gameData.TurnTime == turnTime)
                    return true;
            }

            return false;  
        }

        [Test, TestCaseSource(typeof(HubLinkTestData), "CreateGameData")]
        public bool Creates_New_Game_And_Removes_It_From_The_GameDataMap_Returns_If_Game_Has_Been_Removed_Successfully(int turnTime)
        {
            var createdGameData = service.CreateNewGameAsync(turnTime).GetAwaiter().GetResult();

            bool eventRaised = false;
            service.ObjectRemoved += (sender, e) => { eventRaised = true; };

            service.RemoveGameAsync(createdGameData.GameID).GetAwaiter().GetResult();

            if (!service.GameDataMap.DoesEntryExistAsync(createdGameData.GameID).Result)
            {
                return eventRaised;
            }

            return false;
        }
    }

    public static class HubLinkTestData
    {
        /// <summary>
        /// Parameters: (int turnTime)
        /// </summary>
        public static IEnumerable CreateGameData
        {
            get
            {
                yield return new TestCaseData(5).Returns(true);
            }
        }
    }
}
