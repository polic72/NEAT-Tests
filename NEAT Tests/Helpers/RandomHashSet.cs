﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEAT_Tests.Gene_;

namespace NEAT_Tests
{
    /// <summary>
    /// Acts as a list with a hashed set next to it for a quick contains method.
    /// </summary>
    /// <typeparam name="T">The objects being stored.</typeparam>
    public class RandomHashSet<T>
    {
        private HashSet<T> set;
        private List<T> list;

        private Random random;


        /// <summary>
        /// Constructs a RandomHashSet with default capacity.
        /// </summary>
        public RandomHashSet()
        {
            set = new HashSet<T>();
            list = new List<T>();

            random = new Random();
        }


        /// <summary>
        /// Constructs a RandomHashSet with the given capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity of this RandomHashSet.</param>
        public RandomHashSet(int capacity)
        {
            set = new HashSet<T>(capacity);
            list = new List<T>(capacity);

            random = new Random();
        }


        /// <summary>
        /// Whether or not this RandomHashSet contains the given object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if this RandomHashSet contains it, false otherwise.</returns>
        public bool Contains(T obj)
        {
            return set.Contains(obj);
        }


        /// <summary>
        /// The number of elements in this RandomHashSet.
        /// </summary>
        public int Size { get { return list.Count; } }


        /// <summary>
        /// Gets a radom element in this RandomHashSet.
        /// </summary>
        /// <returns>Random T object if there is any in there, default(T) otherwise.</returns>
        public T GetRandomElement()
        {
            if (set.Count > 0)
            {
                return list[random.Next(0, list.Count)];
            }

            return default;
        }


        /// <summary>
        /// Adds the given T to this RandomHashSet.
        /// </summary>
        /// <param name="obj">The T object to add.</param>
        /// <returns>True if added, false if already contained.</returns>
        public bool Add(T obj)
        {
            if (!set.Contains(obj))
            {
                set.Add(obj);
                list.Add(obj);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Adds the given gene sorted by innovation number. T must be castable as a Gene.
        /// </summary>
        /// <param name="gene">The gene to add.</param>
        public void Add_Sorted_Gene(Gene gene)
        {
            if (typeof(Gene).IsAssignableFrom(typeof(T)))
            {
                for (int i = 0; i < Size; ++i)
                {
                    if (gene.InnovationNumber < ((Gene)(object)list[i]).InnovationNumber)   //Gotta love generics.
                    {
                        list.Insert(i, (T)(object)gene);
                        set.Add((T)(object)gene);

                        return;
                    }
                }

                list.Add((T)(object)gene);
                set.Add((T)(object)gene);
            }
        }


        /// <summary>
        /// Clears every T in this RandomHashSet.
        /// </summary>
        public void Clear()
        {
            set.Clear();
            list.Clear();
        }


        /// <summary>
        /// Gets the T at the given index.
        /// </summary>
        /// <param name="index">The index to query.</param>
        /// <returns>The T at the given index, default(T) if not a valid index.</returns>
        public T Get(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                return default;
            }

            return list[index];
        }


        /// <summary>
        /// Removes the T at the given index.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>True if removed, false if invalid index.</returns>
        public bool Remove(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                return false;
            }

            set.Remove(list[index]);
            list.Remove(list[index]);

            return true;
        }


        /// <summary>
        /// Removes the given T.
        /// </summary>
        /// <param name="obj">The T object to remove.</param>
        /// <returns>True if found and removed, false otherwise.</returns>
        public bool Remove(T obj)
        {
            return set.Remove(obj) && list.Remove(obj);
        }


        /// <summary>
        /// Sorts the clients by Score.
        /// </summary>
        public void Sort()
        {
            if (typeof(Client).IsAssignableFrom(typeof(T)))
            {
                //List<Client> clients = list.Cast<Client>().ToList();

                //list.Sort(new IComparer<Client>((x, y) => (x.Score < y.Score) ? -1 : ((x.Score > y.Score) ? 1 : 0));

                List<Client> temp = list.Cast<Client>().ToList();
                temp.Sort(new Comparison<Client>((x, y) => (x.Score < y.Score) ? -1 : ((x.Score > y.Score) ? 1 : 0)));

                list = temp.Cast<T>().ToList();
            }
        }


        /// <summary>
        /// Gets the T at the given index.
        /// </summary>
        /// <param name="index">The index to query.</param>
        /// <returns>The T at the given index, default(T) if not a valid index.</returns>
        public T this[int index]
        {
            get { return Get(index); }
        }
    }
}
