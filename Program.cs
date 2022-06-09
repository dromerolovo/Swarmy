using System;
using Swarmy;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Globalization;

namespace SwarmyNET 
{
    class SwarmyMain
{        static async Task Main(string[] args)
        {

            var textInfo = CultureInfo.InvariantCulture.TextInfo;

            SwarmyCore swarmyCore = new SwarmyCore();

            MLContext mlContext = new MLContext();
            AIModel aiModel = new AIModel();

            IdentityManager identityManager = new IdentityManager();

            // ITransformer model = aiModel.GenerateModel(mlContext);
            // aiModel.ClassifySingleImage(mlContext, model);

            


            identityManager.identityConstructor(20, InitializationCommand.Continue);


        }
    }
}
