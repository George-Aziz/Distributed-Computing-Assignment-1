// Filename: RegistryJson.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains the format of JSONs passed to and from the services.txt file
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry_API_Classes
{
    public class RegistryJson
    {
        public string Name;
        public string Description;
        public string APIEndpoint;
        public uint NumberOfOperands;
        public string OperandType;

        public override string ToString()
        {
            return Name;
        }
    }
}
