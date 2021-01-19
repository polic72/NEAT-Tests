using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests
{
    /// <summary>
    /// A species of clients (clients that are similar enough to one another).
    /// </summary>
    public class Species
    {
        /// <summary>
        /// The quintessential client of this species.
        /// </summary>
        public Client Representative { get; set; }

        private RandomHashSet<Client> clients = new RandomHashSet<Client>();

        /// <summary>
        /// The size of the total number of clients in this species.
        /// </summary>
        public int Size { get { return clients.Size; } }

        /// <summary>
        /// The average score of this species.
        /// </summary>
        public double Score { get; set; }


        private Random random;


        /// <summary>
        /// Constructs a Species with the given representative client.
        /// </summary>
        /// <param name="representative">The client to represent the core of the species.</param>
        /// <param name="random">The random object to use.</param>
        public Species(Client representative, Random random)
        {
            Representative = representative;
            representative.Species = this;

            clients.Add(representative);

            this.random = random;
        }


        /// <summary>
        /// Puts the client into the list of recognized clients. The distance to the Representative must be within the 
        /// <see cref="NEAT_Tests.NEAT.MAXIMUM_RACIAL_DIFFERENCE"/>.
        /// </summary>
        /// <param name="client">The client to add.</param>
        /// <returns>True if added, false otherwise.</returns>
        public bool Put(Client client)
        {
            if (client.Geneome.Distance(Representative.Geneome) < NEAT.MAXIMUM_RACIAL_DIFFERENCE)
            {
                client.Species = this;
                clients.Add(client);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Puts the client into the list of recognized clients. Does not check if it should be in there.
        /// </summary>
        /// <param name="client">The client to add.</param>
        public void ForcePut(Client client)
        {
            client.Species = this;
            clients.Add(client);
        }
        

        /// <summary>
        /// Evaluates the score based on the current internal clients.
        /// </summary>
        public void EvaluateScore()
        {
            //TODO use Linq after making RandomHashSet IEnumerable
            double sum = 0;

            for (int i = 0; i < clients.Size; ++i)
            {
                sum += clients[i].Score;
            }

            Score = sum / clients.Size;
        }


        /// <summary>
        /// Makes the species lose every client it has.
        /// </summary>
        public void GoExtict()
        {
            //TODO foreach would be nice
            for (int i = 0; i < clients.Size; ++i)
            {
                clients[i].Species = null;
            }
        }


        /// <summary>
        /// Resets the Species. Not sure why this is less "final" than <see cref="NEAT_Tests.Species.GoExtict"/>.
        /// </summary>
        public void Reset()
        {
            Representative = clients.GetRandomElement();

            for (int i = 0; i < clients.Size; ++i)
            {
                clients[i].Species = null;
            }

            clients.Clear();


            clients.Add(Representative);
            Score = 0;
        }


        /// <summary>
        /// Kills the bottom [given-percentage]% of the clients in this species.
        /// </summary>
        /// <param name="percentage">The percentage of clients to prune.</param>
        public void Prune(double percentage)
        {
            clients.Sort();


            for (int i = 0; i < percentage * clients.Size; ++i)
            {
                clients.Get(0).Species = null;

                clients.Remove(0);
            }
        }


        /// <summary>
        /// Breeds 2 random genomes in the stored clients.
        /// </summary>
        /// <returns>The breeded genome.</returns>
        public Genome Breed()
        {
            Client client_1 = clients.GetRandomElement();
            Client client_2 = clients.GetRandomElement();

            if (client_1.Score < client_2.Score)    //Isn't this unnecessary?
            {
                return Genome.CrossOver(client_1.Geneome, client_2.Geneome, random);
            }
            else
            {
                return Genome.CrossOver(client_2.Geneome, client_1.Geneome, random);
            }
        }
    }
}
