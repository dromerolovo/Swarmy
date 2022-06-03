using Microsoft.ML;
using Microsoft.ML.Data;

namespace Swarmy
{
     public struct Paths
    {
        public static string assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        public static string imagesFolder = Path.Combine(assetsPath, "images");
        public static string trainTagsTsv = Path.Combine(imagesFolder, "tags.tsv");
        public static string testTagsTsv = Path.Combine(imagesFolder, "test-tags.tsv");
        public static string predictSingleImage = Path.Combine(imagesFolder, "toaster3.jpg");
        public static string inceptionTensorFlowModel = Path.Combine(assetsPath, "inception", "tensorflow_inception_graph.pb");

    }

    public struct InceptionSettings
    {
        public const int ImageHeight = 224;
        public const int ImageWidth = 224;
        public const float Mean = 117;
        public const float Scale = 1;
        public const bool ChannelsLast = true;

    }

    public class ImageData
    {
        [LoadColumn(0)]
        public string? ImagePath;

        [LoadColumn(1)]
        public string? Label;
    }

    public class ImagePrediction : ImageData
    {
        public float[]? Score;

        public string? PredictedLabelValue;
    }

    public class AIModel
    {

        public ITransformer GenerateModel(MLContext mlContext)
        {
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: Paths.imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
            .Append(mlContext.Model.LoadTensorFlowModel(Paths.inceptionTensorFlowModel).
            ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
            .AppendCacheCheckpoint(mlContext);

            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: Paths.trainTagsTsv, hasHeader: false);
            ITransformer model = pipeline.Fit(trainingData);

            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: Paths.testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);

            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                    labelColumnName: "LabelKey",
                    predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

            return model;

        }

        public void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach( ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score!.Max()}");
            }
        }

        public void ClassifySingleImage(MLContext mlContext, ITransformer model)
        {
             var imageData = new ImageData()
             {
                 ImagePath = Paths.predictSingleImage
             };

             var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
             var prediction = predictor.Predict(imageData);

             Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score!.Max()} ");
        }
    }
}