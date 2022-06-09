using System;
using System.Collections;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;

namespace Swarmy
{
    public enum InitializationCommand {StartOver, Continue}

    public class IdentityManager
    {   
        

        public class IdentityName {

            protected string? name;
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


            //./identityManagment/raw_data/female_names.csv
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
                int randomNumberSex = random.Next(0, 2);
                string uuid = Guid.NewGuid().ToString();
                identity.Id = uuid;

                if(randomNumberSex == 0)
                {
                    identity.Sex = "Female";
                    identity.Name = listFemaleName[randomNumber];
                } 
                else
                {
                    identity.Sex = "Male";
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

            // using( var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "./identityManagment/identities.csv")))
            // {
            //     using(var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //     {
                    // if (initializationCommand == InitializationCommand.Continue)
                    // {
                        
                    //     int count = 0;

                    //     foreach(var record in identityRecords)
                    //     {
                    //         count++;
                    //         if(count <= limit)
                    //         {
                    //             csv.NextRecord();
                    //             csv.Row
                    //         }
                    //         else
                    //         {
                    //             csv.WriteRecord(record);
                    //             csv.NextRecord();
                    //         }
                    //     }

                    // }
                    // else if(initializationCommand == InitializationCommand.StartOver)
                    // {
                    //     csv.WriteRecords(identityRecords!);
                    //     writer.Flush();
                    // }
            //     }
            // }
        }
    }

    public class Identity
    {
        public string? Id {get; set;}
        public string? Sex {get; set;}
        public string? Name {get; set;}
        public string? Surname {get; set;}
        public string? ProfilePicture {get; set;}

    }
}