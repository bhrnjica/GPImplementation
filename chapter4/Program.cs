using System;
using System.Linq;
using System.Collections.Generic;
namespace GPImplemenation
{
    class Program
    {
        static double[][] currentDataSet;
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        static void Main(string[] args)
        {
            Console.Title = "Optimized Genetic Programming Applications: Emerging Research and Opportunities";

            //read type value from command line argument list
            if (!int.TryParse(args[0], out int type))
                type = 1;

            if (type == 1)
                Console.WriteLine("GP Regression is selected!");
            else
                Console.WriteLine("GP Classification is selected!");

            double[][] dataSet = null;
            // 
            dataSet = getDataSet(type);
            //Split data into train and test data
            Console.WriteLine("Creating 80% training and 20% test data data");
            int count = (int)(dataSet.Length * 0.8);
            var trainigData = dataSet.Take(count).ToArray();
            var testDataSet = trainigData.Reverse().Take(dataSet.Length - count).Reverse().ToArray();
            //set training data set  
            currentDataSet = trainigData;
            Console.WriteLine("First 3 rows of training data set");
            printData(trainigData, 3, 3, true);

            Console.WriteLine("First 3 rows of testing data set");
            printData(testDataSet, 3, 3, true);

            Console.WriteLine("Generate GP Parameters");
            var param = generateParameters(type);
            var terminalSet = generateTerminalSet(trainigData, param.Constants);
            var functionSet = generateFunctionSet();

            Console.WriteLine("Create Initial population");
            var pop = new Population();
            pop.Initialize(param, functionSet, terminalSet);
            pop.EvaluatePopulation(param);
            //define iteration count
            int iteration = 150;

            Console.WriteLine("Run GPsearch algorithm.");
            //perform gp run
            var bestSolution = GPRun(pop, param, functionSet, terminalSet, iteration);

            //Show best solution results
            Console.WriteLine();
            Console.WriteLine($"Best found solution: {bestSolution.Node.Print(terminalSet)}");

            //evaluation of multi class model
            if (type == 2)
            {
                // printOutput(bestSolution.Node, param);
                var fitt = accracy_Fitness(bestSolution.Node, param);
                Console.WriteLine($"Accuracy for training data Acc={fitt}!");
                //test GP model on test data set
                currentDataSet = testDataSet;
                fitt = accracy_Fitness(bestSolution.Node, param);
                Console.WriteLine($"Accuracy for testing data Acc={fitt}!");
            }
            //evaluation of regression model
            else
            {
                // 
                var rv = calculateOutput(bestSolution.Node, param);
                var r = calculateR(rv.obsrv, rv.predc);
                Console.WriteLine($"R value for training data R={r}!");
                //
                currentDataSet = testDataSet;
                rv = calculateOutput(bestSolution.Node, param);
                r = calculateR(rv.obsrv, rv.predc);
                Console.WriteLine($"R value for testing data R={r}!");
            }
            //
            Console.WriteLine("Press any key to continue!");
            Console.Read();
        }

        static Chromosome GPRun(Population p, Parameters par, Function[] fSet, string[] tSet, int it)
        {
            Chromosome bestSol = null;
            for (int i = 1; i <= it; i++)
            {
                p.Crossover(par, fSet, tSet);
                p.Mutate(par, fSet, tSet);
                p.Reproduction(par, fSet, tSet);
                p.EvaluatePopulation(par);
                p.CreateNewGeneration(par, fSet, tSet);
                //select best solution from the current generation
                bestSol = p.Chromosomes.OrderByDescending(x => x.Fitness).FirstOrDefault();

                //report to console about progress
                if (i % 10 == 0 || i == 1)
                    Console.WriteLine($"Iteration={i}, fitness value for the Best solution Fitness={bestSol.Fitness} out of 1.0.");
            }
            return bestSol;
        }

