// Filename: RegPublishInput.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains data for input of Publish service in Registry
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry_API_Classes
{
    public class RegPublishInput
    {
        public int Token;
        public string Name;
        public string Description;
        public string APIEndpoint;
        public uint NumberOfOperands;
        public string OperandType;
    }
}
