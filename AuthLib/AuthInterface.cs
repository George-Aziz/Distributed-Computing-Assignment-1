// Filename: AuthInterface.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Interface class that provides other classes to interact with the services provided by the Authentication Server
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib
{
    [ServiceContract]
    public interface AuthInterface
    {
        [OperationContract]
        string Register(string name, string password);
        [OperationContract]
        int Login(string name, string password);
        [OperationContract]
        string Validate(int token);
    }
}
