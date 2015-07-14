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
    /// This class implements MPALM.
    /// </summary>
    public class Mpalm : Mondrian
    {

        DataTable privateTable;
        double min, max;
             
        /// <param name="publicTable">public/external table</param>
        /// <param name="privateTable">private table to be anonymized</param>
        /// <param name="qid">QI indecies</param>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <param name="min">minimum delta presence threshold value</param>
        /// <param name="max">maximum delta presence threshold value</param>
        public Mpalm(DataTable publicTable, DataTable privateTable, List<int> qid, List<IHierarchy> hierarchies, double min, double max)
            : base(publicTable, qid, hierarchies)
        {
            this.min = min;
            this.max = max;
            this.privateTable = privateTable;
            InsertSensitiveValues(publicTable, privateTable);
        }

        public override BucketList Run()
        {
            var result = new BucketList();
            var generalizedBucketList = base.Run();

            foreach (var bucket in generalizedBucketList)
            {
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
                    result.Add(privateBucket);
            }
            return result;
        }

        /// <summary>
        /// delta presence anonymity criteria
        /// </summary>
        /// <returns>Returs true, if the the input bucket delta presence value is 
        /// between min and max delta presence threshold values,
        /// false otherwise.</returns>
        public override bool IsAnonymous(Bucket bucket)
        {
            if (bucket.IsGeneralizedBucketDeltaPresent(min, max)) return true;
            else return false;
        }

    }
}
