//-----------------------------------------------------------------------
// <copyright file="TimerElapsedEventArgs.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.EventArgs
{
    using System;
    using Interfaces;

    /// <summary>
    /// Event arguments for the <see cref="IBackgroundTimerService.TimerElapsed"/> event. 
    /// </summary>
    public class TimerElapsedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerElapsedEventArgs"/> class.
        /// </summary>
        public TimerElapsedEventArgs(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key must not be null.");

            this.Key = key;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key
        {
            get;
            private set;
        }
    }
}
