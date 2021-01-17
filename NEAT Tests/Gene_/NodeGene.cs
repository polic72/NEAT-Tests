using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT_Tests.Gene_
{
    /// <summary>
    /// A gene specifying a nueron in the NN.
    /// </summary>
    public class NodeGene : Gene
    {
        /// <summary>
        /// The X position of the node.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y position of the node.
        /// </summary>
        public double Y { get; set; }


        /// <summary>
        /// Constructs a node gene with the given innovation number.
        /// </summary>
        /// <param name="innovation_number">The innovation number.</param>
        public NodeGene(int innovation_number) :
            base(innovation_number)
        {
            
        }


        /// <summary>
        /// Whether or not this NodeGene is equal to the given object.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>True if obj is a NodeGene object and has the same innovation number. False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NodeGene nodeGene)
            {
                return InnovationNumber == nodeGene.InnovationNumber;
            }

            return false;
        }


        /// <summary>
        /// Gets the hash code of this NodeGene. This is equivalent to the innovation number.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return InnovationNumber;
        }


        /// <summary>
        /// Whether or not the left NodeGene is equal to the right one.
        /// </summary>
        /// <param name="left">The left NodeGene to test.</param>
        /// <param name="right">The right NodeGene to test.</param>
        /// <returns>True if both NodeGenes have the same innovation number. False otherwise.</returns>
        public static bool operator ==(NodeGene left, NodeGene right) => left.Equals(right);


        /// <summary>
        /// Whether or not the left NodeGene is not equal to the right one.
        /// </summary>
        /// <param name="left">The left NodeGene to test.</param>
        /// <param name="right">The right NodeGene to test.</param>
        /// <returns>False if both NodeGenes have the same innovation number. True otherwise.</returns>
        public static bool operator !=(NodeGene left, NodeGene right) => !left.Equals(right);
    }
}
