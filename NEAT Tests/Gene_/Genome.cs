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
    /// <remarks>
    /// When comparing 2 genomes, there are a few things to think about:
    /// <para/>
    /// Similar: Two genes are similar when they share the same innovation number and are at the same spot.
    /// <para/>
    /// Disjoint: A gene is disjointed if its innovation number is less that of the neighbor in the same spot.
    /// <para/>
    /// Excess: A gene is excess if it doesn't have a neighbor.
    /// </remarks>
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
        /// Gets the distance between this Genome and the given Genome. Higher = less compatible.
        /// </summary>
        /// <param name="genome">The Genome to compare to.</param>
        /// <returns>The distance between this Genome and the given Genome.</returns>
        /// <remarks>
        /// The distance d can be measured by the following equation:
        /// <para/>
        /// d = c1(E / N) + c2(D / N) + c3 * W
        /// <para/>
        /// Where:
        ///     d = distance
        ///     E = # excess genes
        ///     D = # of disjoint genes
        ///     W = weight difference of similar genes
        ///     N = # of genes in largest genome (this or them), 1 if #genes &lt; 20
        ///     c_ = constant for adjusting
        /// </remarks>
        public double Distance(Genome genome)
        {
            if (Connections[Connections.Size - 1].InnovationNumber
                < genome.Connections[genome.Connections.Size - 1].InnovationNumber)
            {
                return genome.Distance(this);   //Handles excess properly.
            }


            int index_me = 0;
            int index_them = 0;


            int num_excess = 0;     //The number of excess genes.
            int num_disjoint = 0;   //The number of disjoint genes.

            double weight_diff = 0; //The weight difference between similar genes.
            double num_similar = 0; //The number of genes that are similar.


            while (index_me < Connections.Size && index_them < genome.Connections.Size)
            {
                ConnectionGene connectionGene_me = Connections[index_me];
                ConnectionGene connectionGene_them = genome.Connections[index_them];

                int inNum_me = connectionGene_me.InnovationNumber;
                int inNum_them = connectionGene_them.InnovationNumber;


                if (inNum_me == inNum_them) //Similar genes.
                {
                    index_me++;
                    index_them++;

                    num_similar++;
                    weight_diff += Math.Abs(connectionGene_me.Weight - connectionGene_them.Weight);
                }
                else if (inNum_me > inNum_them) //Disjoint gene at them, increase them.
                {
                    index_them++;

                    num_disjoint++;
                }
                else    //Disjoint gene at me, increase me.
                {
                    index_me++;

                    num_disjoint++;
                }
            }


            //This is why this genome must have the higher innovation number.
            num_excess = Connections.Size - index_me;

            weight_diff /= num_similar;


            double N = Math.Max(Connections.Size, genome.Connections.Size);
            N = (N < 20) ? 1 : N;


            return NEAT.C1 * (num_excess / N) + NEAT.C2 * (num_disjoint / N) + (NEAT.C3 * weight_diff);
        }


        /// <summary>
        /// Crosses over the two given Genomes.
        /// </summary>
        /// <param name="genome_1">The first Genome.</param>
        /// <param name="genome_2">The second Genome.</param>
        /// <param name="random">The random object to use when deciding genes.</param>
        /// <returns>The crossed-over Genome.</returns>
        public static Genome CrossOver(Genome genome_1, Genome genome_2, Random random)
        {
            if (genome_1.Connections[genome_1.Connections.Size - 1].InnovationNumber
                < genome_2.Connections[genome_2.Connections.Size - 1].InnovationNumber)
            {
                return CrossOver(genome_2, genome_1, random);   //Handles excess properly.
            }


            Genome created_genome = genome_1.NEAT.CreateGenome();


            #region Connections

            int index_me = 0;
            int index_them = 0;

            while (index_me < genome_1.Connections.Size && index_them < genome_2.Connections.Size)
            {
                ConnectionGene connectionGene_me = genome_1.Connections[index_me];
                ConnectionGene connectionGene_them = genome_2.Connections[index_them];

                int inNum_me = connectionGene_me.InnovationNumber;
                int inNum_them = connectionGene_them.InnovationNumber;


                if (inNum_me == inNum_them) //Similar genes, choose either side at random.
                {
                    index_me++;
                    index_them++;

                    if (random.NextDouble() < .5)
                    {
                        created_genome.Connections.Add(NEAT.CopyConnectionGene(connectionGene_me));
                    }
                    else
                    {
                        created_genome.Connections.Add(NEAT.CopyConnectionGene(connectionGene_them));
                    }
                }
                else if (inNum_me > inNum_them) //Disjoint gene at them, increase them.
                {
                    index_them++;

                    //This is hotly debated, but will still work without this.
                    //created_genome.Connections.Add(NEAT.CopyConnectionGene(connectionGene_them));
                }
                else    //Disjoint gene at me, increase me.
                {
                    index_me++;

                    created_genome.Connections.Add(NEAT.CopyConnectionGene(connectionGene_me));
                }
            }


            //This is why this genome must have the higher innovation number.
            while (index_me < genome_1.Connections.Size)    //Run through the excess connections and add them all.
            {
                created_genome.Connections.Add(NEAT.CopyConnectionGene(genome_1.Connections[index_me++]));
            }

            #endregion Connections


            #region Nodes

            //foreach (ConnectionGene connectionGene in created_genome.Connections)
            //TODO implement foreach functionality into RandomHashSet

            for (int i = 0; i < created_genome.Connections.Size; ++i)
            {
                ConnectionGene connectionGene = created_genome.Connections[i];

                created_genome.Nodes.Add(connectionGene.From);  //Finally making use of the uniqness in RandomHashSet.
                created_genome.Nodes.Add(connectionGene.To);
            }

            #endregion Nodes


            return created_genome;
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
