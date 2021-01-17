using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT_Tests.Gene_
{
    /// <summary>
    /// A base class for a gene.
    /// </summary>
    public class Gene
    {
        /// <summary>
        /// The innovation number of this gene.
        /// </summary>
        public int InnovationNumber { get; set; }


        /// <summary>
        /// Constructs a gene with the given innovation number.
        /// </summary>
        /// <param name="innovation_number">The innovation number.</param>
        public Gene(int innovation_number)
        {
            InnovationNumber = innovation_number;
        }
    }
}
