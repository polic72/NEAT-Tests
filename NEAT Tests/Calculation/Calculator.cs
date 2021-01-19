using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests.Calculation
{
    /// <summary>
    /// A calculator for a genome. This does the actual calculations for the NN.
    /// </summary>
    public class Calculator
    {
        private List<Node> input_nodes = new List<Node>();
        private List<Node> output_nodes = new List<Node>();
        private List<Node> hidden_nodes = new List<Node>();


        /// <summary>
        /// Constructs a new calculator for the given genome.
        /// </summary>
        /// <param name="genome">The genome to create a calculator for.</param>
        public Calculator(Genome genome)
        {
            RandomHashSet<NodeGene> nodes = genome.Nodes;
            RandomHashSet<ConnectionGene> connections = genome.Connections;

            Dictionary<int, Node> nodeDictionary = new Dictionary<int, Node>();

            for (int i = 0; i < nodes.Size; ++i)
            {
                Node node = new Node(nodes[i].X);

                nodeDictionary.Add(nodes[i].InnovationNumber, node);

                if (node.X <= 0.1)
                {
                    input_nodes.Add(node);
                }
                else if (node.X >= 0.9)
                {
                    output_nodes.Add(node);
                }
                else
                {
                    hidden_nodes.Add(node);
                }
            }


            hidden_nodes.Sort();    //Uses the comparer we set.


            for (int i = 0; i < connections.Size; ++i)
            {
                Node from = nodeDictionary[connections[i].From.InnovationNumber];   //new Node(connections[i].From.InnovationNumber);
                Node to = nodeDictionary[connections[i].To.InnovationNumber]; //new Node(connections[i].To.InnovationNumber);

                //Node from = new Node(connections[i].From.InnovationNumber);
                //Node to = new Node(connections[i].To.InnovationNumber);

                Connection connection = new Connection(from, to);
                connection.Weight = connections[i].Weight;
                connection.Enabled = connections[i].Enabled;

                to.Connections.Add(connection);
            }
        }


        /// <summary>
        /// Runs the feed-forward method of the NN and returns the output.
        /// </summary>
        /// <param name="input">The values for the input nodes of the NN.</param>
        /// <returns>The output of the NN.</returns>
        public double[] Calculate(params double[] input)
        {
            if (input.Length != input_nodes.Count)
            {
                throw new ArgumentException("The input must be the same length as the input nodes of the genome.", "input");
            }


            for (int i = 0; i < input_nodes.Count; ++i)
            {
                input_nodes[i].Output = input[i];
            }


            foreach (Node node in hidden_nodes)
            {
                node.Calculate();
            }


            foreach(Node node in output_nodes)
            {
                node.Calculate();
            }


            return output_nodes.Select(x => x.Output).ToArray();
        }
    }
}