        static double[][] getDataSet(int type)
        {
            double[][] regData = new double[][] {
    //              x1      y
                new double[]{7,  132} ,
                new double[]{5,  64} ,
                new double[]{7,  132} ,
                new double[]{4,  39} ,
                new double[]{7,  132} ,
                new double[]{8,  175} ,
                new double[]{7,  132} ,
                new double[]{5,  64} ,
                new double[]{5,  64} ,
                new double[]{7,  132} ,
                new double[]{0,  -1} ,
                new double[]{6,  95} ,
                new double[]{ -2, 15} ,
               new double[]{ 1,  0} ,
               new double[]{-2, 15} ,
               new double[]{ 4,  39} ,
              new double[]{ -3, 32} ,
              new double[]{ -2, 15} ,
               new double[]{ 8,  175} ,
               new double[]{ 0,  -1} ,
    };

            double[][] mccData = new double[][] {
    // inputs: (sepal_l, sepal_w,petal_l,pental_w)
    // output (last column): setosa = 0,versicolor = 1,virginica = 2
        new double[] { 5.1, 3.5, 1.4, 0.2, 0 },
        new double[] { 4.9, 3.0, 1.4, 0.2, 0 },
        new double[] { 4.7, 3.2, 1.3, 0.2, 0 },
        new double[] { 4.6, 3.1, 1.5, 0.2, 0 },
        new double[] { 6.9, 3.1, 4.9, 1.5, 1 },
         new double[] { 5.5, 2.3, 4.0, 1.3, 1 },
         new double[] { 6.5, 2.8, 4.6, 1.5, 1 },
         new double[] { 5.0, 3.4, 1.5, 0.2, 0 },
        new double[] { 4.4, 2.9, 1.4, 0.2, 0 },
        new double[] { 4.9, 3.1, 1.5, 0.1, 0 },
        new double[] { 5.4, 3.7, 1.5, 0.2, 0 },
        new double[] { 4.8, 3.4, 1.6, 0.2, 0 },
        new double[] { 4.8, 3.0, 1.4, 0.1, 0 },
        new double[] { 4.3, 3.0, 1.1, 0.1, 0 },
        new double[] { 6.5, 3.0, 5.8, 2.2, 2 },
        new double[] { 7.6, 3.0, 6.6, 2.1, 2 },
        new double[] { 4.9, 2.5, 4.5, 1.7, 2 },
        new double[] { 7.3, 2.9, 6.3, 1.8, 2 },
        new double[] { 5.7, 3.8, 1.7, 0.3, 0 },
        new double[] { 5.1, 3.8, 1.5, 0.3, 0 },

    };

            if (type == 2) //randomize data set
            {
                var data = mccData.ToList<double[]>();
                var result1 = data.OrderBy(item => rnd.Next());
                var result = result1.OrderBy(item => rnd.Next());
                return result.ToArray();
            }
            else
                return regData;
        }

        static Parameters generateParameters(int type)
        {
            int constCount = 3;

            var param = new Parameters();
            param.Constants = generateRandomConstants(-1, 1, constCount);
            param.CrossoverProbability = 0.95f;
            param.MutationProbability = 0.1f;
            param.ReproductionProbability = 0.20f;
            param.Elitism = 1;

            if (type == 1)//for Regression
                param.FitnessFunction = RMSE_Fitness;
            else//for classification
                param.FitnessFunction = accracy_Fitness;

            param.InitializationMethod = 1;
            param.MaxInitLevel = 5;
            param.MaxOperationLevel = 8;
            param.PopulationSize = 100;

            return param;
        }

        static double[] generateRandomConstants(double from, double to, int constCount)
        {
            if (constCount == 0)
                return null;
            else
            {
                var consts = new double[constCount];
                for (int i = 0; i < constCount; i++)
                {
                    consts[i] = from + to * Node.Rand.NextDouble();
                }
                return consts;
            }
        }

        static float RMSE_Fitness(Node ch, Parameters param)
        {
            //get data set
            var data = currentDataSet;
            var colCout = data[0].Length - 1;/*output column is last*/;

            //helper fitness vars
            double rowFitness = 0.0;

            //iteration through all rows
            for (int i = 0; i < data.Length; i++)
            {
                var inputRow = new List<double>();
                inputRow.AddRange(data[i]);
                //ad constance before last element (output value)
                inputRow.InsertRange(colCout, param.Constants);

                //calculate output
                double yGp = ch.Evaluate(inputRow.ToArray());

                //last element is output value
                double y = inputRow.Last();

                //Calculate square error
                var ae = y - yGp;
                rowFitness += (ae * ae);
            }

            //root means square error is transformed into adjusted fitness value
            var fitness = ((1.0 / (1.0 + Math.Sqrt(rowFitness / data.Length))));
            return (float)Math.Round(fitness, 5);
        }

