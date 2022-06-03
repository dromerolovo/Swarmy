using System;
using Swarmy;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace SwarmyNET 
{
    class SwarmyMain
{        static async Task Main(string[] args)
        {
            SwarmyCore swarmyCore = new SwarmyCore();
            await swarmyCore.GetModelsProfiles(5, InitializationCommand.Continue);

            MLContext mlContext = new MLContext();
            AIModel aiModel = new AIModel();

            // ITransformer model = aiModel.GenerateModel(mlContext);
            // aiModel.ClassifySingleImage(mlContext, model);

        }
    }
}
