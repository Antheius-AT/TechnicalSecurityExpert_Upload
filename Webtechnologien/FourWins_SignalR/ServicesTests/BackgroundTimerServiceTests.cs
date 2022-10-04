//-----------------------------------------------------------------------
// <copyright file="BackgroundTimerServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>  </author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using NUnit.Framework;
    using SignalRServices.ConcreteServices;
    using SignalRServices.EventArgs;
    using System;
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundTimerServiceTests
    {
        private BackgroundTimerService service;

        [SetUp]
        public void Setup()
        {
            this.service = new BackgroundTimerService();
        }

        [Test]
        public void Does_StartTimerAsyncGeneric_Automatically_Start_Task_In_Background_And_Properly_Executes_Callback_When_Timer_HasElapsed()
        {
            var isElapsed = false;
            var cts = new CancellationTokenSource();

            Func<string, Task> callBack = p =>
            {
                if (p == "Cancel")
                    isElapsed = true;
                else
                    isElapsed = false;

                return Task.CompletedTask;
            };

            this.service.StartTimerAsync<string>(5, cts.Token, callBack, "Cancel");

            var initialTime = DateTime.Now;

            do
            {
                var currentTime = DateTime.Now;

                if ((currentTime - initialTime).TotalSeconds > 6)
                    Assert.Fail();
            } 
            while (!isElapsed);

            Assert.Pass();
        }

        [Test]
        public void Does_StartTimerAsync_Automatically_Start_Task_In_Background_And_Raises_Event_When_Timer_HasElapsed()
        {
            var isElapsed = false;
            var cts = new CancellationTokenSource();

            void EventHandler(object source, TimerElapsedEventArgs e)
            {
                if (e.Key == "Elapsed")
                    isElapsed = true;
            }

            this.service.TimerElapsed += EventHandler;
            this.service.StartTimerAsync(5, "Elapsed", cts.Token);

            var initialTime = DateTime.Now;

            do
            {
                var currentTime = DateTime.Now;

                if ((currentTime - initialTime).TotalSeconds > 6)
                    Assert.Fail();
            }
            while (!isElapsed);

            Assert.Pass();
        }

        [Test]
        public void Does_StartTimerAsyncGeneric_StopExecution_And_Not_Execute_Callback_If_Token_Is_Canceled()
        {
            var success = true;

            Func<string, Task> callBack = p =>
            {
                success = false;

                return Task.CompletedTask;
            };

            var cts = new CancellationTokenSource();

            this.service.StartTimerAsync<string>(5, cts.Token, callBack, "whatever");

            var initialTime = DateTime.Now;

            do
            {
                var currentTime = DateTime.Now;

                if ((currentTime - initialTime).TotalSeconds > 3)
                    cts.Cancel();
            } 
            while (!cts.Token.IsCancellationRequested);

            // Waiting to ensure the task is truly canceled. 
            // If it wasn`t, during the 5 second wait the task would complete and set the 
            // boolean to false, failing the test.
            Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            Assert.True(success);
        }

        [Test]
        public void Does_StartTimerAsync_StopExecution_And_Not_Raise_Event_If_Token_Is_Canceled()
        {
            var success = true;

            var cts = new CancellationTokenSource();

            void EventHandler(object source, TimerElapsedEventArgs e)
            {
                success = false;
            }

            service.TimerElapsed += EventHandler;
            this.service.StartTimerAsync(5, "whatever", cts.Token);

            var initialTime = DateTime.Now;

            do
            {
                var currentTime = DateTime.Now;

                if ((currentTime - initialTime).TotalSeconds > 3)
                    cts.Cancel();
            }
            while (!cts.Token.IsCancellationRequested);

            // Waiting to ensure the task is truly canceled. 
            // If it wasn`t, during the 3 second wait the task would complete and set the 
            // boolean to false, failing the test.
            Task.Delay(TimeSpan.FromSeconds(3)).GetAwaiter().GetResult();

            Assert.True(success);
        }

        [Test]
        public void Does_StartTimerAsyncGeneric_Call_Callback_If_Timer_Is_Canceled()
        {
            var cancellationCallbackCalled = false;
            var timerElapsedCallbackCalled = false;

            Func<string, Task> timerElapsedCallback = p =>
            {
                timerElapsedCallbackCalled = true;

                return Task.CompletedTask;
            };

            var cancellationCallback = new Action(() =>
            {
                cancellationCallbackCalled = true;
            });

            var cts = new CancellationTokenSource();
            cts.Token.Register(cancellationCallback);

            this.service.StartTimerAsync<string>(1, cts.Token, timerElapsedCallback, "Whatever");

            cts.Cancel();

            // Again, wait 2 seconds to ensure the task is truly canceled and not calling the
            // timer elapsed callback. If it were called, the respective boolean would be true
            // and failing the test.
            Task.Delay(2).GetAwaiter().GetResult();

            Assert.That(cancellationCallbackCalled && !timerElapsedCallbackCalled);
        }

        [Test]
        public void Does_StartTimerAsync_Call_Callback_If_Timer_Is_Canceled()
        {
            var cancellationCallbackCalled = false;
            var eventRaised = false;

            void EventHandler(object source, TimerElapsedEventArgs e)
            {
                eventRaised = true;
            }

            var cancellationCallback = new Action(() =>
            {
                cancellationCallbackCalled = true;
            });

            var cts = new CancellationTokenSource();
            cts.Token.Register(cancellationCallback);

            this.service.TimerElapsed += EventHandler;
            this.service.StartTimerAsync(1, "Whatever", cts.Token);

            cts.Cancel();

            // Again, wait 2 seconds to ensure the task is truly canceled and not calling the
            // timer elapsed callback. If it were called, the respective boolean would be true
            // and failing the test.
            Task.Delay(2).GetAwaiter().GetResult();

            Assert.That(cancellationCallbackCalled && !eventRaised);
        }
    }
}
