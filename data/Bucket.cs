using AnonymizationLibrary.criteria;
using AnonymizationLibrary.hierarchies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.data
{

    public class Bucket : List<Tuple>
    {
        private int[] qid = null;
        public Node node { get; set; }

        public Bucket()
            : base()
        {
        }

        public Bucket(Tuple t)
            : base()
        {
            this.Add(t);
        }


        public Bucket(int[] qid)
            : base()
        {
            this.qid = qid;
        }

        /// <summary>
        /// Computes, if the generalized bucket is delta present.
        /// </summary>
        /// <param name="min">minimum delta presence threshold</param>
        /// <param name="max">maximum delta presence threshold</param>
        /// <returns>True, if the generalized bucket is delata present, false otherwise.</returns>
        public bool IsGeneralizedBucketDeltaPresent(double min, double max)
        {
            double deltaValue = (double)SensitiveValueCount() / (double)this.Count;
            if (deltaValue >= min && deltaValue <= max) return true;
            return false;
        }



        /// <summary>
        /// Generalizes the tuples of the Bucket with the help of domain generalization hierarchies.
        /// </summary>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <returns>Returns a Bucket containing the generalized tuples.</returns>
        public Bucket Generalize(List<IHierarchy> hierarchies)
        {
            Bucket anonymizedEq = new Bucket();

            foreach (Tuple tuple in this)
            {
                Tuple anonymizedTuple = new Tuple(tuple.Generalize(hierarchies));
                anonymizedEq.Add(anonymizedTuple);
            }
            return anonymizedEq;
        }


        /// <param name="dimension">index of the column/dimension</param>
        /// <returns>Returns an array of strings containing each value in a dimension/column</returns>
        public string[] GetValuesByDimension(int dimension)
        {
            string[] values = new string[this.Count()];
            for (int i = 0; i < this.Count(); i++)
                values[i] = this[i].GetValue(dimension);
            return values;
        }


        public override string ToString()
        {
            string buffer = "[";
            int i;
            if (this.Count() >= 1)
            {
                for (i = 0; i < this.Count() - 1; i++)
                    buffer += "(" + this[i] + "),";
                buffer += "(" + this[i] + ")]";
            }
            else
                buffer += "]";
            return buffer;
        }




        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <returns>Returns the sorted generalization nodeList.</returns>
        public List<Node> MakeNodeList(List<IHierarchy> hierarchies)
        {
            List<int[]> hierArchiesLevels = new List<int[]>();
            foreach (IHierarchy h in hierarchies)
            {
                int depth = h.GetDepth(); // deth = number of dictionaries in the list -or number of levels

                int[] level = new int[depth];
                for (int i = 0; i < depth; i++)
                {
                    level[i] = i;
                }
                hierArchiesLevels.Add(level);
            }

            var cartesianProductOfPointers = CartesianProductExtension.CartesianProduct(hierArchiesLevels);

            int idCount = 0;
            List<Node> nodeList = new List<Node>();

            foreach (var product in cartesianProductOfPointers)
            {
                Node node = new Node();
                node.id = idCount++;
                node.generalizations = product.ToList();
                nodeList.Add(node);
            }
            nodeList.Sort();
            return nodeList;
        }


        /// <summary>
        /// Generalizes the tuples in the Bucket to the maximum.
        /// </summary>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <returns>Returns a Bucket that contains the most generalized versions of the tuples.</returns>
        public Bucket GetMaximumGeneralizedBucket(List<IHierarchy> hierarchies)
        {
            Bucket generalizedBucket = new Bucket();
            Node node = new Node();

            foreach (var hierarchy in hierarchies)
            {
                hierarchy.SetLevel(hierarchy.GetDepth() - 1);
                node.generalizations.Add(hierarchy.GetDepth() - 1);
            }
            generalizedBucket.node = node;

            foreach (var tuple in this)
                generalizedBucket.Add(new Tuple(tuple.Generalize(hierarchies)));

            return generalizedBucket;
        }




        /// <summary>
        /// Cheks, whether there are different values in a column.
        /// </summary>
        /// <param name="dimension">column index</param>
        /// <returns>If there are different values in a column, it returns the index of the tuple, 
        /// which has different value in the specified dimension, than the first tuple in the bucket.
        /// If the column is single valued, the method returns -1.
        /// </returns>
        public int HasDistinctValuesAt(int dimension)
        {
            var value = this[0].GetValue(dimension);
            for (int i = 0; i < this.Count; i++)
            {
                if (!this[i].GetValue(dimension).Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }



        /// <returns>Returns the number of tuples existing in the private table.</returns>
        public int SensitiveValueCount()
        {
            int count = 0;
            foreach (var tuple in this)
            {
                if (Int32.Parse(tuple.GetValue(tuple.GetNumberOfAttributes() - 1)) == 1) count++;
            }
            return count;
        }

        /// <summary>
        /// Permutes the values in a column/dimension.
        /// </summary>
        /// <param name="dimension">column index</param>
        public void PermuteValues(int dimension)
        {
            Random random = new Random();

            foreach (var tuple in this)
            {
                int randomInt = random.Next(0, this.Count);
                var otherTuple = this[randomInt];
                string temp = tuple.GetValue(dimension);
                tuple.SetValue(dimension, otherTuple.GetValue(dimension));
                otherTuple.SetValue(dimension, temp);

            }
        }


    }
}
