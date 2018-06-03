using System;
namespace GPImplemenation
{
    public class Parameters
    {
        //chromosome related parameters
        public float CrossoverProbability { get; set; }
        public float MutationProbability { get; set; }
        public float ReproductionProbability { get; set; }
        public int MaxOperationLevel { get; set; }
        public int MaxInitLevel { get; set; }

        //population related parameters
        public int PopulationSize { get; set; }
        public int Elitism { get; set; }
        public int SelectionMethod { get; set; }
        public int InitializationMethod { get; set; }
        public Func<Node, Parameters, float> FitnessFunction { get; set; }

        //random constance collection
        public double[] Constants { get; set; }
    }
}
