// Filename: RegistryController.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Web API Controller for Registry Service
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using AuthLib;
using System.ServiceModel;
using Registry_API_Classes;

namespace Registry.Controllers
{
    public class RegistryController : ApiController
    {
        //Registry Services File services.txt
        private string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "RegistryFiles/services.txt");
       
        private NetTcpBinding tcp = new NetTcpBinding();
        private string authURL = "net.tcp://localhost/AuthService";
        private AuthInterface authConnection;

        //Constructor for Controller that gets called when the controller is called which will setup the connection to the authentication server
        public RegistryController()
        {
            ChannelFactory<AuthInterface> authFactory = new ChannelFactory<AuthInterface>(tcp, authURL);
            authConnection = authFactory.CreateChannel();
        }

        /// <summary>
        /// Web Service that publishes a server to the services.txt file containing all services
        /// </summary>
        /// <param name="request">RegPublishInput object that contains all required input for the service</param>
        /// <returns>RegPubAndUnPubResult object that contains all data to be returned</returns>
        [Route("api/Registry/Publish")]
        [HttpPost]
        public RegPubAndUnPubResult Publish(RegPublishInput request)
        {
            RegPubAndUnPubResult retVal = new RegPubAndUnPubResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    if (!File.Exists(path)) //In case there is no file (First time Running for example)
                    {
                        File.Create(path).Close();
                    }

                    //Reads all current services
                    string jsonData = File.ReadAllText(path);
                    List<RegistryJson> servicesList = JsonConvert.DeserializeObject<List<RegistryJson>>(jsonData) ?? new List<RegistryJson>();

                    //Creates the format needed for file input without the token from the request
                    RegistryJson json = new RegistryJson();
                    json.Name = request.Name;
                    json.Description = request.Description;
                    json.APIEndpoint = request.APIEndpoint;
                    json.NumberOfOperands = request.NumberOfOperands;
                    json.OperandType = request.OperandType;
                    servicesList.Add(json);

                    //Adds new service with the other services and since its with the others, when it is written to file it will be written in correct format
                    jsonData = JsonConvert.SerializeObject(servicesList, Formatting.Indented);
                    File.WriteAllText(path, jsonData);

                    retVal.Status = "Authenticated";
                    retVal.Reason = "Publishing " + request.Name + " Service";
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Publishing " + request.Name + " Service";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that searches for services within the services.txt file 
        /// </summary>
        /// <param name="request">RegSearchInput object that contains all required input for the service</param>
        /// <returns>RegServicesResult object that contains all data to be returned</returns>
        [Route("api/Registry/Search")]
        [HttpPost]
        public RegServicesResult Search(RegSearchInput request)
        {
            RegServicesResult retVal = new RegServicesResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    if (!File.Exists(path)) //In case there is no file (First time Running for example)
                    {
                        File.Create(path).Close();
                    }
                    //Reads all current services
                    string jsonData = File.ReadAllText(path);
                    List<RegistryJson> servicesList = JsonConvert.DeserializeObject<List<RegistryJson>>(jsonData) ?? new List<RegistryJson>();

                    //Searches for matching description and inserts into list that will be returned
                    List<RegistryJson> retServiceList = new List<RegistryJson>();
                    foreach (RegistryJson curJson in servicesList)
                    {
                        if (curJson.Description.ToUpper().Contains(request.Description.ToUpper()))
                        {
                            retServiceList.Add(curJson);
                        }
                    }

                    retVal.Status = "Authenticated";
                    retVal.Reason = "Searching for " + request.Description + " services";
                    retVal.Services = retServiceList;
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Searching for " + request.Description + " services";
                    retVal.Services = null; //For if there is no services matched due to invalid token
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Web Service that searches retrieves all services from the services.txt file
        /// </summary>
        /// <param name="request">RegAllServicesInput object that contains all required input for the service</param>
        /// <returns>RegServicesResult object that contains all data to be returned</returns>
        [Route("api/Registry/AllServices")]
        [HttpPost]
        public RegServicesResult AllServices(RegAllServicesInput request)
        {
            RegServicesResult retVal = new RegServicesResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    if (!File.Exists(path)) //In case there is no file (First time Running for example)
                    {
                        File.Create(path).Close();
                    }
                    //Reads all current services
                    string jsonData = File.ReadAllText(path);
                    List<RegistryJson> servicesList = JsonConvert.DeserializeObject<List<RegistryJson>>(jsonData) ?? new List<RegistryJson>();

                    retVal.Status = "Authenticated";
                    retVal.Reason = "Retrieving all services";
                    retVal.Services = servicesList;
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Retrieving all services";
                    retVal.Services = null; //For if there is no services matched due to invalid token
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }


        /// <summary>
        /// Web Service that unpublishes a server from the services.txt file containing all services
        /// </summary>
        /// <param name="request">RegUnPublishInput object that contains all required input for the service</param>
        /// <returns>RegPubAndUnPubResult object that contains all data to be returned</returns>
        [Route("api/Registry/UnPublish")]
        [HttpPost]
        public RegPubAndUnPubResult UnPublish(RegUnPublishInput request)
        {
            RegPubAndUnPubResult retVal = new RegPubAndUnPubResult();
            try
            {
                string response = authConnection.Validate(request.Token);
                if (response.Equals("Validated"))
                {
                    if (!File.Exists(path)) //In case there is no file (First time Running for example)
                    {
                        File.Create(path).Close();
                    }

                    //Reads all current services
                    string jsonData = File.ReadAllText(path);
                    List<RegistryJson> servicesList = JsonConvert.DeserializeObject<List<RegistryJson>>(jsonData) ?? new List<RegistryJson>();

                    int count = servicesList.RemoveAll(curJson => curJson.APIEndpoint.ToUpper().Equals(request.APIEndpoint.ToUpper()));

                    //Re-writes the file with correct formatting with the services that were matched removed
                    jsonData = JsonConvert.SerializeObject(servicesList, Formatting.Indented);
                    File.WriteAllText(path, jsonData);

                    retVal.Status = "Authenticated";
                    retVal.Reason = "Unpublishing " + count + " service(s)";
                }
                else
                {
                    retVal.Status = "Denied";
                    retVal.Reason = "Unpublishing service";
                }
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {
                retVal = null;
            }

            return retVal;
        }
    }
}
