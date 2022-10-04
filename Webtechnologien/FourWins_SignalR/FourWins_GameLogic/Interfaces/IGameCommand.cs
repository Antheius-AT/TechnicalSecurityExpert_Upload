//-----------------------------------------------------------------------
// <copyright file="IGameCommand.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.Interfaces
{
    /// <summary>
    /// The interface to use for the game to execute game board changes.
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// Executes the game board changes.
        /// </summary>
        public void Execute();
    }
}
