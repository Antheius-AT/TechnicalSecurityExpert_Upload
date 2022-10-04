//-----------------------------------------------------------------------
// <copyright file="Login.xaml.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp
{
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Interfaces;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für Login_.xaml
    /// </summary>
    public partial class Login : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InGame"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public Login(ILoginVM<ErrorOccurredEventArgs> loginVM)
        {
            InitializeComponent();

            this.DataContext = loginVM;
            loginVM.ClosingRequested += HandleClosingRequested;
            loginVM.ErrorOccurred += HandleErrorOccurred;
        }
        /// <summary>
        /// Handles the event in which some error occurred during connection.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            MessageBox.Show(e.ErrorMessage, "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Handles the event in which closing of the view is requested.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleClosingRequested(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
