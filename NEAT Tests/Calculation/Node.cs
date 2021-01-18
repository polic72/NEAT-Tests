using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests.Calculation
{
    public class Node : IComparable<Node>
    {
        /// <summary>
        /// The X value of this node.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The output of this node.
        /// </summary>
        public double Output { get; set; }

        
        /// <summary>
        /// The connections of this node.
        /// </summary>
        public List<Connection> Connections { get; set; }


        /// <summary>
        /// Constructs a node with the given X value.
        /// </summary>
        /// <param name="x">The X value of this node.</param>
        public Node(double x)
        {
            X = x;

            Connections = new List<Connection>();
        }


        /// <summary>
        /// Calculates the output of this node.
        /// </summary>
        public void Calculate()
        {
            double sum = 0;

            foreach (Connection connection in Connections)
            {
                if (connection.Enabled)
                {
                    sum += connection.From.Output;
                }
            }

            Output = Activation_Sigmoid(sum);
        }


        /// <summary>
        /// Calculates the sigmoid of x.
        /// </summary>
        /// <param name="x">The x to get the sigmoid of.</param>
        /// <returns>The sigmoid of x.</returns>
        public static double Activation_Sigmoid(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
        }


        /// <summary>
        /// Compares this Node to the given Node.
        /// </summary>
        /// <param name="other">The other node to compare this node to.</param>
        /// <returns>-1 if this node's X is greater than the given, 1 if lesser than, 0 otherwise.</returns>
        public int CompareTo(Node other)
        {
            if (X > other.X)
            {
                return -1;
            }
            if (X < other.X)
            {
                return 1;
            }

            return 0;
        }
    }
}
