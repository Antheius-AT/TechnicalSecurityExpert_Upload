//-----------------------------------------------------------------------
// <copyright file="BackgroundTimerService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ConcreteServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SignalRServices.EventArgs;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;

    /// <summary>
    /// Represent a timer service that runs in the background and, depending
    /// on the start method called, raises an event or invokes a delegate on completion.
    /// </summary>
    public class BackgroundTimerService : IBackgroundTimerService
    {
        /// <summary>
        /// This event is raised when the timer elapsed.
        /// </summary>
        public event EventHandler<TimerElapsedEventArgs> TimerElapsed;

        /// <summary>
        /// Starts the timer to run asynchronously in the background.
        /// </summary>
        /// <param name="seconds">The seconds the timer should run.</param>
        /// <param name="cancellationToken">The token to cancel the timer prematurely.</param>
        /// <param name="callback">A callback to invoke after the timer terminated.</param>
        /// <returns>A task object handling the running of the timer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if seconds is negative.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key or cancellation token are null.
        /// </exception>
        public Task StartTimerAsync<T>(int seconds, CancellationToken cancellationToken, Func<T, Task> callback, T callbackParameter)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must not be negative.");

            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Callback must not be null.");

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken), "Cancellation token must not be null.");

            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    await callback.Invoke(callbackParameter);
            });
        }

        /// <summary>
        /// Starts the timer to run asynchronously in the background.
        /// </summary>
        /// <param name="seconds">The seconds the timer should run.</param>
        /// <param name="key">The key used in the callback to identify the necessary action that needs to be taken.</param>
        /// <param name="cancellationToken">The token to cancel the timer prematurely.</param>
        /// <param name="restart">A value indicating whether to restart the timer automatically after termination.</param>
        /// <returns>A task object handling the running of the timer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if seconds is negative.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key or cancellation token are null.
        /// </exception>
        public Task StartTimerAsync(int seconds, string key, CancellationToken cancellationToken)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must not be negative.");

            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key must not be null.");

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken), "Cancellation token must not be null.");
            
            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                    this.RaiseTimerElapsed(key);
            },
            cancellationToken);
        }

        /// <summary>
        /// Raises the <see cref="TimerElapsed"/> event.
        /// </summary>
        /// <param name="key">The key used in the callback to identify the necessary action.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key is null.
        /// </exception>
        protected virtual void RaiseTimerElapsed(string key)
        {
            this.TimerElapsed?.Invoke(this, new TimerElapsedEventArgs(key));
        }
    }
}
