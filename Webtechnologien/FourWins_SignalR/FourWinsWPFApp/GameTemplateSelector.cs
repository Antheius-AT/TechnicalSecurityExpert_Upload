//-----------------------------------------------------------------------
// <copyright file="GameCell.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer.</author>
//-----------------------------------------------------------------------
using FourWinsWPFApp.VMs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FourWinsWPFApp
{
    /// <summary>
    /// A data template Selector for differentiating between the user being a player or a spectator
    /// </summary>
    public class GameTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Selects the Template for the specified item.
        /// </summary>
        /// <param name="item">The item that shall be shown, in this case a GameVM.</param>
        /// <param name="container">The DependencyObject which needs the template to show the object, in this case a FrameworkElement.</param>
        /// <returns>The correct DataTemplate.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            GameVM game = item as GameVM;

            if (game.IsPlayer)
            {
                return element.FindResource("PlayerGameTemplate") as DataTemplate;
            }
            else
            {
                return element.FindResource("SpectatorGameTemplate") as DataTemplate;
            }
        }
    }
}
