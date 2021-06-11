// Filename: ServicesTest.xaml.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Service Test Screen for user to access the Registry Web Service and also test the calc services provided by the service provider
// Author:   George Aziz (19765453)
// Date:     17/04/2021

using AuthLib;
using Newtonsoft.Json;
using Registry_API_Classes;
using RestSharp;
using Service_Provider_API_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace Client_GUI
{
    /// <summary>
    /// Interaction logic for ServicesTest.xaml
    /// </summary>
    public partial class ServicesTest : Window
    {
        private MainWindow initialWindow;
        private string registryUrl;
        private RestClient registryClient;
        private int token;
        private List<TextBox> operandTextBoxes;
        private List<TextBlock> operandTextBlocks;

        //Delegates setup
        private delegate RegServicesResult PerformAllServices(RegAllServicesInput input);
        private delegate RegServicesResult PerformSearchService(RegSearchInput input);
        private delegate string PerformRunService(CalcInputData input, string servicePoint);
        private delegate void PerformTokenCheck();

        /// <summary>
        /// Constructor of the Services Test Window
        /// </summary>
        /// <param name="initWindow">MainWindow object to be shown when tokens get cleared out and need user to re-login</param>
        /// <param name="userToken">Token of logged in user</param>
        public ServicesTest(MainWindow initWindow, int userToken)
        {
            InitializeComponent();
            initialWindow = initWindow;
            registryUrl = "https://localhost:44367/api/Registry";
            registryClient = new RestClient(registryUrl);

            token = userToken;
            operandTextBoxes = new List<TextBox>();
            operandTextBlocks = new List<TextBlock>();
            TokenCheck();
        }

        /// <summary>
        /// OnClick for the AllServices Button to allow user to view all services currently in Registry
        /// </summary>
        private void AllServices_Btn(object sender, RoutedEventArgs e)
        {
            AsyncCallback callBackClick;

            RegAllServicesInput input = new RegAllServicesInput();
            input.Token = token;

            callBackClick = this.AllServicesCompletion;
            PerformAllServices perform = new PerformAllServices(this.DoAllServices);
            perform.BeginInvoke(input,callBackClick, null);
        }

        /// <summary>
        /// Runs AllService service from Registry on a new thread
        /// </summary>
        /// <param name="input">RegAllServicesInput object that contains all input details required for service to run</param>
        /// <returns>RegServiceResult object that contains all returned data from the service execution</returns>
        private RegServicesResult DoAllServices(RegAllServicesInput input)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
                ProgBar.IsIndeterminate = true;
            });
       
            IRestRequest request = new RestRequest("AllServices", Method.POST).AddJsonBody(input);
            IRestResponse response = registryClient.Execute(request);
            return JsonConvert.DeserializeObject<RegServicesResult>(response.Content);
        }

        /// <summary>
        /// On completion of the new AllService thread, it will process the returned data and display to user
        /// </summary>
        /// <param name="asyncResult">IAsyncResult object that contains the result of the service</param>
        private void AllServicesCompletion(IAsyncResult asyncResult)
        {
            PerformAllServices allServices;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            RegServicesResult result;

            if (asyncObj.EndInvokeCalled == false)
            {
                allServices = (PerformAllServices)asyncObj.AsyncDelegate;
                result = allServices.EndInvoke(asyncObj);

                if (!(result is null)) //If null then something is wrong with Registry Service
                {
                    if (result.Status.Contains("Authenticated"))
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ServiceDropMenu.Items.Clear(); //Ensures no duplicates appear in the menu
                            foreach (RegistryJson service in result.Services)
                            {
                                ServiceDropMenu.Items.Add(service);
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show("Error: Please try again", "Retrieving All services");
                    }
                }
                else
                {
                    MessageBox.Show("Service is Down - Please Try Again Later", "Service Down Error");
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
        /// OnClick for the SearchService Button to allow user to view all services that match the user search
        /// </summary>
        private void SearchService_Btn(object sender, RoutedEventArgs e)
        {
            AsyncCallback callBackClick;

            RegSearchInput input = new RegSearchInput();
            input.Token = token;
            input.Description = ServiceSearchBox.Text;

            callBackClick = this.SearchServiceCompletion;
            PerformSearchService perform = new PerformSearchService(this.DoSearchService);
            perform.BeginInvoke(input, callBackClick, null);
        }

        /// <summary>
        /// Runs AllService service from Registry on a new thread
        /// </summary>
        /// <param name="input">RegAllServicesInput object that contains all input details required for service to run</param>
        /// <returns>RegServiceResult object that contains all returned data from the service execution</returns>
        private RegServicesResult DoSearchService(RegSearchInput input)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
                ProgBar.IsIndeterminate = true;
            });


            IRestRequest request = new RestRequest("Search", Method.POST).AddJsonBody(input);
            IRestResponse response = registryClient.Execute(request);

            return JsonConvert.DeserializeObject<RegServicesResult>(response.Content);
        }

        /// <summary>
        /// On completion of the new SearchService thread, it will process the returned data and display to user
        /// </summary>
        /// <param name="asyncResult">IAsyncResult object that contains the result of the service</param>
        private void SearchServiceCompletion(IAsyncResult asyncResult)
        {
            PerformSearchService searchService;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            RegServicesResult result;

            if (asyncObj.EndInvokeCalled == false)
            {

                searchService = (PerformSearchService)asyncObj.AsyncDelegate;
                result = searchService.EndInvoke(asyncObj);
                if (!(result is null)) //If null then something is wrong with Registry Service
                {
                    if (result.Status.Contains("Authenticated"))
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ServiceDropMenu.Items.Clear(); //Ensures no duplicates appear in the menu
                            foreach (RegistryJson service in result.Services)
                            {
                                ServiceDropMenu.Items.Add(service);
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show("Error: Please try again", "Retrieving All services");
                    }
                }
                else
                {
                    MessageBox.Show("Service is Down - Please Try Again Later", "Service Down Error");
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
        /// When the Drop Down Menu has a different selection, it will update the UI to display the current selected service details
        /// as well create amount of operand textboxes required by the service
        /// </summary>
        private void ServiceDropMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            RegistryJson selectedItem = (RegistryJson)cmb.SelectedItem;
            if (selectedItem != null)
            {
                ServiceName.Text = selectedItem.Name;
                ServiceDesc.Text = selectedItem.Description;
                ServicePoint.Text = selectedItem.APIEndpoint;
                ServiceNumOfOp.Text = selectedItem.NumberOfOperands.ToString();
                ServiceOperandType.Text = selectedItem.OperandType;

                uint numOfOperands = uint.Parse(ServiceNumOfOp.Text);
                //Removes All operand blocks and boxes to prevent any overlapping elements
                if (operandTextBoxes != null) //Only removes if there are elements (On first service selection, this will not run)
                {
                    foreach (TextBox box in operandTextBoxes.ToList())
                    {
                        operandTextBoxes.Remove(box);
                        MainDisplay.Children.Remove(box);
                    }
                    foreach (TextBlock block in operandTextBlocks.ToList())
                    {
                        operandTextBlocks.Remove(block);
                        MainDisplay.Children.Remove(block);
                    }
                }
               
                //Creates amount of Operand boxes and text according to ammount of of operands needed by service
                for (int i = 1; i <= numOfOperands; i++)
                {
                    TextBlock operandText = new TextBlock();
                    TextBox operandBox = new TextBox();

                    //Create Label 'Operand X: '
                    operandText.Text = "Operand " + i + ":";
                    operandText.Margin = new Thickness(480, (103 + ((i - 1) * 30)), 0, 0);
                    operandText.HorizontalAlignment = HorizontalAlignment.Left;
                    operandText.TextWrapping = TextWrapping.Wrap;
                    operandText.VerticalAlignment = VerticalAlignment.Top;

                    //Create Operand Box
                    operandBox.Margin = new Thickness(547, (102 + ((i - 1) * 30)), 0, 0);
                    operandBox.Height = 22;
                    operandBox.Width = 235;
                    operandBox.HorizontalAlignment = HorizontalAlignment.Left;
                    operandBox.VerticalAlignment = VerticalAlignment.Top;
                    //Adds the box and block to be displayed and into list to keep track
                    operandTextBlocks.Add(operandText);
                    operandTextBoxes.Add(operandBox);
                    MainDisplay.Children.Add(operandText);
                    MainDisplay.Children.Add(operandBox);
                }
                RunServiceBtn.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// OnClick for the RunService Button to allow user to execute the current selected service
        /// </summary>
        private void RunService_Btn(object sender, RoutedEventArgs e)
        {

            AsyncCallback callBackClick;

            CalcInputData input = new CalcInputData();
            input.Token = token;

            for (int i = 0; i < operandTextBoxes.Count; i++) //Input validation for operand int inputs
            {
                int val;
                if(Int32.TryParse(operandTextBoxes[i].Text, out val))
                {
                    input.Vals.Add(val);
                }
                else
                {
                    MessageBox.Show("Please Input Integers Only For Operands", "Invalid Input");
                    return;
                }
            }

            callBackClick = this.RunServiceCompletion;
            PerformRunService perform = new PerformRunService(this.DoRunService);
            perform.BeginInvoke(input, ServicePoint.Text, callBackClick, null);
        }

        /// <summary>
        /// Runs the selected service from services drop down menu on a new thread and uses operand text boxes for input of service
        /// </summary>
        /// <param name="input">CalcInputData object that contains all input details required for calc service to run</param>
        /// /// <param name="input">String that contains API Endpoint of the service in Service Provider</param>
        /// <returns>String that contains result of executed service</returns>
        private string DoRunService(CalcInputData input, string servicePoint)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
                ProgBar.IsIndeterminate = true;
            });

            string resultString = "No Result"; //Default error string which would only pass if service is down
            RestClient calcClient = new RestClient();
            IRestRequest request = new RestRequest(servicePoint, Method.POST).AddJsonBody(input);
            IRestResponse response = calcClient.Execute(request);
    
            if (servicePoint.Contains("Add") || servicePoint.Contains("Mul")) //Add or Multi Has same output object
            {
                AddMultiResult result = JsonConvert.DeserializeObject<AddMultiResult>(response.Content);
                if (!(result is null))
                {
                    resultString = result.Result.ToString();
                }
            }
            else if (servicePoint.Contains("Gen")) //Anything Gen is PrimeNum Gen which both have the same output object
            {
                PrimeNumsResult result = JsonConvert.DeserializeObject<PrimeNumsResult>(response.Content);
                if (!(result is null))
                {
                    resultString = string.Join(",", result.PrimeNumbers);
                }
            }
            else if (servicePoint.Contains("is")) //is refers to isPrime which has its own output object
            {
                PrimeCheckResult result = JsonConvert.DeserializeObject<PrimeCheckResult>(response.Content);
                if (!(result is null))
                {
                    resultString = result.Result.ToString();
                }
            }
            return resultString; //Returns string from result
        }

        /// <summary>
        /// On completion of the new RunService thread, it will process the returned data and display to user
        /// </summary>
        /// <param name="asyncResult">IAsyncResult object that contains the result of the service</param>
        private void RunServiceCompletion(IAsyncResult asyncResult)
        {
            PerformRunService runService;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            string result;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    runService = (PerformRunService)asyncObj.AsyncDelegate;
                    result = runService.EndInvoke(asyncObj);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!result.Contains("No Result"))
                        {
                            ServiceOutput.Text = result;
                        }
                        else
                        {
                            MessageBox.Show("Service is Down - Please Try Again Later", "Service Down Error"); //If null then either service is down or endpoint doesn't exist
                        }

                        UnlockElements();
                        ProgBar.IsIndeterminate = false;
                    });
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
                {
                    MessageBox.Show("Service is Down - Please Try Again Later", "Service Down Error");
                }

            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }

        /// <summary>
        /// Lock the UI so that the user can't mess anything while a service is being executed
        /// </summary>
        private void LockElements()
        {
            SearchServiceBtn.IsEnabled = false;
            AllServicesBtn.IsEnabled = false;
            RunServiceBtn.IsEnabled = false;

            foreach (TextBox box in operandTextBoxes.ToList())
            {
                box.IsReadOnly = true;
            }

        }

        /// <summary>
        /// Unlock the UI so that the user can interact with the interface again after a service has been executed
        /// </summary>
        private void UnlockElements()
        {
            SearchServiceBtn.IsEnabled = true;
            AllServicesBtn.IsEnabled = true;
            RunServiceBtn.IsEnabled = true;

            foreach (TextBox box in operandTextBoxes.ToList())
            {
                box.IsReadOnly = false;
            }
        }

        /// <summary>
        /// Runs token file check on a background thread
        /// </summary>
        private void TokenCheck()
        {
            PerformTokenCheck perform = new PerformTokenCheck(this.DoTokenCheck);
            perform.BeginInvoke(null, null);
        }

        /// <summary>
        /// Checks Token File to see if it is empty. If it is empty then it will close current window and display initial window
        /// Forces User to re-login as file get's cleared since the token will not be valid as it was removed by the Authentication server
        /// </summary>
        private void DoTokenCheck()
        {
            do
            {
                //Retrieves fileLength to check if file is empty or not
                long fileLength = new FileInfo(System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "AuthFiles/tokens.txt")).Length;
                if (fileLength == 0) //If file is empty then that means there are no tokens and should Log out the user
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Login Expired, Please Re-Login", "Tokens Cleared"); //Prompts the user what is happening
                        initialWindow.Show(); //Shows the initial window again 
                        this.Hide(); //Closes the window as the user is logged out 
                    });
                    return; //Once this window is closed, this current thread should stop doing anything
                }
            } while (true);
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
