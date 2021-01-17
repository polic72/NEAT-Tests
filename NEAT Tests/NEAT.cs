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


        private Dictionary<ConnectionGene, ConnectionGene> all_connections;
        private List<NodeGene> all_nodes;

        private int num_genomes;

        private int num_inputs, num_outputs;


        /// <summary>
        /// Constructs a NEAT with the given number of inputs and outputs. Will evolve the given number of genomes.
        /// </summary>
        /// <param name="num_inputs">The number of input nodes in the NEAT.</param>
        /// <param name="num_outputs">The number of output nodes in the NEAT.</param>
        /// <param name="max_genomes">The number of genomes to evolve.</param>
        public NEAT(int num_inputs, int num_outputs, int num_genomes)
        {
#pragma warning disable 
            Reset(num_inputs, num_outputs, num_genomes);
#pragma warning enable
        }


        /// <summary>
        /// Resets this NEAT to be used again. Can't we just make a new object?
        /// </summary>
        /// <param name="num_inputs">The number of input nodes in the NEAT.</param>
        /// <param name="num_outputs">The number of output nodes in the NEAT.</param>
        /// <param name="max_genomes">The number of genomes to evolve.</param>
        [Obsolete]
        public void Reset(int num_inputs, int num_outputs, int num_genomes)
        {
            this.num_inputs = num_inputs;
            this.num_outputs = num_outputs;

            this.num_genomes = num_genomes;


            all_connections = new Dictionary<ConnectionGene, ConnectionGene>();
            all_nodes = new List<NodeGene>();

            all_connections.Clear();
            all_nodes.Clear();


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
        }


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