        static float accracy_Fitness(Node ch, Parameters param)
        {
            //get data set
            var data = currentDataSet;
            var colCout = data[0].Length - 1;/*output column is last*/;

            //helper fitness vars
            double fitness = 0;
            double rowFitness = 0.0;

            //iteration through all rows
            for (int i = 0; i < data.Length; i++)
            {
                var inputRow = new List<double>();
                inputRow.AddRange(data[i]);

                //add constance before last element (output value)
                inputRow.InsertRange(colCout, param.Constants);

                //calculate output
                double yGp = ch.Evaluate(inputRow.ToArray());
                //perform sigmoid function for three class problems defined 
                var yclass = Math.Truncate(3.0 / (1.0 + Math.Exp(-yGp)));
                //last element is output value
                double y = inputRow.Last();

                //Calculate accuracy
                var result = y - yclass == 0 ? 1 : 0;
                rowFitness += result;
            }

            if (double.IsNaN(rowFitness) || double.IsInfinity(rowFitness))
                fitness = float.NaN;
            else
                fitness = (rowFitness / data.Length);

            return (float)Math.Round(fitness, 4);
        }
        static string[] generateTerminalSet(double[][] tData, double[] rConstants)
        {
            var cCount = 0;
            if (rConstants != null)
                cCount = rConstants.Length;

            //number of input variables
            var inputCount = tData[0].Length - 1;
            var tCount = inputCount + cCount;

            //Terminal set
            var ts = new string[tCount];
            //count input variables
            for (int i = 0; i < inputCount; i++)
            {
                ts[i] = "x" + (i + 1).ToString();
            }
            //count cons
            for (int i = inputCount; i < tCount; i++)
            {
                ts[i] = rConstants[i - inputCount].ToString();
            }
            return ts;
        }
        static Function[] generateFunctionSet()
        {
            var fs = new Function[] {

            //addition
            new Function() { Arity = 2, Id = 0, Name = "+" },
            //subtraction
            new Function() { Arity = 2, Id = 1, Name = "-" },
            //multiplication
            new Function() { Arity = 2, Id = 2, Name = "*" },
            //division
            new Function() { Arity = 2, Id = 3, Name = "/" },
            //1/x
            new Function() { Arity = 2, Id = 4, Name = "1/x" },
            ////sin
            //new Function() { Arity = 1, Id = 5, Name = "sin" },
            ////tan
            //new Function() { Arity = 1, Id = 6, Name = "tan" }
            };
            return fs;
        }

        static void printData(double[][] data, int numRows, int decimals, bool newLine)
        {
            for (int i = 0; i < numRows; ++i)
            {
                Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < data[i].Length; ++j)
                {
                    if (data[i][j] >= 0.0) Console.Write(" "); else Console.Write("-");
                    Console.Write(Math.Abs(data[i][j]).ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
            }
            if (newLine == true) Console.WriteLine("");
        }
        static (double[] obsrv, double[] predc) calculateOutput(Node ch, Parameters param)
        {
            //get data set
            var data = currentDataSet;
            var colCout = data[0].Length - 1;/*output column is last*/;
            //
            double[] obsrv = new double[data.Length];
            double[] predc = new double[data.Length];

            //iteration through all rows
            for (int i = 0; i < data.Length; i++)
            {
                var inputRow = new List<double>();
                inputRow.AddRange(data[i]);
                //add constance before last element (output value)
                inputRow.InsertRange(colCout, param.Constants);

                //calculate output
                double result = ch.Evaluate(inputRow.ToArray());
                //last element is output value
                double y = inputRow.Last();
                obsrv[i] = y;
                predc[i] = result;
            }
            return (obsrv, predc);
        }
        static double calculateR(double[] obsData, double[] preData)
        {
            //calculate average for each dataset
            double aav = obsData.Sum() / obsData.Length;
            double bav = preData.Sum() / obsData.Length;

            double corr = 0;
            double ab = 0, aa = 0, bb = 0;
            for (int i = 0; i < obsData.Length; i++)
            {
                var a = obsData[i] - aav;
                var b = preData[i] - bav;

                ab += a * b;
                aa += a * a;
                bb += b * b;
            }
            corr = ab / Math.Sqrt(aa * bb);
            return corr;
        }

    }
}