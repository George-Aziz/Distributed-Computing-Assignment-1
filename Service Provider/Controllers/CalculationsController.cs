// Filename: CalculationsController.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Web API Controller for Service Provider Service that contains all calculation services
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using AuthLib;
using RestSharp;
using Service_Provider_API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;

namespace Service_Provider.Controllers
{
    public class CalculationsController : ApiController
    {
        private NetTcpBinding tcp = new NetTcpBinding();
        private string URL = "net.tcp://localhost/AuthService";
        private AuthInterface authConnection;

        //Constructor for Controller that gets called when the controller is called which will setup the connection to the authentication server
        public CalculationsController()
        {
            ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, URL);
            authConnection = authFactory.CreateChannel();
        }

        /// <summary>
        /// Web Service that Adds two numbers together
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>AddMultiResult object that contains all data to be returned</returns>
        [Route("api/Calculations/AddTwo/")]
        [HttpPost]
        public AddMultiResult AddTwoNumbers(CalcInputData request)
        {
            AddMultiResult retVal = new AddMultiResult();
            try
            { 
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Adding Two Numbers";
                    retVal.Result = request.Vals[0] + request.Vals[1];
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Adding Two Numbers Authentication Error";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that Adds three numbers together
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>AddMultiResult object that contains all data to be returned</returns>
        [Route("api/Calculations/AddThree/")]
        [HttpPost]
        public AddMultiResult AddThreeNumbers(CalcInputData request)
        {
            AddMultiResult retVal = new AddMultiResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Adding Three Numbers";
                    retVal.Result = request.Vals[0] + request.Vals[1] + request.Vals[2];
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Adding Three Numbers Authentication Error";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that Multiplies two numbers together
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>AddMultiResult object that contains all data to be returned</returns>
        [Route("api/Calculations/MulTwo/")]
        [HttpPost]
        public AddMultiResult MulTwoNumbers(CalcInputData request)
        {
            AddMultiResult retVal = new AddMultiResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Multiplying Two Numbers";
                    retVal.Result = request.Vals[0] * request.Vals[1];
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Multiplying Two Numbers Authentication Error";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that Multiplies three numbers together
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>AddMultiResult object that contains all data to be returned</returns>
        [Route("api/Calculations/MulThree/")]
        [HttpPost]
        public AddMultiResult MulThreeNumbers(CalcInputData request)
        {
            AddMultiResult retVal = new AddMultiResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Multiplying Three Numbers";
                    retVal.Result = request.Vals[0] * request.Vals[1] * request.Vals[2];
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Multiplying Three Numbers Authentication Error";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that generates prime numbers to an inputted value
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>PrimeNumsResult object that contains all data to be returned</returns>
        [Route("api/Calculations/GenPrimeToVal/")]
        [HttpPost]
        public PrimeNumsResult GeneratePrimeNumberstoValue(CalcInputData request)
        {
            List<int> results = new List<int>();
            PrimeNumsResult retVal = new PrimeNumsResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    for (int i = 1; i <= request.Vals[0]; i++)
                    {
                        if (checkPrime(i))
                        {
                            results.Add(i);
                        }
                    }
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Generating Prime Numbers to " + request.Vals[0].ToString();
                    retVal.PrimeNumbers = results;
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Generating Prime Numbers to " + request.Vals[0].ToString() + " Authentication Error";
                    retVal.PrimeNumbers = null;
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that generates prime numbers between 2 inputted values
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>PrimeNumsResult object that contains all data to be returned</returns>
        [Route("api/Calculations/GenPrimeInRange/")]
        [HttpPost]
        public PrimeNumsResult GeneratePrimeNumbersinRange(CalcInputData request)
        {
            List<int> results = new List<int>();
            PrimeNumsResult retVal = new PrimeNumsResult();
            try
            {
                int min = request.Vals[0];
                int max = request.Vals[1];

                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    for (int i = min; i <= max; i++)
                    {
                        if (checkPrime(i))
                        {
                            results.Add(i);
                        }
                    }
                    retVal.Status = "Authenticated";
                    retVal.Reason = "Generating Prime Numbers between " + min.ToString() + " and " + max.ToString();
                    retVal.PrimeNumbers = results;
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Generating Prime Numbers between " + min.ToString() + " and " + max.ToString() + " Authentication Error";
                    retVal.PrimeNumbers = null;
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that checks if an inputted value is a prime number or not
        /// </summary>
        /// <param name="request">CalcInputData object that contains all required input for the service</param>
        /// <returns>PrimeCheckResult object that contains all data to be returned</returns>
        [Route("api/Calculations/isPrime/")]
        [HttpPost]
        public PrimeCheckResult isPrimeNumber(CalcInputData request)
        {
            PrimeCheckResult retVal = new PrimeCheckResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    if (checkPrime(request.Vals[0]))
                    {
                        retVal.Status = "Authenticated";
                        retVal.Reason = "Checking Prime Number";
                        retVal.Result = true;
                    }
                    else
                    {
                        retVal.Status = "Authenticated";
                        retVal.Reason = "Checking Prime Number";
                        retVal.Result = false;
                    }
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Checking Prime Number Authentication Error";
                    retVal.Result = false;
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Checks if a value inputted is a prime number or not
        /// </summary>
        /// <param name="val">Integer which contains the number that will be checked</param>
        /// <returns>Boolean indicating whether the value is a prime number or not</returns>
        private bool checkPrime(int val)
        {
            if(val <= 1)
            {
                return false;
            }

            for (int i = 2; i < val; i++)
            {
                if (val % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
