using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPImplemenation
{
    public class Population
    {
        //current population 
        internal List<Chromosome> Chromosomes { get; set; }
        //offspring population
        internal List<Chromosome> m_Offspring;
        //default constructor
        public Population() => Chromosomes = new List<Chromosome>();
        //Operations
        internal void Initialize(Parameters param, Function[] funSet, string[] terSet)
        {
            if (Chromosomes != null)
                Chromosomes.Clear();
            else
                Chromosomes = new List<Chromosome>();

            m_Offspring = new List<Chromosome>();

            GenerateChromosomes(param.PopulationSize, param, funSet, terSet);
        }

        private void GenerateChromosomes(int size, Parameters param, Function[] funSet, string[] terSet)
        {
            //Chromosome generation 
            for (int i = 0; i < size; i++)
            {
                //Randomly choose which level chromosome will have
                int levels = Node.Rand.Next(2, param.MaxInitLevel + 1);

                // generate new chromosome
                var ch = new Chromosome();
                ch.Generate(levels, funSet, terSet);

                // add chromosome in to population
                Chromosomes.Add(ch);
            }
        }

        internal void Crossover(Parameters param, Function[] funSet, string[] terSet)
        {

            //if crossover is defined
            if (param.CrossoverProbability == 0)
                return;
            for (int i = 1; i < param.PopulationSize; i += 2)
            {

                if (Node.Rand.NextDouble() <= param.CrossoverProbability)
                {
                    var p1 = SelectChromosome(Chromosomes, param);
                    var p2 = SelectChromosome(Chromosomes, param);

                    // cloning the chromosome and prepare for crossover
                    var ch1 = p1.Clone();
                    var ch2 = p2.Clone();

                    // crossover
                    ch1.Crossover(ch2, param.MaxOperationLevel, funSet, terSet);

                    //reset fitness
                    ch1.Fitness = float.MinValue;
                    ch2.Fitness = float.MinValue;

                    //add new chromosomes to chromosomes
                    m_Offspring.Add(ch1);
                    m_Offspring.Add(ch2);

                    if (ch1.Node.Level > param.MaxOperationLevel)
                        throw new Exception("Max level exceeded!");
                    if (ch1.Node.Level > param.MaxOperationLevel)
                        throw new Exception("Max level exceeded!");
                }
            }
        }

        internal void Mutate(Parameters param, Function[] funSet, string[] terSet)
        {
            //if mutation is defined
            if (param.MutationProbability == 0)
                return;
            for (int i = 0; i < param.PopulationSize; i++)
            {
                // 
                if (Node.Rand.NextDouble() <= param.MutationProbability)
                {
                    var p = SelectChromosome(Chromosomes, param);
                    var ch = p.Clone();
                    ch.Mutate(param.MaxOperationLevel, funSet, terSet);
                    ch.Fitness = float.MinValue;
                    m_Offspring.Add(ch);

                }
            }
        }

        internal void EvaluatePopulation(Parameters param)
        {
            //perform evaluation on all chromosomes
            EvaluateParallel(Chromosomes, param);
            EvaluateParallel(m_Offspring, param);
        }

        private void Evaluate(List<Chromosome> chs, Parameters param)
        {
            if (chs == null || chs.Count == 0)
                return;

            for (int i = 0; i < chs.Count; i++)
            {
                if (chs[i].Fitness == float.MinValue)
                {
                    chs[i].Fitness = param.FitnessFunction(chs[i].Node, param);
                }
            }
        }

        private void EvaluateParallel(List<Chromosome> chs, Parameters param)
        {
            if (chs == null || chs.Count == 0)
                return;

            Parallel.For(0, chs.Count, (i) =>
            {
                if (chs[i].Fitness == float.MinValue)
                    chs[i].Fitness = param.FitnessFunction(chs[i].Node, param);
            });
        }

        internal void Reproduction(Parameters param, Function[] funSet, string[] terSet)
        {
            // probability of reproduction
            int repNumber = (int)(param.ReproductionProbability * param.PopulationSize) - param.Elitism >= 0 ? param.Elitism : 0;
            var chs = Chromosomes;

            //Elitism number of very best chromosome to survive to new generation
            if (param.Elitism > 0)
            {
                int i = param.Elitism;
                foreach (var c in chs.OrderByDescending(x => x.Fitness))
                {
                    if (i == 0)
                        break;
                    m_Offspring.Add(c.Clone());
                    i--;
                }
            }

            //reproduce repNumber of chromosomes in to new population
            for (int j = 0; j < repNumber; j++)
            {
                Chromosome c = SelectChromosome(chs, param);
                m_Offspring.Add(c);
            }
        }

        private Chromosome SelectChromosome(List<Chromosome> ch, Parameters par)
        {
            if (ch == null || par == null || ch.Count == 0)
                throw new Exception("Population cannot be empty.");

            //parameters of the selection
            double sumOfFitness = 0;

            //calculate sum of fitness
            for (int i = 0; i < ch.Count; i++)
            {
                //check for invalid fitness
                if (double.IsNaN(ch[i].Fitness) || double.IsInfinity(ch[i].Fitness))
                    sumOfFitness += 0;
                else
                    sumOfFitness += ch[i].Fitness;
            }

            //generate random selection point
            var selPoint = Node.Rand.NextDouble() * sumOfFitness;

            //enumerate population from the best to the worst
            double currentScore = 0;
            foreach (var c in ch.OrderByDescending(x => x.Fitness))
            {
                if (selPoint >= currentScore && selPoint <= currentScore + c.Fitness)
                    return c.Clone() as Chromosome;
                currentScore += c.Fitness;
            }

            //this should not happen
            throw new Exception("Fitness proportionate selection exception.");
        }

        internal void CreateNewGeneration(Parameters par, Function[] funSet, string[] terSet)
        {
            //
            Chromosomes.Clear();
            Chromosomes.AddRange(m_Offspring.OrderByDescending(x => x.Fitness).GroupBy(x => x.Fitness).Select(grp => grp.First()).Take(par.PopulationSize));
            m_Offspring.Clear();
            //if the population has less chromosomes than defined generate new one
            int size = par.PopulationSize - Chromosomes.Count;
            if (size > 0)
            {
                GenerateChromosomes(size, par, funSet, terSet);
                EvaluatePopulation(par);
            }
        }
    }
}
