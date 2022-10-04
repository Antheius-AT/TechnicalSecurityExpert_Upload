//-----------------------------------------------------------------------
// <copyright file="IResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.Interfaces
{
    /// <summary>
    /// Represents a response object containing at the very least a simple response message.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets the response message.
        /// </summary>
        string Message { get; }
    }
}
