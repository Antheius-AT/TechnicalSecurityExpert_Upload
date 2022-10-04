//-----------------------------------------------------------------------
// <copyright file="BackgroundLobbyQueueService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ConcreteServices
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SignalRServices.Interfaces.DataInterfaces;
    using SignalRServices.Interfaces.ServiceInterfaces.LobbyServices;
    using SignalRServices.QueueItems;

    /// <summary>
    /// This class holds a queue of changes regarding the lobby.
    /// </summary>
    public class BackgroundLobbyQueueService : IBackgroundQueueService
    {
        private Func<IEnumerable<IQueueItem>, Task> queueInvocationCallback;
        private double delay;
        private CancellationToken queueCancellationToken;
        private readonly ConcurrentQueue<IQueueItem> queue;
        private object queueLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundLobbyQueueService"/> class.
        /// </summary>
        public BackgroundLobbyQueueService()
        {
            this.queue = new ConcurrentQueue<IQueueItem>();
            this.Delay = 0.5;
            this.queueLock = new object();
        }

        /// <summary>
        /// Gets or sets a callback that is invoked whenever the specified <see cref="Delay"/> has passed
        /// and the queue items need to be handled.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public Func<IEnumerable<IQueueItem>, Task> QueueInvocationCallback
        {
            get
            {
                return this.queueInvocationCallback;
            }

            set
            {
                this.queueInvocationCallback = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds that the executing thread
        /// waits before handling the queue items.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if you attempt to set a negative value.
        /// </exception>
        public double Delay
        {
            get
            {
                return this.delay;
            }

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must not be negative.");

                this.delay = value;
            }
        }


        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public CancellationToken QueueCancellationToken
        {
            get
            {
                return this.queueCancellationToken;
            }

            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Cancellation token must not be null.");

                this.queueCancellationToken = value;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the queue is started.
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// Enqueues a new todo object.
        /// </summary>
        /// <param name="todoObject">The todo object to enqueue.</param>
        /// <returns>A task handling the process of enqueueing the object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if todo object is null.
        /// </exception>
        public Task EnqueueAsync(IQueueItem todoObject)
        {
            if (todoObject == null)
                throw new ArgumentNullException(nameof(todoObject), "Todo object must not be null.");
            
            this.queue.Enqueue(todoObject);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts the thread monitoring the queue and pushing updates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel monitoring of the queue.</param>
        /// <returns>A task working through the queue and pushing updates every few seconds.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if token is null.
        /// </exception>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken), "Cancellation token must not be null.");

            if (this.IsRunning)
                throw new InvalidOperationException("Could not start queue because queue was already started.");

            this.IsRunning = true;
            this.QueueCancellationToken = cancellationToken;

            return this.WorkThroughQueue();
        }

        /// <summary>
        /// Empties the queue every few seconds, handling all of the contained todo items.
        /// </summary>
        /// <returns>A task object performing the logic.</returns>
        protected virtual async Task WorkThroughQueue()
        {
            while (!this.QueueCancellationToken.IsCancellationRequested)
            {
                if (this.queue.Count > 0)
                {
                    var items = new List<IQueueItem>();

                    // Muss hier noch schauen wie ich es am besten mache, dass alle items gemeinsam gesendet werden, sodass pro Queue
                    // iteration nur eine Nachricht pro Client rausgeht.
                    // Idee: Von der queue nicht direkt items schicken, sondern per Event/sonstigem Konstrukt in die Hub übergehen
                    // und von dort eine Composite response aus den queue items parsen die zusammengehören und schicken.
                    for (int i = 0; i < this.queue.Count; i++)
                    {
                        this.queue.TryDequeue(out IQueueItem result);
                        items.Add(result);
                    }

                    await this.queueInvocationCallback.Invoke(items);
                }

                await Task.Delay(TimeSpan.FromSeconds(this.Delay));
            }

            this.IsRunning = false;
        }
    }
}
