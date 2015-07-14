using AnonymizationLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.criteria
{


    /// <summary>
    /// This class computes of delta presence of generalized and permuted buckets/tables.
    /// I got help from Troels Bjerre Sørensen <trbj@itu.dk>  in the implementation of this class.
    /// </summary>
    public class DeltaPresence
    {
        //A -set of tuples that exist in in the external table.
        //B -set of tuples that exist in the private table.
        int[][] A, B;

        int[] successfulOutcomesCount;
        int possibleOutcomeCount, numRecord, numAttributes, numSample;
        int[] selected;
        int[][] f;

        //This dictionary stores the mapping between the real values of the tuples, and the integers they are mapped to.
        Dictionary<string, int> dictionary;
        int valueCount;
        string[] ids;
        List<int> sensitiveValueIndexes;

        public DeltaPresence()
        {
            dictionary = new Dictionary<string, int>();
        }


        /// <summary>
        /// Computes the the probabilities for each record to exist in B.
        /// </summary>
        /// <param name="A">set of tuples that exist in in the external table</param>
        /// <param name="B">set of tuples that exist in the private table</param>
        /// <returns>A double array containing the probabilities for each record to exist in B</returns>
        public double[] Probability(int[][] A, int[][] B)
        {
            this.A = A;
            this.B = B;
            numRecord = A.Length;
            numSample = B.Length;
            numAttributes = A[0].Length;

            successfulOutcomesCount = new int[numRecord];
            selected = new int[numSample];
            int max = 0;
            for (int p = 0; p < numRecord; p++)
            {
                for (int a = 0; a < numAttributes; a++)
                {
                    max = Math.Max(max, A[p][a]);
                }
            }

            //initialize f
            // f stores the frequency of each value in each column of B
            // row index in f corresponds to the colomn index in B
            // column index in f corresponds to values in B
            f = new int[numAttributes][];
            for (var i = 0; i < f.Length; i++) f[i] = new int[max + 1];

            //initial values of f
            for (int p = 0; p < numSample; p++)
            {
                for (int a = 0; a < numAttributes; a++)
                {
                    f[a][B[p][a]]++;
                }
            }

            possibleOutcomeCount = 0;
            Helper(0, numSample);
            double[] retval = new double[numRecord];
            for (int p = 0; p < numRecord; p++) retval[p] = successfulOutcomesCount[p] / (double)possibleOutcomeCount;
            return retval;
        }




        /// <summary>
        /// This recursive method helps to count the probabilities. 
        /// </summary>
        /// <param name="p">column index in A, it is also an index of a person</param>
        /// <param name="left">number of left records in the array selected</param>
        void Helper(int p, int left)
        {
            //if left is 0, it means that the array selected is full. selected contains record ids
            //successfulOutcomeCount[] stores for each record, how many successful outcome they have
            if (left == 0)
            {
                //possibleOutcomeCount counts the possible outcomes
                possibleOutcomeCount++;
                foreach (int s in selected) successfulOutcomesCount[s]++;
                return;
            }
            //Here, it is not out of the array, because it returns.
            if (p >= numRecord) return;
            Helper(p + 1, left);
            //pruning step
            for (int a = 0; a < numAttributes; a++)
                if (f[a][A[p][a]] == 0) // if A has a specific value, but the count of that value in f is 0.
                {
                    return;
                }
            for (int a = 0; a < numAttributes; a++) f[a][A[p][a]]--;
            selected[left - 1] = p;
            //try with different records
            Helper(p + 1, left - 1);
            //load back the counter to f
            for (int a = 0; a < numAttributes; a++) f[a][A[p][a]]++;
        }





        /// <summary>
        /// Adds the mapped integer values to A and B.
        /// </summary>
        /// <param name="bucket">the bucket that to be examined</param>
        /// <param name="qid">QI indecies</param>
        void CreateAB(Bucket bucket, int[] qid)
        {
            ids = new string[bucket.Count];
            // here the sensitive value signifies, if the tuple is in B or not
            // store the sensitive value indexes in this:
            sensitiveValueIndexes = new List<int>();
            dictionary = new Dictionary<string, int>();

            for (int k = 0; k < bucket.Count; k++)
            {
                data.Tuple tuple = bucket[k];
                // I suppose that the sensitive value is the last one
                int dimension = tuple.GetNumberOfAttributes() - 1;
                int sensitiveValue = Convert.ToInt32(tuple.GetValue(dimension));
                if (sensitiveValue == 1) sensitiveValueIndexes.Add(k);
            }

            //1. Initialize  A and B.
            A = new int[bucket.Count][];
            for (var i = 0; i < A.Length; i++) A[i] = new int[qid.Length];
            B = new int[sensitiveValueIndexes.Count][];
            for (var i = 0; i < B.Length; i++) B[i] = new int[qid.Length];
            int nextIndexForB = -1;

            // 2. Add the mapped integer values to A and B
            // In A, row index corresponds to the tuple index. In B row index doesn't have extra information.
            // column index corresponds to the attributes index
            for (int k = 0; k < bucket.Count; k++)
            {
                data.Tuple tuple = bucket[k];
                int dimension = tuple.GetNumberOfAttributes() - 1;
                // I suppose that the sensitive value is the last one
                //int sensitiveValue = Convert.ToInt32(eq[k].GetValue(eq[k].getNumberOfAttributes() - 1));
                int sensitiveValue = Convert.ToInt32(tuple.GetValue(dimension));
                if (sensitiveValue == 1) nextIndexForB++;
                //Add the id attribute: 0 in this case
                ids[k] = (bucket[k].GetValue(0));

                for (int y = 0; y < qid.Count(); y++)
                {
                    int value = -1;
                    string key = bucket[k].GetValue(qid[y]);
                    // map the original values to small integers
                    if (dictionary.ContainsKey(key)) value = dictionary[key];
                    else
                    {
                        dictionary[key] = valueCount++;
                        value = dictionary[key];
                    }
                    A[k][y] = value;
                    if (sensitiveValue == 1) B[nextIndexForB][y] = value;
                }
            }
        }




        /// <param name="min">minimum delta presence threshold</param>
        /// <param name="max">maximum delta presence threshold</param>
        /// <param name="bucket">the bucket that to be examined</param>
        /// <param name="qid">QI indecies</param>
        /// <returns>Returns true, if the bucket is delta present, false otherwise.</returns>
        public bool IsDeltaPresent(double min, double max, Bucket bucket, int[] qid)
        {
            CreateAB(bucket, qid);
            double[] probabilities = Probability(A, B);

            for (int k = 0; k < probabilities.Count(); k++)
            {
                double curValue = probabilities[k];
                if (curValue < min || curValue > max)
                    return false;
            }
            return true;
        }

        /* This method returns the list of reidentified record ids. 
         */

        /// <summary>
        /// Finds records whose probability to exist in B is higher, than min.
        /// </summary>
        /// <param name="bucket">the bucket that to be examined</param>
        /// <param name="qid">QI indecies</param>
        /// <param name="min">the minimum probability that makes a record reidentified</param>
        /// <returns>Returns the list of reidentified record ids.</returns>
        public List<string> ReIdentifyIndividuals(Bucket bucket, int[] qid, double min)
        {
            List<string> reidentifiedIndividuals = new List<string>();
            CreateAB(bucket, qid);
            double[] probabilities = Probability(A, B);
            for (int i = 0; i < probabilities.Count(); i++)
            {
                if (probabilities[i] >= min)
                    reidentifiedIndividuals.Add(ids[i]);
            }
            return reidentifiedIndividuals;
        }

    }
}
