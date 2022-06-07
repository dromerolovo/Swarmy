using System;
using CsvHelper;

namespace Swarmy
{
    public class Utils
    {
        public class FemaleName
        {
            public string? Name {get; set;}
        }

        dynamic readCsv() {
            
            using(var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "female_names.csv")))
        }
    }
}