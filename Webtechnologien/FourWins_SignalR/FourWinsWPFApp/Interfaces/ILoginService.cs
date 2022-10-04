//-----------------------------------------------------------------------
// <copyright file="ILoginService.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Interfaces
{
    using FourWinsWPFApp.Models;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represent a service capable of handling logins.
    /// </summary>
    public interface IClientFactoryService<T> where T : class, IClientContract
    {
        /// <summary>
        /// Tries to log in to a SignalR Server using the specified connection information.
        /// </summary>
        /// <param name="connectionInfo">A class containing connection information.</param>
        /// <param name="URLpostfix">The relative path to the remote endpoint located under the same top level domain.</param>
        /// <returns>A task containing whether the login was successful once it terminates.</returns>
        T CreateClient(ConnectionInfo connectionInfo, string URLpostfix);
   
    }
}
