# Data Anonymization Library

The library consists of implementations of some data anonymization algorithms in c#.

Generalization based algorithms:
- K-Mondrian
- Mpalm

Permutation based algorithms:
- Bucketization
- One-Attribute-Per-Column-Slicing

Permutation Combined with Generalization 

Example usage:

```csharp
public class Program
    {
        static void Main(string[] args)
        {
            //The first attribute has to be a unique id in the DataTable
            
            DataTable kMondrian = KMondrian();
            Helper.WriteCsvFile(kMondrian, "data//kMondrian.csv");

            DataTable mpalm = Mpalm();
            Helper.WriteCsvFile(mpalm, "data//mpalm.csv");

            DataTable bucketization = Bucketization();
            Helper.WriteCsvFile(bucketization, "data//bucketization.csv");

            DataTable oneAttributePerColumnSlicing = OneAttributePerColumnSlicing();
            Helper.WriteCsvFile(oneAttributePerColumnSlicing, "data//oneAttributePerColumnSlicing.csv");

            DataTable gap = GeneralizationAndPermutationConbined();
            Helper.WriteCsvFile(gap, "data//generalizationAndPermutationConbined.csv");

         
        }


        public static DataTable Mpalm()
        {
            DataTable publicTable = Helper.LoadCsv("data//adult.csv");
            DataTable privateTable = Helper.LoadCsv("data//adultSubset.csv");

            Mpalm mpalm = new Mpalm(publicTable, privateTable, 
                Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 0, 0.5);

            var result = mpalm.Run();

            int discernibility = new DiscernibilityMetric().GetMeasurement(result);
            int amountOfGeneralization = new AmountOfGeneralization().GetMeasurement(result);

            return mpalm.ConvertBucketListToDataTable(result);
        }

        public static DataTable Bucketization()
        {
            List<int> sa = new List<int>();
            sa.Add(0);
            DataTable publicTable = Helper.LoadCsv("data//adult.csv");
            var bucketization = new Bucketization(publicTable,
               Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 4, sa);
            var result = bucketization.Run();
            return bucketization.ConvertBucketListToDataTable(result);
        }


        public static DataTable OneAttributePerColumnSlicing()
        {
            DataTable table = Helper.LoadCsv("data//adult.csv");
            var oneAttributePerColumnSlicing = new OneAttributePerColumnSlicing(table,
               Helper.CreateAdultQid() , Helper.MakeAdultHierarchies(), 4);
            var result = oneAttributePerColumnSlicing.Run();
            return oneAttributePerColumnSlicing.ConvertBucketListToDataTable(result);
        }


        public static DataTable KMondrian()
        {
            DataTable table = Helper.LoadCsv("data//adult.csv");
            var kMondrian = new KMondrian(table, 
                Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 4);
            var result = kMondrian.Run();
            return kMondrian.ConvertBucketListToDataTable(result);       
        }

        public static DataTable GeneralizationAndPermutationConbined()
        {

            DataTable publicTable = Helper.LoadCsv("data//adult.csv");
            DataTable privateTable = Helper.LoadCsv("data//adultSubset.csv");

            var gap = new GeneralizationAndPermutationConbined(publicTable, 
                privateTable, Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 0, 0.9, 8);
         
            var result = gap.Run();
            return gap.ConvertBucketListToDataTable(result); 
        }
    }
}
```
