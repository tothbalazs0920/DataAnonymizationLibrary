using AnonymizationLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.costmetrics
{
    public class AmountOfGeneralization :ICostMetric
    {

        public int GetMeasurement(BucketList bucketList)
        {
            int result = 0;
            foreach (var bucket in bucketList)
            {
                result += (bucket.Count() * (bucket.node.SumOfLevels()));
            }
            return result;
        }

        public int GetMeasurementOfPublicTable(BucketList bucketList)
        {
            int result = 0;           
            foreach (var bucket in bucketList)
            {
                int count = bucket.SensitiveValueCount();
                result += (count * (bucket.node.SumOfLevels()));
            }
            return result;
        }

       
   
    }
}
