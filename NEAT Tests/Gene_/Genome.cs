using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Calculation;

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
        #region Properties

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

        #endregion Properties


        private Random random;

        private Calculator calculator;


        /// <summary>
        /// Constructs a Genome with the given NEAT.
        /// </summary>
        /// <param name="neat">The NEAT this Genome is a part of.</param>
        public Genome(NEAT neat)
        {
            Connections = new RandomHashSet<ConnectionGene>();
            Nodes = new RandomHashSet<NodeGene>();

            NEAT = neat;

            random = new Random();
        }



        /// <summary>
        /// Constructs a Genome with the given NEAT.
        /// </summary>
        /// <param name="neat">The NEAT this Genome is a part of.</param>
        /// <param name="capacity">The capacity of the Genome.</param>
        public Genome(NEAT neat, int capacity, Random random)
        {
            Connections = new RandomHashSet<ConnectionGene>(capacity);
            Nodes = new RandomHashSet<NodeGene>(capacity);

            NEAT = neat;

            this.random = random;
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


        #region Mutate

        /// <summary>
        /// Mutates this Genome.
        /// </summary>
        public void Mutate()
        {
            if (NEAT.PROBABILITY_MUTATE_LINK > random.NextDouble())
            {
                Mutate_Link();
            }

            if (NEAT.PROBABILITY_MUTATE_NODE > random.NextDouble())
            {
                Mutate_Node();
            }

            if (NEAT.PROBABILITY_MUTATE_WEIGHTSHIFT > random.NextDouble())
            {
                Mutate_WeightShift();
            }

            if (NEAT.PROBABILITY_MUTATE_WEIGHTRANDOM > random.NextDouble())
            {
                Mutate_WeightRandom();
            }

            if (NEAT.PROBABILITY_MUTATE_TOGGLE > random.NextDouble())
            {
                Mutate_LinkToggle();
            }
        }


        /// <summary>
        /// Mutates the genome by creating a new connection. 
        /// </summary>
        public void Mutate_Link()
        {
            for (int i = 0; i < NEAT.LINK_ATTEMPTS; ++i)
            {
                NodeGene nodeGene_a = Nodes.GetRandomElement();
                NodeGene nodeGene_b = Nodes.GetRandomElement();

                if (nodeGene_a.X == nodeGene_b.X)
                {
                    continue;
                }


                ConnectionGene connectionGene;

                if (nodeGene_a.X < nodeGene_b.X)
                {
                    connectionGene = new ConnectionGene(nodeGene_a, nodeGene_b, 0); //Temp innovation number.
                }
                else
                {
                    connectionGene = new ConnectionGene(nodeGene_b, nodeGene_a, 0); //Temp innovation number.
                }

                if (Connections.Contains(connectionGene))
                {
                    continue;
                }


                connectionGene = NEAT.CreateConnection(connectionGene.From, connectionGene.To);
                connectionGene.Weight = NEAT.WEIGHT_RANDOM + (random.NextDouble() * 2 - 1);

                Connections.Add_Sorted_Gene(connectionGene);    //This needs to be sorted otherwise something breaks.

                return;
            }
        }


        /// <summary>
        /// Mutates a random connection splitting it with a node.
        /// </summary>
        public void Mutate_Node()
        {
            ConnectionGene connectionGene = Connections.GetRandomElement();

            if (connectionGene == null)
            {
                return;
            }


            NodeGene from = connectionGene.From;
            NodeGene to = connectionGene.To;

            NodeGene created = NEAT.CreateNode();

            created.X = (from.X + to.X) / 2;
            created.Y = (from.Y + to.Y) / 2 + random.NextDouble() * 0.1 + .05;

            Nodes.Add(created);


            ConnectionGene created_connectionGene_1 = NEAT.CreateConnection(from, created);
            ConnectionGene created_connectionGene_2 = NEAT.CreateConnection(created, to);

            Connections.Remove(connectionGene);

            Connections.Add(created_connectionGene_1);
            Connections.Add(created_connectionGene_2);


            created_connectionGene_1.Weight = 1;                        //Default weight.
            created_connectionGene_2.Weight = connectionGene.Weight;    //Old connection's weight.

            created_connectionGene_2.Enabled = connectionGene.Enabled;  //Old enabled state.
        }


        /// <summary>
        /// Mutates a random connection by shifting its weight up or down by a radom value.
        /// </summary>
        public void Mutate_WeightShift()
        {
            ConnectionGene connectionGene = Connections.GetRandomElement();  //Ayy, finally using this.

            if (connectionGene!= null)
            {
                connectionGene.Weight = connectionGene.Weight + NEAT.WEIGHT_RANDOM + (random.NextDouble() * 2 - 1);
            }
        }


        /// <summary>
        /// Mutates a random connection by radomizing its weight.
        /// </summary>
        public void Mutate_WeightRandom()
        {
            ConnectionGene connectionGene = Connections.GetRandomElement();  //Ayy, finally using this.

            if (connectionGene != null)
            {
                connectionGene.Weight = NEAT.WEIGHT_RANDOM + (random.NextDouble() * 2 - 1);
            }
        }


        /// <summary>
        /// Mutates a random connection by inverting its current Enabled status.
        /// </summary>
        public void Mutate_LinkToggle()
        {
            ConnectionGene connectionGene = Connections.GetRandomElement();  //Ayy, finally using this.

            if (connectionGene != null)
            {
                connectionGene.Enabled = !connectionGene.Enabled;
            }
        }

        #endregion Mutate


        /// <summary>
        /// Runs the feed-forward method of the NN and returns the output.
        /// </summary>
        /// <param name="input">The values for the input nodes of the NN.</param>
        /// <returns>The output of the NN.</returns>
        public double[] Calculate(params double[] input)
        {
            if (calculator == null)
            {
                calculator = new Calculator(this);
            }

            return calculator.Calculate(input);
        }
    }
}
