// Filename: PrimeCheckResult.cs
// Project:  DC Assignment 1 (COMP3008)
// Purpose:  Class that contains data for the result that comes from the Prime Number Check service from Service Provider
// Author:   George Aziz (19765453)
// Date:     11/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Provider_API_Classes
{
    public class PrimeCheckResult
    {
        public string Status;
        public string Reason;
        public bool Result;
    }
}
