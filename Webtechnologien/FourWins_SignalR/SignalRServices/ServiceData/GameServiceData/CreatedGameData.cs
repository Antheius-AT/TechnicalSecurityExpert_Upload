//-----------------------------------------------------------------------
// <copyright file="CreatedGameData.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ServiceData
{
    /// <summary>
    /// This class stores information about a newly created game.
    /// </summary>
    public class CreatedGameData
    {
        // Hier kommen deine Properties hinein die du brauchst. Und alles gleich dokumentieren. 
        // Ich kenn mich nicht aus was du haben willst sonst würd ichs gleich selbst machen.
        // Und auf exceptions im Constructor prüfen!! 
        // UND Exceptions dokumentieren und zwar alle!
        
        public CreatedGameData(string gameID, string playerID)
        {
            this.GameID = gameID;
            this.PlayerID = playerID;;
        }

        /// <summary>
        /// The ID of the game.
        /// </summary>
        public string GameID
        {
            get;
        }

        /// <summary>
        /// The ID to verify as a player.
        /// </summary>
        public string PlayerID
        {
            get;
        }
    }
}
