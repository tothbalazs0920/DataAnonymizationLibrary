using AnonymizationLibrary.criteria;
using AnonymizationLibrary.data;
using AnonymizationLibrary.hierarchies;
using AnonymizationLibrary.mondrian;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.mondrian
{
    /// <summary>
    /// This class implements algorithm 6 from my thesis.
    /// </summary>
    public class GeneralizationAndPermutationConbined : Mondrian
    {

        DataTable privateTable;
        double min, max;
        int k;

        public GeneralizationAndPermutationConbined(DataTable publicTable, DataTable privateTable, List<int> qid, List<IHierarchy> hierarchies, double min, double max, int k)
            : base(publicTable, qid, hierarchies)
        {
            this.k = k;
            this.min = min;
            this.max = max;
            this.privateTable = privateTable;
            InsertSensitiveValues(publicTable, privateTable);
        }

        public override BucketList Run()
        {

            var result = new BucketList();
            var generalizedBucketList = base.Run();
            var genPerm = GetBestPermutedTable(generalizedBucketList);


            foreach (var bucket in genPerm)
            {
                //if any of the bucket is null, it means that it is not delta-present, k has to be increased 
                if (bucket == null) return null;
                var privateBucket = new Bucket();
                foreach (var tuple in bucket)
                {
                    if (Int32.Parse(tuple.GetValue(tuple.GetNumberOfAttributes() - 1)) == 1)
                    {
                        tuple.RemoveAttribute(tuple.GetNumberOfAttributes() - 1);
                        privateBucket.Add(tuple);
                    }                    
                    if (privateBucket.Count > 0)                   
                        privateBucket.node = bucket.node;                                           
                }
                if (privateBucket.Count > 0)
                {
                    foreach (int i in qid)
                        privateBucket.PermuteValues(i);
                    result.Add(privateBucket);
                }
            }
            
            return result;
        }


        public override bool IsAnonymous(Bucket bucket)
        {
            if (bucket.Count >= this.k) return true;
            else return false;
        }


        /*
         * It returns a delta present BucketList with as less generalization as possible.
         */
        public BucketList GetBestPermutedTable(BucketList bucketList)
        {
            BucketList bestTable = new BucketList();
            for (int i = 0; i < bucketList.Count(); i++)
            {
                List<Node> nodeList = MakeNodeList(bucketList[i].node);
                var anonymizedBucket = GeneralizeAndPermute(min, max, bucketList[i], qid, nodeList, hierarchies);
                bestTable.Add(anonymizedBucket);
            }
            
            return bestTable;
        }


        /*
        * It returns a delta present Bucket with as less generalization as possible.
        */
        public Bucket GeneralizeAndPermute(double min, double max, Bucket bucket, List<int> qid, List<Node> nodeList, List<IHierarchy> hierarchies)
        {
            Node mostgeneralizedNode = bucket.node;

            foreach (var node in nodeList)
            {
                bucket.node = CopyNode(node);
                var generalizedBucket = GeneralizeBucket(bucket, hierarchies);
                DeltaPresence dp5 = new DeltaPresence();
                if (dp5.IsDeltaPresent(min, max, generalizedBucket, qid.ToArray()))
                {
                    generalizedBucket.node = CopyNode(node);
                    return generalizedBucket;
                }
            }
            return null;
        }


        /*
         * It returns a sorted generalization node list belongigng to a node/bucket
         */
        public List<Node> MakeNodeList(Node inputNode)
        {
            List<int[]> nodeLevels = new List<int[]>();
            foreach (var h in inputNode.generalizations)
            {
                int max = h; // deth = number of dictionaries in the list -or number of levels

                int[] level = new int[max + 1];
                for (int i = 0; i <= max; i++)
                {
                    level[i] = i;
                }
                nodeLevels.Add(level);
            }

            var cartesianProductOfPointers = CartesianProductExtension.CartesianProduct(nodeLevels);

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

    }
}
