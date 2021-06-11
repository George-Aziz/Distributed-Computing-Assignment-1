// Filename: MainWindow.xaml.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Initial screen that the user sees when they launch the GUI which allows them to register or login
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using AuthLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AuthInterface authConnection;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// OnClick for the Register Button to allow user launch the Register Window
        /// </summary>
        public void Register_Btn(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //Creates Authenticator Connection here since if server was down and is now up it will create new server for Registration
            NetTcpBinding tcp = new NetTcpBinding();
            string authURL = "net.tcp://localhost/AuthService";
            ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
            authConnection = authFactory.CreateChannel();
            Registration regWindow = new Registration(this, authConnection);
            regWindow.Show();
        }

        /// <summary>
        /// OnClick for the Login Button to allow user launch the Login Window
        /// </summary>
        public void Login_Btn(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //Creates Authenticator Connection here since if server was down and is now up it will create new server for Login
            NetTcpBinding tcp = new NetTcpBinding();
            string authURL = "net.tcp://localhost/AuthService";
            ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
            authConnection = authFactory.CreateChannel();
            Login loginWindow = new Login(this, authConnection);
            loginWindow.Show();
        }

        /// <summary>
        /// Closes all windows when X on top right corner is clicked
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
