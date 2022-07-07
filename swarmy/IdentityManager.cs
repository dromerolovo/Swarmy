using System;
using System.Collections;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;

namespace Swarmy
{
    public enum InitializationCommand {StartOver, Continue}

    sealed public class IdentityManager
    {   
        

        public class IdentityName {

            private string? name;
            public string Name
            {
                get => name!;
                set
                {   
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;
                    name = textInfo.ToTitleCase(value);
                }
                
            }
        }

        public class MaleName : IdentityName {}

        public class FemaleName : IdentityName {}
        
        public class Surname : IdentityName {}



        public static List<String> GetDataFromCsv<T>(T obj, string pathToFile, string property )
        {
            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            var recordsList = new List<string> {};


            using( var StreamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, pathToFile)))
            {
                using(var csvReader = new CsvReader(StreamReader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    var records = csvReader.GetRecords<T>().ToList();
                    foreach( var record in records)
                    {
                        var value = record!.GetType().GetProperty(property)!.GetValue(record);
                        var valueLower = textInfo.ToLower(value!.ToString()!);
                        record!.GetType().GetProperty(property)!.SetValue(record, valueLower);
                        recordsList.Add(record!.GetType().GetProperty(property)!.GetValue(record)!.ToString()!);
                    }
                }
            }

            return recordsList;
        }

        public void identityConstructor(int numberOfRecords, InitializationCommand initializationCommand)
        {
            if(initializationCommand == InitializationCommand.Continue && 
            File.Exists(Path.Combine(Environment.CurrentDirectory, "./identityManagment/identities.csv")) == false)
            {
                throw new ArgumentException("You shouldn't use InitializationCommand.Continue if there is no identities.csv file");
            }

            List<Identity>? identityRecords = new List<Identity>{};

            List<string> listFemaleName = IdentityManager.GetDataFromCsv(new IdentityManager.FemaleName(), "./identityManagment/raw_data/female_names.csv","Name");
            List<string> listMaleName = IdentityManager.GetDataFromCsv(new IdentityManager.MaleName(), "./identityManagment/raw_data/male_names.csv","Name");
            List<string> listSurnames = IdentityManager.GetDataFromCsv(new IdentityManager.Surname(), "./identityManagment/raw_data/surnames.csv","Name");

            for(var i = 0; i < numberOfRecords; i++)
            {
                Identity identity = new Identity();
                Random random = new Random();
                int randomNumber = random.Next(1,149);
                int randomNumberGender = random.Next(0, 2);
                string uuid = Guid.NewGuid().ToString();
                identity.Id = uuid;
                identity.Gmail = false;

                if(randomNumberGender == 0)
                {
                    identity.Genre = "Female";
                    identity.Name = listFemaleName[randomNumber];
                } 
                else
                {
                    identity.Genre = "Male";
                    identity.Name = listMaleName[randomNumber];
                }

                identity.Surname = listSurnames[randomNumber];

                identityRecords!.Add(identity);
            }

            if(initializationCommand == InitializationCommand.StartOver)
            {
                using( var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "./identityManagment/identities.csv")))
                {
                    using(var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(identityRecords!);
                    }
                }
            }
            else if(initializationCommand == InitializationCommand.Continue)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);

                config.HasHeaderRecord = false;

                using( var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "./identityManagment/identities.csv"), true))
                {
                    using(var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecords(identityRecords!);
                        csv.Flush();
                    }
                }
            }
        }

        public int getRecordsCount() {

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            int count = 0;

            config.HeaderValidated = null;

            using (TextReader dataCsvFileReader = File.OpenText(Path.Combine(Environment.CurrentDirectory, "./identityManagment/identities.csv")))
            {
                using (CsvReader dataCsvReader = new CsvReader(dataCsvFileReader, CultureInfo.InvariantCulture))
                {
                    var records = dataCsvReader.GetRecords<Identity>().ToList();
                    var filteredRecords = records.Where(element => element.Gmail == false);
                    count = filteredRecords.Count();
                }
            }

            return count;
        }
    }



    public class Identity
    {
        public string? Id {get; set;}
        public string? Genre {get; set;}
        public string? Name {get; set;}
        public string? Surname {get; set;}
        public bool? Gmail {get; set;}
        

    }
}