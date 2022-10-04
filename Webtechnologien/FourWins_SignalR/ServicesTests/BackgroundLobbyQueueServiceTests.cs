//-----------------------------------------------------------------------
// <copyright file="GenericMappingServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>  </author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using NUnit.Framework;
    using SignalRServices.ConcreteServices;
    using System.Collections;

    public class BackgroundLobbyQueueServiceTests
    {
        private BackgroundLobbyQueueService service;

        [SetUp]
        public void Setup()
        {
            this.service = new BackgroundLobbyQueueService();
        }

        [Test, TestCaseSource(typeof(BackgroundLobbyQueueServiceTestData), "GetEnqueueEntryData")]
        public void Enqueue()
        {
            // this.service.EnqueueAsync();
        }
    }

    public static class BackgroundLobbyQueueServiceTestData
    {
        /// <summary>
        ///
        /// </summary>
        public static IEnumerable GetEnqueueEntryData
        {
            get
            {
                yield return new TestCaseData().Returns(1);
            }
        }
    }
}
