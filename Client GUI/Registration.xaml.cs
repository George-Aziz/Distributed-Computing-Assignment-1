// Filename: Registration.xaml.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Registration screen for user to register into the system
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private MainWindow initialWindow;
        private AuthInterface authConnection;
        private delegate string PerformRegistration(string name, string password);

        /// <summary>
        /// Constructor of the Registration Window
        /// </summary>
        /// <param name="initWindow">MainWindow object to allow the user to go back to it on the GoBack button click</param>
        /// <param name="authConnect">Authentication Interface Object to allow communication with the authentication server</param>
        public Registration(MainWindow initWindow, AuthInterface authConnect)
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
        /// OnClick for the Register Button to allow user to register into the system
        /// </summary>
        public void Register_Btn(object sender, RoutedEventArgs e)
        {
            AsyncCallback callBackClick;
            if (ValidateString(NameBox.Text) && ValidateString(PassBox.Password))
            {

                callBackClick = this.RegistrationCompletion;
                PerformRegistration perform = new PerformRegistration(this.DoRegistration);
                perform.BeginInvoke(NameBox.Text, PassBox.Password, callBackClick, null);
            }
            else
            {
                MessageBox.Show("Please ensure no input is empty & no whitespaces", "Registration Error");
            }
        }

        /// <summary>
        /// Runs Register Service From Authenticator Server on a new thread
        /// </summary>
        /// <param name="name">String of username inputted</param>
        /// <param name="pass">String of password inputted</param>
        /// <returns>result string</returns>
        private string DoRegistration(string name, string pass)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
                ProgBar.IsIndeterminate = true;
            });

            return authConnection.Register(name, pass);
        }

        /// <summary>
        /// On completion of the new Registration thread, it will process the returned data and notify whether registration was succesful or not
        /// </summary>
        /// <param name="asyncResult">IAsyncResult object that contains the result of the service</param>
        private void RegistrationCompletion(IAsyncResult asyncResult)
        {
            PerformRegistration registerService;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            string result;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    registerService = (PerformRegistration)asyncObj.AsyncDelegate;
                    result = registerService.EndInvoke(asyncObj);
                    if (result.Contains("Unsuccesful")) //Invalid Credentials has been inputted
                    {
                        MessageBox.Show(result, "Registration Error");
                    }
                    else
                    {
                        MessageBox.Show("Succesful Registration", "Registration");
                        this.Dispatcher.Invoke(() =>
                        {
                            initialWindow.Show();
                            this.Hide();
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
        /// String Validation
        /// </summary>
        /// <param name="valid">Validation boolean</param>
        private bool ValidateString(string input)
        {
            bool valid = false;
            //Username and passwords cannot contain whitespaces
            if(!(String.IsNullOrWhiteSpace(input) || input.Contains(" ")))
            {
                valid = true;
            }

            return valid;
        }

        /// <summary>
        /// Lock the UI so that the user can't mess anything while registration service is being executed
        /// </summary>
        private void LockElements()
        {
            NameBox.IsEnabled = false;
            PassBox.IsEnabled = false;
            BackBtn.IsEnabled = false;
            RegisterBtn.IsEnabled = false;
        }

        /// <summary>
        /// Unlock the UI so that the user can interact with the interface again after registration has been executed
        /// </summary>
        private void UnlockElements()
        {
            NameBox.IsEnabled = true;
            PassBox.IsEnabled = true;
            BackBtn.IsEnabled = true;
            RegisterBtn.IsEnabled = true;
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
