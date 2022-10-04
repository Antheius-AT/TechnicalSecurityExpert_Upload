//-----------------------------------------------------------------------
// <copyright file="ISignalRClient.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.ClientContract
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.Interfaces.DataInterfaces;

    /// <summary>
    /// This interface defines a game client which implements the <see cref="IClientProxy"/> interface
    /// and can be used in the server side hubs as a client object.
    /// </summary>
    public interface ISignalRClient : IClientContract, IComparable<ISignalRClient>
    {
        /// <summary>
        /// Gets the client proxy through which to send messages.
        /// </summary>
        IClientProxy ClientProxy { get; }

        /// <summary>
        /// Gets the connection ID of the client.
        /// </summary>
        string ConnectionID { get; }
    }
}
