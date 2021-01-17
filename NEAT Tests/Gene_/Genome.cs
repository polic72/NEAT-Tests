using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT_Tests.Gene_
{
    /// <summary>
    /// A group of ConnectionGenes and NodeGenes.
    /// </summary>
    public class Genome
    {
        /// <summary>
        /// The ConnectionGenes.
        /// </summary>
        public RandomHashSet<ConnectionGene> Connections { get; private set; }

        /// <summary>
        /// The NodeGenes.
        /// </summary>
        public RandomHashSet<NodeGene> Nodes { get; private set; }


        /// <summary>
        /// The NEAT this Genome is a part of.
        /// </summary>
        public NEAT NEAT { get; private set; }


        /// <summary>
        /// Constructs a Genome with the given NEAT.
        /// </summary>
        /// <param name="neat">The NEAT this Genome is a part of.</param>
        public Genome(NEAT neat)
        {
            Connections = new RandomHashSet<ConnectionGene>();
            Nodes = new RandomHashSet<NodeGene>();

            NEAT = neat;
        }


        /// <summary>
        /// Constructs a Genome with the given NEAT.
        /// </summary>
        /// <param name="neat">The NEAT this Genome is a part of.</param>
        /// <param name="capacity">The capacity of the Genome.</param>
        public Genome(NEAT neat, int capacity)
        {
            Connections = new RandomHashSet<ConnectionGene>(capacity);
            Nodes = new RandomHashSet<NodeGene>(capacity);

            NEAT = neat;
        }


        /// <summary>
        /// Gets the distance between this Genome and the given Genome.
        /// </summary>
        /// <param name="genome">The Genome to compare to.</param>
        /// <returns>The distance between this Genome and the given Genome.</returns>
        public double Distance(Genome genome)
        {
            return 0;   //TODO
        }


        /// <summary>
        /// Crosses over the two given Genomes.
        /// </summary>
        /// <param name="genome_1">The first Genome.</param>
        /// <param name="genome_2">The second Genome.</param>
        /// <returns>The crossed-over Genome.</returns>
        public static Genome CrossOver(Genome genome_1, Genome genome_2)
        {
            return null;    //TODO
        }


        /// <summary>
        /// Mutates this Genome.
        /// </summary>
        public void Mutate()
        {
            //TODO
        }
    }
}
