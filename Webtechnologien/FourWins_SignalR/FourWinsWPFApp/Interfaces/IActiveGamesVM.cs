using FourWinsWPFApp.EventArguments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace FourWinsWPFApp.Interfaces
{
    public interface IActiveGamesVM
    {
        /// <summary>
        /// A Task to add a new Game to the 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task AddGame(GameCreationRequestedEventArgs e);
        Task RequestGameWatching(WatchingGameRequestedEventArgs e);
    }
}
