using AnonymizationLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.costmetrics
{
    
    public class DiscernibilityMetric : ICostMetric
    {

        public int GetMeasurement(BucketList bucketList)
        {
            int result = 0;
            foreach (var bucket in bucketList)
                result += (bucket.Count * bucket.Count());
          
            return result;
        }

      

        public int GetMeasurementOfPublicTable(BucketList bucketList)
        {
            int result = 0;
            foreach (var bucket in bucketList)
                result += (bucket.SensitiveValueCount() * bucket.SensitiveValueCount());

            return result;
        }

       

        

    }
}
