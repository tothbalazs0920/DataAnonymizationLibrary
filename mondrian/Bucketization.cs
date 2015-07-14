using AnonymizationLibrary.data;
using AnonymizationLibrary.hierarchies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.mondrian
{
    /// <summary>
    /// This calss implements bucketization. It uses K-Mondrian as its partitioning algorithm.
    /// </summary>
    public class Bucketization : KMondrian
    {

        private List<int> sensitiveAttributeIndices;

        public Bucketization(DataTable table, List<int> qid, List<IHierarchy> hierarchies, int k, List<int> sensitiveAttributeIndices)
            : base(table, qid, hierarchies, k)
        {
            this.sensitiveAttributeIndices = sensitiveAttributeIndices;
        }

        /// <summary>
        /// Tuple partitioning is dane by K-Mondrian. 
        /// This method doesn't generalize the tuples, but permutes the sensitive values 
        /// within each sensitive value column inside every bucket. 
        /// </summary>
        /// <returns>a bucketized BucketList</returns>
        public override BucketList Run()
        {
            var result = new BucketList();
            var generalizedBucketList = base.Run();

            foreach (var bucket in generalizedBucketList)
            {
                //undo all of the generalizations
                for (int i = 0; i < bucket.node.generalizations.Count; i++)
                {
                    bucket.node.generalizations[i] = 0;
                }
                var permutedBucket = GeneralizeBucket(bucket, hierarchies);

                foreach (int i in sensitiveAttributeIndices)
                    permutedBucket.PermuteValues(i);

                result.Add(permutedBucket);
            }
            return result;
        }

    }
}
