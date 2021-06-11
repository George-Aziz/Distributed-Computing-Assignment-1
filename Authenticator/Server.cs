// Filename: Server.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains the Interaction Logic with the Authentication Server as well as Periodic Cleanup of Tokens Setup
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using AuthLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Authenticator
{
    public class Server
    {
        //Delagate Definition for periodic Token clearing
        private delegate void PerformClearTokens(int interval);

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Authentication server!");

            //Server Network Setup
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding(); //This represents a tcp/ip binding in the Windows network stack
            host = new ServiceHost(typeof(AuthImplementation)); //Bind server to the implementation of Authentication Server
            host.AddServiceEndpoint(typeof(AuthInterface), tcp, "net.tcp://localhost/AuthService"); //Service Endpoint
            //Open Host
            host.Open();
            Console.WriteLine("Authenticaiton System is now online");

            //Menu for User to ask whether or not they would like to clear tokens periodically
            int curOption = -1;
            bool complete = false;
            do
            {
                Console.WriteLine("=Would you like to periodically clear login tokens every X amount of minutes?=");
                Console.WriteLine("[1] Yes");
                Console.WriteLine("[2] No");

                //User menu for how frequent the tokens should be cleared up
                curOption = Int32.Parse(Console.ReadLine());
                if(curOption == 1)
                {
                    int input = 0;
                    do
                    {
                        Console.WriteLine("You have selected to clear tokens every X amount of minutes");
                        Console.WriteLine("Please input how often you would like to clear up the tokens (Input in minutes and be at least 1 minute): ");
                        input = Int32.Parse(Console.ReadLine());
                        if (input < 1) //If the input is less that 1 then it is an invalid input
                        {
                            Console.WriteLine("Error: Please make sure the time interval is at least 1 minute");
                        }
                        else //Proceed to open the new thread to run in background for Token Clean up
                        {
                            complete = true;
                            PerformClearTokens perform = new PerformClearTokens(CleanUp);
                            perform.BeginInvoke(input, null, null);
                            Console.WriteLine("You have selected to periodically clear tokens every " + input + " minutes");
                        }
                    } while (input < 1);
                }
                else if (curOption == 2)
                {
                    Console.WriteLine("You have selected to not periodically clear tokens");
                }
            } while (curOption != 2 && complete == false);

            Console.WriteLine("\n==Any Input Now Will Close & Exit the Server==");
            Console.ReadLine(); //If the user has selected to not clear tokens, let the console stay open until any further input

            //Ensure to close host after everything is complete
            host.Close();
        }

        /// <summary>
        /// Clears token file full of user tokens every X amount of minutes
        /// </summary>
        /// <param name="interval">Integer that indicates how frequently the token file gets cleared</param>
        private static void CleanUp(int interval)
        {
            while (true) //Runs as long as the server is open in the background
            {
                System.Threading.Thread.Sleep(1000 * 60 * interval); //Waits however minutes before executing the actual function
                Console.WriteLine("Clearing tokens...");
                File.CreateText(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "AuthFiles/tokens.txt")).Close();
            }
        }
    }
}
