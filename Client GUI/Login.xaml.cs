// Filename: Login.xaml.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Login screen for user to log into the system
// Author:   George Aziz (19765453)
// Date:     17/04/2021

using AuthLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using System.Windows.Shapes;

namespace Client_GUI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private MainWindow initialWindow;
        private AuthInterface authConnection;
        private delegate int PerformLogin(string name, string password);

        /// <summary>
        /// Constructor of the Login Window
        /// </summary>
        /// <param name="initWindow">MainWindow object to allow the user to go back to it on the GoBack button click</param>
        /// <param name="authConnect">Authentication Interface Object to allow communication with the authentication server</param>
        public Login(MainWindow initWindow, AuthInterface authConnect)
        {
            InitializeComponent();
            initialWindow = initWindow;
            authConnection = authConnect;
        }

        /// <summary>
        /// OnClick for the GoBack Button to allow user launch the initial/main window again
        /// </summary>
        public void GoBack_Btn(object sender, RoutedEventArgs e)
        {
            initialWindow.Show();
            this.Hide();
        }

        /// <summary>
        /// OnClick for the Login Button to allow user to log into the system
        /// </summary>
        public void Login_Btn(object sender, RoutedEventArgs e)
        {
            AsyncCallback callBackClick;
            callBackClick = this.LoginCompletion;
            PerformLogin perform = new PerformLogin(this.DoLogin);
            perform.BeginInvoke(NameBox.Text, PassBox.Password, callBackClick, null);
        }

        /// <summary>
        /// Runs Login Service From Authenticator Server on a new thread
        /// </summary>
        /// <param name="name">String of username inputted</param>
        /// <param name="pass">String of password inputted</param>
        /// <returns>result string</returns>
        private int DoLogin(string name, string pass)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
                ProgBar.IsIndeterminate = true;
            });

            return authConnection.Login(name, pass);

        }

        /// <summary>
        /// On completion of the new Login thread, it will process the returned data and open new window if login was succesful
        /// </summary>
        /// <param name="asyncResult">IAsyncResult object that contains the result of the service</param>
        private void LoginCompletion(IAsyncResult asyncResult)
        {
            PerformLogin loginService;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            int token;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    loginService = (PerformLogin)asyncObj.AsyncDelegate;
                    token = loginService.EndInvoke(asyncObj);
                    if (token == -1) //Invalid Credentials has been inputted
                    {
                        MessageBox.Show("Invalid Credentials - Please Try Again", "Login Error");
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ServicesTest serviceTest = new ServicesTest(initialWindow, token);
                            serviceTest.Show(); //Displays the Server Tester Screen 
                            this.Hide(); //Closes this Login Screen as there is no need for it to be using resources
                        });
                    }
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
                {
                    //Creates a new connection for the next time authServer is being called to retry
                    NetTcpBinding tcp = new NetTcpBinding();
                    string authURL = "net.tcp://localhost/AuthService";
                    ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
                    authConnection = authFactory.CreateChannel();
                    MessageBox.Show("Authentication Server is Down - Please Try Again Later", "Service Down");
                }

                //GUI Elements Change
                this.Dispatcher.Invoke(() =>
                {
                    UnlockElements();
                    ProgBar.IsIndeterminate = false;
                });
            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }

        /// <summary>
        /// Lock the UI so that the user can't mess anything while registration service is being executed
        /// </summary>
        private void LockElements()
        {
            NameBox.IsEnabled = false;
            PassBox.IsEnabled = false;
            BackBtn.IsEnabled = false;
            LoginBtn.IsEnabled = false;
        }

        /// <summary>
        /// Unlock the UI so that the user can interact with the interface again after registration has been executed
        /// </summary>
        private void UnlockElements()
        {
            NameBox.IsEnabled = true;
            PassBox.IsEnabled = true;
            BackBtn.IsEnabled = true;
            LoginBtn.IsEnabled = true;
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
