using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests
{
    /// <summary>
    /// Represents a NN that is genetically optimized (NEAT).
    /// </summary>
    public class NEAT
    {
        /// <summary>
        /// The maximum number of nodes allowed.
        /// </summary>
        public const int MAX_NODES = 1048576;


        #region C_ Constants

        /// <summary>
        /// The C1 constant for Genome distancing. See the <see cref="NEAT_Tests.Gene_.Genome.Distance(Genome)"/> 
        /// remarks for more information.
        /// </summary>
        public const int C1 = 1;

        /// <summary>
        /// The C2 constant for Genome distancing. See the <see cref="NEAT_Tests.Gene_.Genome.Distance(Genome)"/> 
        /// remarks for more information.
        /// </summary>
        public const int C2 = 1;

        /// <summary>
        /// The C3 constant for Genome distancing. See the <see cref="NEAT_Tests.Gene_.Genome.Distance(Genome)"/> 
        /// remarks for more information.
        /// </summary>
        public const int C3 = 1;

        #endregion C_ Constants


        #region Mutations

        /// <summary>
        /// The strength to adjust the weight during <see cref="NEAT_Tests.Gene_.Genome.Mutate_WeightShift"/>.
        /// </summary>
        public const double WEIGHT_SHIFT = 0.3;

        /// <summary>
        /// The value to be the min/max [-value, value) for the 
        /// <see cref="NEAT_Tests.Gene_.Genome.Mutate_WeightRandom"/> and 
        /// <see cref="NEAT_Tests.Gene_.Genome.Mutate_Link"/>.
        /// </summary>
        public const double WEIGHT_RANDOM = 1;


        /// <summary>
        /// The number of times to attempt to mutate a new link.
        /// </summary>
        public const int LINK_ATTEMPTS = 100;


        #region Probabilities

        /// <summary>
        /// The probability that a link mutation will occur.
        /// </summary>
        public const double PROBABILITY_MUTATE_LINK = 0.4;

        /// <summary>
        /// The probability that a node mutation will occur.
        /// </summary>
        public const double PROBABILITY_MUTATE_NODE = 0.4;

        /// <summary>
        /// The probability that a weight shift mutation will occur.
        /// </summary>
        public const double PROBABILITY_MUTATE_WEIGHTSHIFT = 0.4;

        /// <summary>
        /// The probability that a weight random mutation will occur.
        /// </summary>
        public const double PROBABILITY_MUTATE_WEIGHTRANDOM = 0.4;

        /// <summary>
        /// The probability that a toggle mutation will occur.
        /// </summary>
        public const double PROBABILITY_MUTATE_TOGGLE = 0.4;

        #endregion Probabilities

        #endregion Mutations


        /// <summary>
        /// The maximum distance a client can be from its species representative client.
        /// </summary>
        public const double MAXIMUM_RACIAL_DIFFERENCE = 4;


        /// <summary>
        /// The percentage of clients that will survive to the next generation.
        /// </summary>
        public const double SURVIVAL_PERCENTAGE = 0.8;


        private Dictionary<ConnectionGene, ConnectionGene> all_connections;
        private List<NodeGene> all_nodes;

        private int num_clients;

        private int num_inputs, num_outputs;

        private Random random = new Random();


        public RandomHashSet<Client> clients = new RandomHashSet<Client>(); //I literally cannot be bothered to make these properties right now.
        public RandomHashSet<Species> species = new RandomHashSet<Species>();


        /// <summary>
        /// Constructs a NEAT with the given number of inputs and outputs. Will evolve the given number of genomes.
        /// </summary>
        /// <param name="num_inputs">The number of input nodes in the NEAT.</param>
        /// <param name="num_outputs">The number of output nodes in the NEAT.</param>
        /// <param name="num_clients">The number of clients to evolve.</param>
        public NEAT(int num_inputs, int num_outputs, int num_clients)
        {
#pragma warning disable 
            Reset(num_inputs, num_outputs, num_clients);
#pragma warning enable
        }


        /// <summary>
        /// Resets this NEAT to be used again. Can't we just make a new object?
        /// </summary>
        /// <param name="num_inputs">The number of input nodes in the NEAT.</param>
        /// <param name="num_outputs">The number of output nodes in the NEAT.</param>
        /// <param name="num_clients">The number of clients to evolve.</param>
        [Obsolete]
        public void Reset(int num_inputs, int num_outputs, int num_clients)
        {
            this.num_inputs = num_inputs;
            this.num_outputs = num_outputs;

            this.num_clients = num_clients;


            all_connections = new Dictionary<ConnectionGene, ConnectionGene>();
            all_nodes = new List<NodeGene>();

            all_connections.Clear();
            all_nodes.Clear();

            clients.Clear();
            species.Clear();    //TODO Might need to remove


            //Initialize input nodes:
            for (int i = 0; i < num_inputs; ++i)
            {
                NodeGene nodeGene = CreateNode();

                nodeGene.X = 0.1;   //All input nodes are at 0.1.

                nodeGene.Y = (i + 1.0) / (num_inputs + 1.0);
            }


            //Initialize output nodes:
            for (int i = 0; i < num_outputs; ++i)
            {
                NodeGene nodeGene = CreateNode();

                nodeGene.X = 0.9;   //All output nodes are at 0.9.

                nodeGene.Y = (i + 1.0) / (num_outputs + 1.0);
            }


            for (int i = 0; i < this.num_clients; ++i)
            {
                Client client = new Client();

                client.Geneome = CreateGenome();

                clients.Add(client);
            }
        }


        /// <summary>
        /// Gets the client at the given index.
        /// </summary>
        /// <param name="index">The index of the client.</param>
        /// <returns>The client at that index.</returns>
        public Client GetClient(int index)
        {
            return clients[index];
        }


        #region Evolution

        /// <summary>
        /// Evolves this generation of clients.
        /// </summary>
        public void Evolve()
        {
            GenerateSpecies();

            Kill();

            RemoveExtictSpecies();

            Reproduce();

            Mutate();


            for (int i = 0; i < clients.Size; ++i)
            {
                clients[i].Generate_Calculator();
            }
        }


        /// <summary>
        /// Generates/appends species for all of the stored clients that don't have one.
        /// </summary>
        private void GenerateSpecies()
        {
            for (int i = 0; i < species.Size; ++i)
            {
                species[i].Reset();
            }


            for (int i = 0; i < clients.Size; ++i)
            {
                if (clients[i].Species != null)
                {
                    continue;
                }


                bool found = false;

                for (int q = 0; q < species.Size; ++q)
                {
                    if (species[q].Put(clients[i]))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    species.Add(new Species(clients[i], random));
                }
            }


            for (int i = 0; i < species.Size; ++i)
            {
                species[i].EvaluateScore();
            }
        }


        /// <summary>
        /// Kills 1 - <see cref="NEAT_Tests.NEAT.SURVIVAL_PERCENTAGE"/> the clients in each species.
        /// </summary>
        private void Kill()
        {
            for (int i = 0; i < species.Size; ++i)
            {
                species[i].Prune(1 - SURVIVAL_PERCENTAGE);
            }
        }


        /// <summary>
        /// Removes any species that should be considered extinct.
        /// </summary>
        private void RemoveExtictSpecies()
        {
            for (int i = species.Size - 1; i >= 0; --i)
            {
                if (species[i].Size <= 1)
                {
                    species[i].GoExtict();

                    species.Remove(i);
                }
            }
        }


        /// <summary>
        /// Reproduces clients that don't have a set species(?) isn't that kinda against the idea?
        /// </summary>
        private void Reproduce()
        {
            Stonis.ScoredDistribution<Species> selector = new Stonis.ScoredDistribution<Species>(species.Size, random);

            for (int i = 0; i < species.Size; ++i)
            {
                selector.Add(species[i], species[i].Score);
            }


            for (int i = 0; i < clients.Size; ++i)
            {
                if (clients[i].Species == null)
                {
                    Species random_species = selector.ChooseValue();

                    if (random_species == null)
                    {
                        random_species = selector.ChooseValue();
                    }

                    clients[i].Geneome = random_species.Breed();
                    random_species.ForcePut(clients[i]);
                }
            }
        }


        /// <summary>
        /// Possibly mutates every client in the genome.
        /// </summary>
        private void Mutate()
        {
            for (int i = 0; i < clients.Size; ++i)
            {
                clients[i].Geneome.Mutate();
            }
        }

        #endregion Evolution


        #region Genome

        /// <summary>
        /// Creates a new genome populated with only the input and output nodes.
        /// </summary>
        /// <returns>The created genome.</returns>
        public Genome CreateGenome()
        {
            Genome genome = new Genome(this);

            for (int i = 0; i < num_inputs + num_outputs; ++i)  //Outputs are right after inputs; this is fine.
            {
                genome.Nodes.Add(GetNodeGene(i + 1));
            }

            return genome;
        }

        #endregion Genome


        #region ConnectionGene

        /// <summary>
        ///  Creates a new ConnectionGene to track and adds it to the tracker. 
        /// </summary>
        /// <param name="from">The NodeGene to connect from.</param>
        /// <param name="to">The NodeGene to connect to.</param>
        /// <returns>The created ConnectionGene.</returns>
        public ConnectionGene CreateConnection(NodeGene from, NodeGene to)
        {
            ConnectionGene connectionGene = new ConnectionGene(from, to, 0);    //Temp innovation number.

            if (all_connections.ContainsKey(connectionGene))
            {
                connectionGene.InnovationNumber = all_connections[connectionGene].InnovationNumber;
            }
            else
            {
                connectionGene.InnovationNumber = all_connections.Count + 1;

                all_connections.Add(connectionGene, connectionGene);
            }

            return connectionGene;
        }


        /// <summary>
        /// Copys the given ConnectionGene to a new object.
        /// </summary>
        /// <param name="connectionGene">The ConnectionGene to copy.</param>
        /// <returns>The copied ConnectionGene.</returns>
        public static ConnectionGene CopyConnectionGene(ConnectionGene connectionGene)
        {
            return new ConnectionGene(connectionGene.From, connectionGene.To, connectionGene.InnovationNumber)
            {
                Weight = connectionGene.Weight,

                Enabled = connectionGene.Enabled
            };
        }

        #endregion ConnectionGene


        #region NodeGene

        /// <summary>
        /// Creates a new NodeGene to track and adds it to the tracker.
        /// </summary>
        /// <returns>The created NodeGene.</returns>
        public NodeGene CreateNode()
        {
            NodeGene nodeGene = new NodeGene(all_nodes.Count + 1);  //0 is wasted, eh
            all_nodes.Add(nodeGene);

            return nodeGene;
        }


        /// <summary>
        /// Gets the tracked node with the given innovation number.
        /// </summary>
        /// <param name="innovation_number">The innovation number of the node to get.</param>
        /// <returns>The node if found, null otherwise.</returns>
        public NodeGene GetNodeGene(int innovation_number)
        {
            if (innovation_number <= 0 || innovation_number > all_nodes.Count)
            {
                return null;
            }

            return all_nodes[innovation_number - 1];    //The innovation numbers are 1-indexed, but Lists are not!
        }

        #endregion NodeGene
    }
}
