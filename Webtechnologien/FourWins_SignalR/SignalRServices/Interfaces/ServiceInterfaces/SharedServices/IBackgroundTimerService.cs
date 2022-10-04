//-----------------------------------------------------------------------
// <copyright file="IBackgroundTimer.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.Interfaces.ServiceInterfaces.SharedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EventArgs;

    /// <summary>
    /// Defines an object which represents a timer service that runs in the background, and, depending
    /// on the start method called, raises an event or invokes a delegate on completion.
    /// </summary>
    public interface IBackgroundTimerService
    {
        /// <summary>
        /// This event is raised when the timer elapsed.
        /// </summary>
        public event EventHandler<TimerElapsedEventArgs> TimerElapsed;

        /// <summary>
        /// Starts the timer to run asynchronously in the background. Invokes the callback
        /// specified after termination.
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
        Task StartTimerAsync<T>(int seconds, CancellationToken cancellationToken, Func<T, Task> callback, T callbackParameter);

        /// <summary>
        /// Starts the timer to run asynchronously in the background. Fires the <see cref="TimerElapsed"/> event
        /// on termination.
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
        Task StartTimerAsync(int seconds, string key, CancellationToken cancellationToken);
    }
}
