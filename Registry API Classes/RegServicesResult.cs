// Filename: RegServicesResult.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains data for result of registry services
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry_API_Classes
{
    public class RegServicesResult
    {
        public string Status;
        public string Reason;
        public List<RegistryJson> Services;
    }
}
