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
    /// This class implements K-Mondrian with strict partitioning.
    /// </summary>
    public class KMondrian : Mondrian
    {
        private int k;
        
        /// <param name="table">the table to be anonymized, the first attribute 
        /// in the DataTable has to be a unique id</param>
        /// <param name="qid">QI indecies</param>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <param name="k">k anonymity criteria</param>
        public KMondrian(DataTable table, List<int> qid, List<IHierarchy> hierarchies, int k)
            : base(table, qid, hierarchies)
        {
            this.k = k;
        }

        /// <summary>
        /// k-anonymity criteria
        /// </summary>
        /// <returns>Returs true, if the the input bucket contains at least k tuples,
        /// false otherwise.</returns>
        public override bool IsAnonymous(Bucket bucket)
        {
            if (bucket.Count >= this.k) return true;
            else return false;
        }

    }
}