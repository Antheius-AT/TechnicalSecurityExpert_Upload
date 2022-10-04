//-----------------------------------------------------------------------
// <copyright file="ChallengeablePlayerViewModel.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using FourWinsWPFApp.EventArguments;

    /// <summary>
    /// This class represents a visualization of other players that the client can challenge.
    /// </summary>
    /// TODO: Make this a view model.
    public class ChallengeablePlayerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeablePlayer"/> class.
        /// </summary>
        /// <param name="username">The players username.</param>
        public ChallengeablePlayerViewModel(ChallengeablePlayer player)
        {
            this.Player = player ?? throw new ArgumentNullException(nameof(player), "Player must not be null.");
        }

        /// <summary>
        /// Notifies subscribers that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies subscribers that a challenge has been accepted.
        /// </summary>
        public event EventHandler<ChallengeAcceptedEventArgs> ChallengeAccepted;

        /// <summary>
        /// Gets the player encapsulated by this view model.
        /// </summary>
        public ChallengeablePlayer Player
        {
            get;
        }

        /// <summary>
        /// Gets the command that allows for accepting an incoming challenge.
        /// </summary>
        public ICommand AcceptChallengeCommand
        {
            get
            {
                return new RelayCommand(challenge =>
                {
                    this.RaiseChallengeAccepted();
                },
                challenge =>
                {
                    var challengedPlayerVM = challenge as ChallengeablePlayerViewModel;

                    return challengedPlayerVM != null && challengedPlayerVM.Player.ChallengeStatus == ChallengeStatus.ChallengeIncoming;
                });
            }
        }

        /// <summary>
        /// Modifies a players challenge status and raises an event notifying any view subscribed 
        /// to this property of the change.
        /// </summary>
        /// <param name="challengeStatus">The modified challenge status.</param>
        public void ModifyPlayerChallengeStatus(ChallengeStatus challengeStatus)
        {
            // TODO: Think about whether to include a custom built event to notify this view model to fire the propertychagned event.
            this.Player.ChallengeStatus = challengeStatus;

            this.RaisePropertyChanged(nameof(this.Player));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyPath">The changed property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyPath = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyPath));
        }

        /// <summary>
        /// Raises the <see cref="ChallengeAccepted"/> event.
        /// </summary>
        protected virtual void RaiseChallengeAccepted()
        {
            this.ChallengeAccepted?.Invoke(this, new ChallengeAcceptedEventArgs(this));
        }
    }
}
