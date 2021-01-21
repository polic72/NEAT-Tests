using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //NEAT neat = new NEAT(2, 1, 5);

            //Genome genome = neat.CreateGenome();

            //genome.Mutate_Link();
            ////genome.Mutate_Link();

            //double[] wegfg = genome.Calculate(1, 1);

            //genome.Mutate_Node();

            //double[] defwf = genome.Calculate(1, 1);


            NEAT neat = new NEAT(10, 1, 1000);

            Random random = new Random();

            double[] inputs = new double[10];

            for (int i = 0; i < inputs.Length; ++i) { inputs[i] = random.NextDouble(); }


            for (int i = 0; i < 1000; ++i)
            {
                for (int q = 0; q < neat.clients.Size; ++q)
                {
                    double score = neat.clients[i].Calculate(inputs)[0];

                    neat.clients[i].Score = score;
                }

                neat.Evolve();
            }
        }
    }
}
