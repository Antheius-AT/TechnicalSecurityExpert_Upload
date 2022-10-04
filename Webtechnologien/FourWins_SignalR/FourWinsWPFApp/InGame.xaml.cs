//-----------------------------------------------------------------------
// <copyright file="GameCell.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer.</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp
{
    using System.Windows;
    using System.Windows.Controls;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Vms;
    using FourWinsWPFApp.VMs;

    /// <summary>
    /// Interaktionslogik für InGame.xaml
    /// </summary>
    public partial class InGame : Window
    {
        /// <summary>
        /// The view model that is the current data context.
        /// </summary>
        private ActiveGamesVM vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="InGame"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public InGame(IActiveGamesVM viewModel)
        {
            InitializeComponent();
            this.vm = viewModel as ActiveGamesVM;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Reacts when the left mouse key was pressed while hovering over a specific grid.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private async void Grid_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            GameVM game = TabControlGames.SelectedItem as GameVM;
            Grid senderGrid = sender as Grid;
            int columnIndex=-1;

            switch (senderGrid.Name)
            {
                case "firstColumn":
                    columnIndex = 0;
                    break;
                case "secondColumn":
                    columnIndex = 1;
                    break;
                case "thirdColumn":
                    columnIndex = 2;
                    break;
                case "fourthColumn":
                    columnIndex = 3;
                    break;
                case "fifthColumn":
                    columnIndex = 4;
                    break;
                case "sixthColumn":
                    columnIndex = 5;
                    break;
                case "seventhColumn":
                    columnIndex = 6;
                    break;
            }

            if (columnIndex == -1)
            {
                return;
            }

          await vm.SendMoveToServer(game.GameID,game.PlayerID,columnIndex);
        }

        /// <summary>
        /// Reacts on a tab control item being selected.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void TabControlGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
                //    GameVM game = TabControlGames.SelectedItem as GameVM;
                //    game.HeaderColor = ActiveGameVMColor.StandardColor_TabHeader;
            //});
        }
    }
}
