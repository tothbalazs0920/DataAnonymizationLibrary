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
    /// Abstract class of the Mondrian algorithm with strict partitioning step.
    /// It operates with categorical attributes.
    /// </summary>
    public abstract class Mondrian
    {

        private BucketList result;
        private DataTable table;
        protected List<int> qid;
        private Queue<Bucket> queue;
        private Bucket wholeTable;
        private Dictionary<int, data.Tuple> idHashIndex;
        protected List<IHierarchy> hierarchies;
        
       
        /// <param name="table">the table to be anonymized, the first attribute 
        /// in the DataTable has to be a unique id</param>
        /// <param name="qid">QI indecies</param>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        public Mondrian(DataTable table, List<int> qid, List<IHierarchy> hierarchies)
        {
            this.table = table;
            this.qid = qid;
            this.hierarchies = hierarchies;
            result = new BucketList();
        }

        private void AddToResults(Bucket eq)
        {
            this.result.Add(eq);
        }

        public Bucket ConvertDataTableToBucket(DataTable dt)
        {
            Bucket bucket = new Bucket();

            foreach (DataRow row in dt.Rows)
            {
                var tuple = new data.Tuple();
                tuple.SetQid(qid);
                foreach (DataColumn column in dt.Columns)
                {                   
                    string value = row[column].ToString().Trim();
                    tuple.AddValue(value);                          
                }
                bucket.Add(tuple);
            }
            return bucket;
        }

        /// <summary>
        /// Convert a BucketList to a DataTable and adds group id's for each tuple.
        /// </summary>
        /// <returns>the converted DataTable</returns>
        public DataTable ConvertBucketListToDataTable(BucketList bt)
        {
            DataTable dt = table.Clone();
            dt.Columns.Add("GroupId", typeof(int));

            int groupId = 0;
            foreach (var bucket in bt)
            {
                foreach (var tuple in bucket)
                {
                    string[] values = tuple.GetValues();
                    List<object> r = new List<object>();
                    r.AddRange(values);
                    r.Add(groupId);                        
                    dt.Rows.Add(r.ToArray());
                }
                groupId++;
            }
            return dt;
        }

        public virtual BucketList Run()
        {        
            queue = new Queue<Bucket>();
            this.wholeTable = ConvertDataTableToBucket(table);
          
            idHashIndex = new Dictionary<int, data.Tuple>();

            // create a map to help the computation
            foreach (data.Tuple tuple in wholeTable)
            {
                //I suppose that the dimension of id is 0
                int id = Convert.ToInt32(tuple.GetValue(0));
                idHashIndex.Add(id, tuple);
            }
            var maximumGeneralizedBucket = wholeTable.GetMaximumGeneralizedBucket(hierarchies);
            queue.Enqueue(maximumGeneralizedBucket);

            while (queue.Count != 0)
            {
                Bucket temp = queue.Dequeue();
                if (!FindNextCategoricalDimension(temp, hierarchies)) this.AddToResults(temp);
            }            
            return result;
        }


        /// <returns>Returns true, if it is possible to split the bucket,
        /// false otherwise.</returns>
        private bool FindNextCategoricalDimension(Bucket bucket, List<IHierarchy> hierarchies)
        {
            for (int i = 0; i < hierarchies.Count; i++)
            {
                if (FindStrictSplit(bucket, hierarchies, i)) return true;
            }
            return false;
        }



        private string[] GetDistinctValues(Bucket eq, int dimension)
        {
            return eq.GetValuesByDimension(dimension).Distinct().ToArray();
        }

     

        /// <summary>
        /// Strict partitioning step of the Mondrian algorithm.
        /// </summary>
        /// <returns>Returns true, if it is possible to split the bucket,
        /// false otherwise.</returns>
        private bool FindStrictSplit(Bucket bucket, List<IHierarchy> hierarchies, int hierarchyIndex)
        {

            Node originalNode = CopyNode(bucket.node);
            Node specializedNode = CopyNode(bucket.node);
            int level = specializedNode.generalizations[hierarchyIndex];
            if (level != 0)
                specializedNode.generalizations[hierarchyIndex] = level - 1;
            else
                return false;

            bucket.node = CopyNode(specializedNode);

            Bucket specializedEQClass = GeneralizeBucket(bucket, hierarchies);

            int dimension = hierarchies[hierarchyIndex].GetQid();

            foreach (var splittingValue in GetDistinctValues(specializedEQClass, dimension))
            {
                Bucket right = new Bucket(), left = new Bucket();
                foreach (var t in specializedEQClass)
                {
                    if (t.GetValue(dimension).Equals(splittingValue))
                        left.Add(t);
                    else
                        right.Add(t);
                }
                // If the left has the same size as the original eq, maybe it is possible to release more generalizations.
                left.node = CopyNode(specializedNode);
                if (left.Count == bucket.Count)
                {
                    queue.Enqueue(left);
                    return true;
                }

                Node leftNode = left.node;
                var generalizedRight = new Bucket();
                generalizedRight.node = CopyNode(originalNode);

                //generalize back the right side/the rest
                if (right.Count > 0)
                {
                    if (right.HasDistinctValuesAt(dimension) != -1)
                    {
                        right.node = CopyNode(originalNode);
                        generalizedRight = GeneralizeBucket(right, hierarchies);
                    }
                    else
                    {
                        right.node = CopyNode(specializedNode);
                        generalizedRight = right;
                        generalizedRight.node = CopyNode(specializedNode);
                    }
                }

                if (IsAnonymous(left) && IsAnonymous(generalizedRight))
                {
                    queue.Enqueue(generalizedRight);
                    queue.Enqueue(left);
                    return true;
                }
            }
            bucket.node = CopyNode(originalNode);
            return false;
        }

        /// <summary>
        /// The class that extends this class should specify the anonymity criteria here.
        /// </summary>
        /// <returns>Returns true, if the anonymity criteria satisfied,
        /// false owtherwise.</returns>
        public abstract bool IsAnonymous(Bucket bucket);


        /// <summary>
        /// Generalizes or specializes the input bucket according to the given hierarchy 
        /// and hierchy level that is stored inside the hierarchy.
        /// </summary>
        /// <returns>Returns the specialized or generalized version of the input Bucket.</returns>
        protected Bucket GeneralizeBucket(Bucket bucket, List<IHierarchy> hierarchies)
        {
            Bucket newEq = new Bucket();

            for (int i = 0; i < hierarchies.Count; i++)
            {
                hierarchies[i].SetLevel(bucket.node.generalizations[i]);
            }

            foreach (data.Tuple tuple in bucket)
            {
                //I suppose that the dimension of id is 0
                int id = Convert.ToInt32(tuple.GetValue(0));
                newEq.Add(idHashIndex[id]);
            }
            var generalizedEq = newEq.Generalize(hierarchies);

            Node node = CopyNode(bucket.node);
            generalizedEq.node = node;
            return generalizedEq;
        }


        protected Node CopyNode(Node fromNode)
        {
            Node node = new Node();
            foreach (var i in fromNode.generalizations)
            {
                node.generalizations.Add(i);
            }
            return node;
        }

        /*
        * It adds an attribute to the table. This attributes shows,
        * if a tuple exists in the private table or not.
        * If an id exists in the private table, the sensitive value is 1, otherwise 0.
        */

        /// <summary>
        /// It adds an attribute to the table. This attributes shows, 
        /// if a tuple exists in the private table or not.
        /// If an id exists in the private table, the sensitive value is 1, otherwise 0.
        /// </summary>
        /// <param name="publicTable">public/external table</param>
        /// <param name="privateTable">private table to be anonymized</param>
        protected void InsertSensitiveValues(DataTable publicTable, DataTable privateTable)
        {
            publicTable.Columns.Add("IsInExternalTable", typeof(string));
            var col = table.Columns["IsInExternalTable"];
            // dictionary of the ids of the private table 
            Dictionary<int, bool> sensitiveIds = new Dictionary<int, bool>();

            foreach (DataRow row in privateTable.Rows)
            {
                sensitiveIds[Int32.Parse(row.ItemArray[0].ToString())] = true;  
            }

            foreach (DataRow row in publicTable.Rows)
            {
                if (sensitiveIds.ContainsKey(Int32.Parse(row.ItemArray[0].ToString())))  row[col] = "1";
                else row[col] = "0";
            }
        }


    }
}
