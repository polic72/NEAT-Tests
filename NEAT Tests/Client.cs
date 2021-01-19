using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Calculation;
using NEAT_Tests.Gene_;

namespace NEAT_Tests
{
    /// <summary>
    /// Can be thought of as an individual animal.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The current calculator for the client. Can be updated with <see cref="NEAT_Tests.Client.Generate_Calculator"/>.
        /// </summary>
        public Calculator Calculator { get; private set; }

        /// <summary>
        /// The genome of this client. Should probably not be settable, but ok.
        /// </summary>
        public Genome Geneome { get; set; }


        /// <summary>
        /// The fitness score of the client.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The species that this client belongs to.
        /// </summary>
        public Species Species { get; set; }


        #region Calculator

        /// <summary>
        /// Generates a new stored calculator from the stored genome.
        /// </summary>
        public void Generate_Calculator()
        {
            Calculator = new Calculator(Geneome);
        }


        /// <summary>
        /// Runs the feed-forward method of the NN and returns the output.
        /// </summary>
        /// <param name="input">The values for the input nodes of the NN.</param>
        /// <returns>The output of the NN.</returns>
        public double[] Calculate(params double[] input)
        {
            if (Calculator == null)
            {
                Generate_Calculator();
            }

            return Calculator.Calculate(input);
        }

        #endregion Calculator
    }
}
