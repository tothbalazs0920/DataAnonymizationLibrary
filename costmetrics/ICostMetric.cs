using AnonymizationLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.costmetrics
{
    public interface ICostMetric
    {
        int GetMeasurement(BucketList bucketList);
    }
}
