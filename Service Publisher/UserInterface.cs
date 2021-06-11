// Filename: UserInterface.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains the Interaction Logic with the Service Publisher Server that allows users to publish and unpublish services from Registry
// Author:   George Aziz (19765453)
// Date:     17/04/2021

using AuthLib;
using Newtonsoft.Json;
using Registry_API_Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service_Publisher
{
    internal class UserInterface
    {
        static void Main(string[] args)
        {
            //Interface for user to login, register or exit the server 
            Console.WriteLine("Welcome to the Service Publisher!");
            int initialOption = 0;
            do
            {
                Console.WriteLine("---===Please select one of the following options===---");
                Console.WriteLine("[1] Register Account");
                Console.WriteLine("[2] Login");
                Console.WriteLine("[3] Exit");
                initialOption = InputMenuOption("Option: ", 1, 3);
                switch (initialOption)
                {
                    case 1: //Register Account
                        string result = Register();
                        Console.WriteLine("\n" + result);
                        Console.WriteLine("============\n");
                        break;
                    case 2: //User Login
                        int token = Login(); //Retrieves token after login attempt
                        if (token == -1) //Invalid Credentials has been inputted
                        {
                            Console.WriteLine("\nInvalid Credentials - Please Try Again");
                            Console.WriteLine("============\n");
                        }
                        else if (token == -2) //Connection to Authentication Server Couldn't be made
                        {
                            Console.WriteLine("Authentication Server is Down - Please Try Again Later");
                            Console.WriteLine("============\n");
                        }
                        else //Succesful login which leads to giving access to publish, unpublish and returning to main menu
                        {
                            Console.WriteLine("\nSuccesfull Login!");
                            Console.WriteLine("============\n");
                            int curOption = -1;
                            do
                            {
                                Console.WriteLine("==Select one of the following options==");
                                Console.WriteLine("[1] Publish a service");
                                Console.WriteLine("[2] UnPublish a service");
                                Console.WriteLine("[3] Go back to main menu");

                                curOption = InputMenuOption("Option: ", 1, 3); ;
                                switch (curOption)
                                {
                                    case 1: //Publish Service
                                        PublishService(token);
                                        Console.WriteLine("============\n");
                                        break;
                                    case 2: //Unpublish Service
                                        UnPublishService(token);
                                        Console.WriteLine("============\n");
                                        break;
                                    case 3: //Go back to previous menu
                                        Console.WriteLine("\nYou have selected to go back to main menu");
                                        Console.WriteLine("============\n");
                                        break;
                                }
                            } while (curOption != 3);
                        }
                        break;
                    case 3: //Exit the server
                        Console.WriteLine("\nYou have selected to exit. Good Bye :)");
                        break;
                }
            } while (initialOption != 3); //Option 3 is to exit
        }

        /// <summary>
        /// Interface for logging into the system
        /// </summary>
        /// <param name="authConnection">AuthInterface to interact with the Authentication server</param>
        private static string Register()
        {
            try
            { 
                Console.WriteLine("==REGISTER==");
                string username = InputAccountDetail("Username: ");
                string password = InputAccountDetail("Password: ");

                //Creates a new connection right before registration is called to server since it may have been down before and is up now
                NetTcpBinding tcp = new NetTcpBinding();
                string authURL = "net.tcp://localhost/AuthService";
                ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
                AuthInterface authConnection = authFactory.CreateChannel();

                return authConnection.Register(username, password);
            }
            catch(Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                return "Authentication Server is Down - Please Try Again Later";
            }
}

        /// <summary>
        /// Interface for logging into the system
        /// </summary>
        /// <param name="authConnection">AuthInterface to interact with the Authentication server</param>
        private static int Login()
        {
            try
            {
                Console.WriteLine("==LOGIN==");
                string username = InputAccountDetail("Username: ");
                string password = InputAccountDetail("Password: ");

                //Creates a new connection right before login is called to server since it may have been down before and is up now
                NetTcpBinding tcp = new NetTcpBinding();
                string authURL = "net.tcp://localhost/AuthService";
                ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
                AuthInterface authConnection = authFactory.CreateChannel();

                return authConnection.Login(username, password);
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                return -2; //Invalid Token
            }
        }

        /// <summary>
        /// Interface for Publishing a service to the Registry
        /// </summary>
        /// <param name="token">Token of logged in user</param>
        private static void PublishService(int token)
        {
           
            string URL = "https://localhost:44367/api/Registry";
            RestClient client = new RestClient(URL);
            RegPublishInput json = new RegPublishInput();

            Console.WriteLine("==PUBLISH SERVICE==");
            json.Name = InputString("Service Name: ");
            json.Description = InputString("Service Description: ");
            json.APIEndpoint = "https://localhost:44325/api/Calculations/" + InputString("Service Endpoint:  https://localhost:44325/api/Calculations/");
            json.NumberOfOperands = InputOperandNum("Service Number of Operands: ");
            json.OperandType = InputString("Service Operand Type: ");
            json.Token = token;
            IRestRequest request = new RestRequest("Publish", Method.POST).AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            RegPubAndUnPubResult result = JsonConvert.DeserializeObject<RegPubAndUnPubResult>(response.Content);

            if (!(result is null)) //If null then something is wrong with Registry Service
            {
                if (!String.IsNullOrWhiteSpace(result.Status))
                {
                    Console.WriteLine("\n\nStatus: " + result.Status);
                    Console.WriteLine("Reason: " + result.Reason);

                    if (result.Status.Contains("Denied"))
                    {
                        Console.WriteLine("Please go back and re-login in order to use any service");
                    }
                }
                else
                {
                    Console.WriteLine("Authenticator Service is down - Please Try Again Later");
                }
            }
            else
            {
                Console.WriteLine("A required Service is down - Please Try Again Later"); //Registry Service is down or Auth Server is down
            } 
        }

        /// <summary>
        /// Interface for Unpublishing a service from the Registry
        /// </summary>
        /// <param name="token">Token of logged in user</param>
        private static void UnPublishService(int token)
        {
            string URL = "https://localhost:44367/api/Registry";
            RestClient client = new RestClient(URL);
            RegUnPublishInput json = new RegUnPublishInput();

            Console.WriteLine("==UNPUBLISH SERVICE==");
            Console.Write("\nService Endpoint: https://localhost:44325/api/Calculations/");
            json.APIEndpoint = "https://localhost:44325/api/Calculations/" + Console.ReadLine();
            json.Token = token;

            IRestRequest request = new RestRequest("UnPublish", Method.POST).AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            RegPubAndUnPubResult result = JsonConvert.DeserializeObject<RegPubAndUnPubResult>(response.Content);
            if (!(result is null)) //If not null then everything is fine
            {
                if (!String.IsNullOrWhiteSpace(result.Status)) //If status is null then authenticator is down
                {
                    Console.WriteLine("\n\nStatus: " + result.Status);
                    Console.WriteLine("Reason: " + result.Reason);

                    if (result.Status.Contains("Denied")) //If Denied then re-login is required
                    {
                        Console.WriteLine("Please go back and re-login in order to use any service");
                    }
                }
                else
                {
                    Console.WriteLine("Authenticator Service is down - Please Try Again Later");
                }
            }
            else //If null then something is wrong with Registry Service or Authentication server depending on the flow of events
            {
                Console.WriteLine("A required Service is down - Please Try Again Later"); 
            } 
        }

        /// <summary>
        /// String input validation
        /// </summary>
        /// <param name="prompt">Output prompt to user</param>
        private static string InputString(string prompt)
        {
            string outString, retString;
            outString = "\n" + prompt; //The output is set to the prompt that is imported

            do
            {
                Console.Write(outString); //Outputs the prompt for the user
                outString = "ERROR: Input cannot be empty or null" + "\n" + prompt; //Makes prompt include error message for next iteration
                retString = Console.ReadLine();

            } while (String.IsNullOrWhiteSpace(retString));

            return retString;
        }

        /// <summary>
        /// Account Details string input validation
        /// </summary>
        /// <param name="prompt">Output prompt to user</param>
        private static string InputAccountDetail(string prompt)
        {
            string outString, retString;
            outString = "\n" + prompt; //The output is set to the prompt that is imported

            do
            {
                Console.Write(outString); //Outputs the prompt for the user
                outString = "ERROR: Input cannot be empty or null or contain whitespace" + "\n" + prompt; //Makes prompt include error message for next iteration
                retString = Console.ReadLine();

            } while ((String.IsNullOrWhiteSpace(retString) || retString.Contains(" ")));

            return retString;
        }

        /// <summary>
        /// Operand Number input validation
        /// </summary>
        /// <param name="prompt">Output prompt to user</param>
        public static uint InputOperandNum(String prompt)
        {
            string outString, inputNum;
            bool success;
            uint retInt;
            outString = "\n" + prompt; //The output is set to the prompt that is imported     

            do
            {
                Console.Write(outString); //Outputs the prompt for the user
                outString = "ERROR: Input cannot be negative or empty and must be an integer" + "\n" + prompt; //Makes prompt include error message for next iteration
                inputNum = Console.ReadLine();
                success = uint.TryParse(inputNum, out retInt);
            }
            while (!success); 
            return retInt;
        }

        /// <summary>
        /// Menu Option input validation
        /// </summary>
        /// <param name="prompt">Output prompt to user</param>
        /// <param name="min">Min Value Bound</param>
        /// <param name="max">Max Value Bound</param>
        public static int InputMenuOption(String prompt, int min, int max)
        {
            string outString, inputNum;
            bool success;
            int retInt;
            outString = "\n" + prompt; //The output is set to the prompt that is imported     

            do
            {
                Console.Write(outString); //Outputs the prompt for the user
                outString = "ERROR: Input cannot be empty & must be between " + min + " and " + max + "\n" + prompt; //Makes prompt include error message for next iteration
                inputNum = Console.ReadLine();
                success = int.TryParse(inputNum, out retInt);
            }
            while (!success || (retInt < min || retInt > max) );
            return retInt;
        }
    }
}
