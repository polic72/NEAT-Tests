using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT_Tests.Gene_
{
    /// <summary>
    /// A gene specifying a synapse in the NN.
    /// </summary>
    public class ConnectionGene : Gene
    {
        #region Properties

        /// <summary>
        /// The left-hand node of this connection.
        /// </summary>
        public NodeGene From { get; set; }

        /// <summary>
        /// The right-hand node of this connection.
        /// </summary>
        public NodeGene To { get; set; }


        /// <summary>
        /// The weight of this connection.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Whether or not this connection is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        #endregion Properties


        /// <summary>
        /// Constructs a connection gene with the given innovation number.
        /// </summary>
        /// <param name="from">The NodeGene to connect from.</param>
        /// <param name="to">The NodeGene to connect to.</param>
        /// <param name="innovation_number">The innovation number.</param>
        public ConnectionGene(NodeGene from, NodeGene to, int innovation_number) :
            base(innovation_number)
        {
            From = from;
            To = to;

            Enabled = true;
        }


        /// <summary>
        /// Whether or not this ConnectionGene is equal to the given object.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>True if obj is a ConnectionGene object and From/To match up. False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ConnectionGene connectionGene)
            {
                if (From == connectionGene.From && To == connectionGene.To)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Gets the hash code of this ConnectionGene. Based of the innovation numbers of the From/To NodeGenes.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return From.InnovationNumber * NEAT.MAX_NODES + To.InnovationNumber;
        }
    }
}
