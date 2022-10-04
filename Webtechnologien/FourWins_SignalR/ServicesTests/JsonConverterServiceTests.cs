//-----------------------------------------------------------------------
// <copyright file="GameServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>    </author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using FourWins_SignalRServer.HubData;
    using NUnit.Framework;
    using SharedData.SharedHubData.ConcreteTypes;
    using SignalRServices.ServiceData;
    using System;
    using System.Collections;

    public class JsonConverterServiceTests
    {
        private readonly JsonConverterService service = new JsonConverterService();

        [Test, TestCaseSource(typeof(JsonConverterServiceTestData), "GetConvertData")]
        public string Converts_Object_Into_Json_Returns_JsonString(object data)
        {
            try
            {
                return service.ConvertAsync(data).Result;
            }
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
            
        }

        [Test, TestCaseSource(typeof(JsonConverterServiceTestData), "GetConvertBackData")]
        public bool Converts_Object_Into_Json_And_Converts_It_Back_Returns_If_The_Same(ClientToGameHubParameters data)
        {
            string jsonString = service.ConvertAsync(data).Result;
            var obj = service.ConvertBackAsync<ClientToGameHubParameters>(jsonString).Result;

            if (obj.Column == data.Column &&
                obj.GameID == data.GameID &&
                obj.PlayerID == data.PlayerID &&
                obj.PlayerName == data.PlayerName)
                return true;

            return false;
        }
    }

    public static class JsonConverterServiceTestData
    {
        /// <summary>
        /// Params: (object: to serialize)
        /// </summary>
        public static IEnumerable GetConvertData
        {
            get
            {
                yield return new TestCaseData(new GameHubToClientParameters()
                {
                    GameID = "abc",
                    PlayerName = "Christian",
                    Column = 0,
                    IsPlayer = true,
                    IsCurrentPlayer = false
                }).Returns(
                       "{" +
                       "\"$type\":\"FourWins_SignalRServer.HubData.GameHubToClientParameters, SharedData\","+
                       "\"GameID\":\"abc\"," +
                       "\"PlayerName\":\"Christian\"," +
                       "\"Column\":0," +
                       "\"IsPlayer\":true," +
                       "\"IsCurrentPlayer\":false" +
                     "}");


                yield return new TestCaseData(new ClientToGameHubParameters()
                {
                    GameID = "abc",
                    PlayerID = "123",
                    PlayerName = "Christian",
                    Column = 0,
                }).Returns(
                       "{" +
                       "\"$type\":\"FourWins_SignalRServer.HubData.ClientToGameHubParameters, SharedData\"," +
                       "\"GameID\":\"abc\"," +
                       "\"PlayerID\":\"123\"," +
                       "\"PlayerName\":\"Christian\"," +
                       "\"Column\":0" +
                     "}");

                yield return new TestCaseData(new object()).Returns("{\"$type\":\"System.Object, System.Private.CoreLib\"}");

                yield return new TestCaseData(null).Returns(string.Empty);
            }
        }

        /// <summary>
        /// Params: (ClientToGameHubParameters: to serialize and deserialize)
        /// </summary>
        public static IEnumerable GetConvertBackData
        {
            get
            {
                yield return new TestCaseData(new ClientToGameHubParameters()
                {
                    GameID = "abc",
                    PlayerID = "123",
                    PlayerName = "Christian",
                    Column = 0,
                }).Returns(true);

                yield return new TestCaseData(new ClientToGameHubParameters()).Returns(true);
            }
        }
    }
}
