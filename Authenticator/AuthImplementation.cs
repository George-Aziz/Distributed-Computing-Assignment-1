// Filename: AuthImplementation.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains the services provided by the Authentication Server
// Author:   George Aziz (19765453)
// Date:     17/04/2021

using AuthLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthImplementation : AuthInterface
    {
        private StreamWriter writer;
        private static Random rand = new Random(DateTime.Now.Second);
        private string accPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "AuthFiles/accounts.txt");
        private string tokenPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "AuthFiles/tokens.txt");

        /// <summary>
        /// Registers a user into the authentication server 
        /// </summary>
        /// <param name="name">String name of user that wants to be registered into the authentication server</param>
        /// <param name="password">String password of user that wants to be registered into the authentication server</param>
        public string Register(string name, string password)
        {
            string retVal;
            bool exist = false;

            if (!File.Exists(accPath)) //In case there is no file (First time Running for example)
            {
                File.Create(accPath).Close();
            }

            foreach (string line in File.ReadLines(accPath))
            {
                string[] account = line.Split(':');
                //Checks if name already registered (No duplicates allowed)
                if (account[0].Equals(name))
                {
                    exist = true;
                }
            }

            if (exist) { retVal = "Unsuccesful Registeration: Username already registered"; }
            else
            {
                writer = File.AppendText(accPath);
                writer.WriteLine(name + ":" + password);
                writer.Close();
                retVal = "Succesful Registeration";
            }

            return retVal;
            
        }

        /// <summary>
        /// Logs a user into the authentication server 
        /// </summary>
        /// <param name="name">String name of user that wants to be logged into the authentication server</param>
        /// <param name="password">String password of user that wants to be logged into the authentication server</param>
        public int Login(string name, string password)
        {
            int token = -1;
            foreach (string line in File.ReadLines(accPath))
            {
                //Checks inputted name and account exists within accounts file
                if (line.Equals(name + ":" + password))
                {
                    token = rand.Next(100000, 1000000); //Ensures integer 6 digits for Token
                    writer = File.AppendText(tokenPath);
                    writer.WriteLine(token);
                    writer.Close();
                    return token; // Provides token of user logged in to calling method
                }
            }
            return token; //If program reaches to this line then something has went wrong/no match found
        }

        /// <summary>
        /// Validates a user by checking their token against token file to see if it is active
        /// </summary>
        /// <param name="token">Integer of the token of a logged user </param>
        public string Validate(int token)
        {
            foreach (string line in File.ReadLines(tokenPath))
            {
                //Checks inputted name and account exists within accounts file
                if (line.Equals(token.ToString()))
                {
                    Debug.WriteLine("CONTAINS THE TOKEN!");
                    return "Validated";
                }
            }
            return "Not Valid"; //If program reaches to this line then something has went wrong/no match found
        }
    }
}
