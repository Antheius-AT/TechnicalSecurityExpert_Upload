//-----------------------------------------------------------------------
// <copyright file="IBackgroundQueueService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.Interfaces.ServiceInterfaces.LobbyServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SignalRServices.Interfaces.DataInterfaces;

    /// <summary>
    /// Represents a service that runs in the background and holds a queue of todo objects that is worked through by a 
    /// separate thread.
    /// </summary>
    public interface IBackgroundQueueService
    {
        /// <summary>
        /// gets a value indicating whether the queue is started.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Gets or sets the amount of time in seconds that the executing thread
        /// waits before handling the next items in the queue.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if you attempt to set a negative value.
        /// </exception>
        public double Delay { get; set; }

        /// <summary>
        /// Gets or sets a callback that is invoked whenever the specified <see cref="Delay"/> has passed
        /// and the queue items need to be handled.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public Func<IEnumerable<IQueueItem>, Task> QueueInvocationCallback { get; set; }

        /// <summary>
        /// Enqueues a new todo object.
        /// </summary>
        /// <param name="client">The client associated with this todo object.</param>
        /// <returns>A task handling the enqueueing process.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if client is null.
        /// </exception>
        public Task EnqueueAsync(IQueueItem client);

        /// <summary>
        /// Starts the thread monitoring the queue and pushing updates every few seconds.
        /// </summary>
        /// <param name="cancellationToken">The token to cancel the thread monitoring the queue.</param>
        /// <returns>A task handling the logic of starting the queue.</returns>
        /// <exception cref="InvalidOperationException">
        /// Is thrown if you call this method while the queue is already running. 
        /// You can check whether the queue is running via the <see cref="IsRunning"/> Property.
        /// </exception>
        public Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the cancellation token which can be used to cancel the queue monitoring thread.
        /// </summary>
        public CancellationToken QueueCancellationToken { get; }
    }
}
