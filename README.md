# Data Anonymization Library

The library consists of implementations of some data anonymization algorithms in c# using .NET 4.5.
Each algorithm return a System.Data.DataTable with the anonymized data.

Generalization based algorithms:
- K-Mondrian
- Mpalm

Permutation based algorithms:
- Bucketization
- One-Attribute-Per-Column-Slicing

Permutation Combined with Generalization 

## Example usage:


```csharp
public class Program
    {
        static void Main(string[] args)
        {
            //The first attribute has to be a unique id in the DataTable
            
            DataTable kMondrian = KMondrian();
            Helper.WriteCsvFile(kMondrian, "kMondrian.csv");

            DataTable mpalm = Mpalm();
            Helper.WriteCsvFile(mpalm, "mpalm.csv");

            DataTable bucketization = Bucketization();
            Helper.WriteCsvFile(bucketization, "bucketization.csv");

            DataTable oneAttributePerColumnSlicing = OneAttributePerColumnSlicing();
            Helper.WriteCsvFile(oneAttributePerColumnSlicing, "oneAttributePerColumnSlicing.csv");

            DataTable gap = GeneralizationAndPermutationConbined();
            Helper.WriteCsvFile(gap, "generalizationAndPermutationConbined.csv");

         
        }


        public static DataTable Mpalm()
        {
            DataTable publicTable = Helper.LoadCsv("adult.csv");
            DataTable privateTable = Helper.LoadCsv("adultSubset.csv");

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
            DataTable publicTable = Helper.LoadCsv("adult.csv");
            var bucketization = new Bucketization(publicTable,
               Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 4, sa);
            var result = bucketization.Run();
            return bucketization.ConvertBucketListToDataTable(result);
        }


        public static DataTable OneAttributePerColumnSlicing()
        {
            DataTable table = Helper.LoadCsv("adult.csv");
            var oneAttributePerColumnSlicing = new OneAttributePerColumnSlicing(table,
               Helper.CreateAdultQid() , Helper.MakeAdultHierarchies(), 4);
            var result = oneAttributePerColumnSlicing.Run();
            return oneAttributePerColumnSlicing.ConvertBucketListToDataTable(result);
        }


        public static DataTable KMondrian()
        {
            DataTable table = Helper.LoadCsv("adult.csv");
            var kMondrian = new KMondrian(table, 
                Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 4);
            var result = kMondrian.Run();
            return kMondrian.ConvertBucketListToDataTable(result);       
        }

        public static DataTable GeneralizationAndPermutationConbined()
        {

            DataTable publicTable = Helper.LoadCsv("adult.csv");
            DataTable privateTable = Helper.LoadCsv("adultSubset.csv");

            var gap = new GeneralizationAndPermutationConbined(publicTable, 
                privateTable, Helper.CreateAdultQid(), Helper.MakeAdultHierarchies(), 0, 0.9, 8);
         
            var result = gap.Run();
            return gap.ConvertBucketListToDataTable(result); 
        }
    }
```
The Helper class:

```csharp
public class Helper
    {
        public static List<int> CreateAdultQid()
        {
            List<int> qid = new List<int>();
            qid.Add(7);
            qid.Add(6);
            qid.Add(4);
            qid.Add(3);
            qid.Add(2);
            qid.Add(1);
            qid.Add(9);
            qid.Add(5);
            qid.Add(8);

            return qid;
        }

        public static List<IHierarchy> MakeAdultHierarchies()
        {
            List<IHierarchy> hierarchies = new List<IHierarchy>();

            GeneralizationHierarchy sex = new GeneralizationHierarchy(7);

            Dictionary<string, string> sexDictionary = new Dictionary<string, string>();
            sexDictionary.Add("Male", "*");
            sexDictionary.Add("Female", "*");
            sex.AddDictionary(sexDictionary);

            hierarchies.Add(sex);

            GeneralizationHierarchy race = new GeneralizationHierarchy(6);

            Dictionary<string, string> raceDictionary = new Dictionary<string, string>();
            raceDictionary.Add("White", "*");
            raceDictionary.Add("Asian-Pac-Islander", "*");
            raceDictionary.Add("Amer-Indian-Eskimo", "*");
            raceDictionary.Add("Other", "*");
            raceDictionary.Add("Black", "*");
            race.AddDictionary(raceDictionary);

            hierarchies.Add(race);
            GeneralizationHierarchy workClass = new GeneralizationHierarchy(2);

            Dictionary<string, string> workClassDictionary1 = new Dictionary<string, string>();
            workClassDictionary1.Add("Private", "Non-Government");
            workClassDictionary1.Add("Priv-house-serv", "Non-Government");
            workClassDictionary1.Add("Self-emp-not-inc", "Non-Government");
            workClassDictionary1.Add("Self-emp-inc", "Non-Government");
            workClassDictionary1.Add("Federal-gov", "Government");
            workClassDictionary1.Add("Local-gov", "Government");
            workClassDictionary1.Add("State-gov", "Government");
            workClassDictionary1.Add("Without-pay", "Unemployed");
            workClassDictionary1.Add("Never-worked", "Unemployed");
            workClass.AddDictionary(workClassDictionary1);

            Dictionary<string, string> workClassDictionary2 = new Dictionary<string, string>();
            workClassDictionary2.Add("Private", "*");
            workClassDictionary2.Add("Priv-house-serv", "*");
            workClassDictionary2.Add("Self-emp-not-inc", "*");
            workClassDictionary2.Add("Self-emp-inc", "*");
            workClassDictionary2.Add("Federal-gov", "*");
            workClassDictionary2.Add("Local-gov", "*");
            workClassDictionary2.Add("State-gov", "*");
            workClassDictionary2.Add("Without-pay", "*");
            workClassDictionary2.Add("Never-worked", "*");
            workClass.AddDictionary(workClassDictionary2);

            hierarchies.Add(workClass);

            GeneralizationHierarchy salary = new GeneralizationHierarchy(9);

            Dictionary<string, string> salaryDictionary = new Dictionary<string, string>();
            salaryDictionary.Add(">50K", "*");
            salaryDictionary.Add("<=50K", "*");
            salary.AddDictionary(salaryDictionary);

            hierarchies.Add(salary);

            GeneralizationHierarchy martialStatus = new GeneralizationHierarchy(4);

            Dictionary<string, string> martialStatusDictionary1 = new Dictionary<string, string>();
            martialStatusDictionary1.Add("Married-civ-spouse", "spouse present");
            martialStatusDictionary1.Add("Divorced", "spouse not present");
            martialStatusDictionary1.Add("Never-married", "spouse not present");
            martialStatusDictionary1.Add("Separated", "spouse not present");
            martialStatusDictionary1.Add("Widowed", "spouse not present");
            martialStatusDictionary1.Add("Married-spouse-absent", "spouse not present");
            martialStatusDictionary1.Add("Married-AF-spouse", "spouse present");
            martialStatus.AddDictionary(martialStatusDictionary1);

            Dictionary<string, string> martialStatusDictionary2 = new Dictionary<string, string>();
            martialStatusDictionary2.Add("Married-civ-spouse", "*");
            martialStatusDictionary2.Add("Divorced", "*");
            martialStatusDictionary2.Add("Never-married", "*");
            martialStatusDictionary2.Add("Separated", "*");
            martialStatusDictionary2.Add("Widowed", "*");
            martialStatusDictionary2.Add("Married-spouse-absent", "*");
            martialStatusDictionary2.Add("Married-AF-spouse", "*");
            martialStatus.AddDictionary(martialStatusDictionary2);


            hierarchies.Add(martialStatus);

            GeneralizationHierarchy education = new GeneralizationHierarchy(3);

            Dictionary<string, string> educationDictionary1 = new Dictionary<string, string>();
            educationDictionary1.Add("Bachelors", "Undergraduate");
            educationDictionary1.Add("Some-college", "Undergraduate");
            educationDictionary1.Add("11th", "High School");
            educationDictionary1.Add("HS-grad", "High School");
            educationDictionary1.Add("Prof-school", "Professional Education");
            educationDictionary1.Add("Assoc-acdm", "Professional Education");
            educationDictionary1.Add("Assoc-voc", "Professional Education");
            educationDictionary1.Add("9th", "High School");
            educationDictionary1.Add("7th-8th", "High School");
            educationDictionary1.Add("12th", "High School");
            educationDictionary1.Add("Masters", "Graduate");
            educationDictionary1.Add("1st-4th", "Primary School");
            educationDictionary1.Add("10th", "High School");
            educationDictionary1.Add("Doctorate", "Graduate");
            educationDictionary1.Add("5th-6th", "Primary School");
            educationDictionary1.Add("Preschool", "Primary School");

            education.AddDictionary(educationDictionary1);

            Dictionary<string, string> educationDictionary2 = new Dictionary<string, string>();
            educationDictionary2.Add("Bachelors", "Higher education");
            educationDictionary2.Add("Some-college", "Higher education");
            educationDictionary2.Add("11th", "Secondary education");
            educationDictionary2.Add("HS-grad", "Secondary education");
            educationDictionary2.Add("Prof-school", "Higher education");
            educationDictionary2.Add("Assoc-acdm", "Higher education");
            educationDictionary2.Add("Assoc-voc", "Higher education");
            educationDictionary2.Add("9th", "Secondary education");
            educationDictionary2.Add("7th-8th", "Secondary education");
            educationDictionary2.Add("12th", "Secondary education");
            educationDictionary2.Add("Masters", "Graduate");
            educationDictionary2.Add("1st-4th", "Primary education");
            educationDictionary2.Add("10th", "Secondary education");
            educationDictionary2.Add("Doctorate", "Higher education");
            educationDictionary2.Add("5th-6th", "Primary education");
            educationDictionary2.Add("Preschool", "Primary education");

            education.AddDictionary(educationDictionary2);

            Dictionary<string, string> educationDictionary3 = new Dictionary<string, string>();
            educationDictionary3.Add("Bachelors", "*");
            educationDictionary3.Add("Some-college", "*");
            educationDictionary3.Add("11th", "*");
            educationDictionary3.Add("HS-grad", "*");
            educationDictionary3.Add("Prof-school", "*");
            educationDictionary3.Add("Assoc-acdm", "*");
            educationDictionary3.Add("Assoc-voc", "*");
            educationDictionary3.Add("9th", "*");
            educationDictionary3.Add("7th-8th", "*");
            educationDictionary3.Add("12th", "*");
            educationDictionary3.Add("Masters", "*");
            educationDictionary3.Add("1st-4th", "*");
            educationDictionary3.Add("10th", "*");
            educationDictionary3.Add("Doctorate", "*");
            educationDictionary3.Add("5th-6th", "*");
            educationDictionary3.Add("Preschool", "*");

            education.AddDictionary(educationDictionary3);

            hierarchies.Add(education);

            NumericGeneralizationHierarchy age = new NumericGeneralizationHierarchy(1);

            Dictionary<int, string> ageDictionary1 = new Dictionary<int, string>();

            ageDictionary1.Add(0, "0-20");
            ageDictionary1.Add(20, "20-25");
            ageDictionary1.Add(25, "26-30");
            ageDictionary1.Add(30, "31-35");
            ageDictionary1.Add(35, "36-40");
            ageDictionary1.Add(40, "41-45");
            ageDictionary1.Add(45, "46-50");
            ageDictionary1.Add(50, "51-55");
            ageDictionary1.Add(55, "56-60");
            ageDictionary1.Add(60, "61-65");
            ageDictionary1.Add(65, "66-70");
            ageDictionary1.Add(70, "71-75");
            ageDictionary1.Add(75, "76-80");
            ageDictionary1.Add(80, "81-85");
            ageDictionary1.Add(85, "86-120");
            ageDictionary1.Add(1000, "1000 -");

            age.AddDictionary(ageDictionary1);

            Dictionary<int, string> ageDictionary2 = new Dictionary<int, string>();

            ageDictionary2.Add(0, "0-20");
            ageDictionary2.Add(20, "21-30");
            ageDictionary2.Add(30, "31-40");
            ageDictionary2.Add(40, "41-50");
            ageDictionary2.Add(50, "51-60");
            ageDictionary2.Add(60, "61-70");
            ageDictionary2.Add(70, "71-80");
            ageDictionary2.Add(80, "81-120");
            ageDictionary2.Add(1000, "1000 -");

            age.AddDictionary(ageDictionary2);

            Dictionary<int, string> level2Dictionary = new Dictionary<int, string>();

            level2Dictionary.Add(120, "0-120");

            age.AddDictionary(level2Dictionary);

            hierarchies.Add(age);

            GeneralizationHierarchy occupation = new GeneralizationHierarchy(5);

            Dictionary<string, string> occupationDictionary1 = new Dictionary<string, string>();
            occupationDictionary1.Add("Tech-support", "Technical");
            occupationDictionary1.Add("Craft-repair", "Technical");
            occupationDictionary1.Add("Other-service", "Other");
            occupationDictionary1.Add("Sales", "Nontechnical");
            occupationDictionary1.Add("Exec-managerial", "Nontechnical");
            occupationDictionary1.Add("Prof-specialty", "Technical");
            occupationDictionary1.Add("Handlers-cleaners", "Nontechnical");
            occupationDictionary1.Add("Priv-house-serv", "Nontechnical");
            occupationDictionary1.Add("Machine-op-inspct", "Technical");
            occupationDictionary1.Add("Adm-clerical", "Other");
            occupationDictionary1.Add("Farming-fishing", "Other");
            occupationDictionary1.Add("Transport-moving", "Other");
            occupationDictionary1.Add("Priv-house-servr", "Other");
            occupationDictionary1.Add("Protective-serv", "Other");
            occupationDictionary1.Add("Armed-Forces", "Other");

            occupation.AddDictionary(occupationDictionary1);

            Dictionary<string, string> occupationDictionary2 = new Dictionary<string, string>();
            occupationDictionary2.Add("Tech-support", "*");
            occupationDictionary2.Add("Craft-repair", "*");
            occupationDictionary2.Add("Other-service", "*");
            occupationDictionary2.Add("Sales", "*");
            occupationDictionary2.Add("Exec-managerial", "*");
            occupationDictionary2.Add("Prof-specialty", "*");
            occupationDictionary2.Add("Handlers-cleaners", "*");
            occupationDictionary2.Add("Machine-op-inspct", "*");
            occupationDictionary2.Add("Adm-clerical", "*");
            occupationDictionary2.Add("Farming-fishing", "*");
            occupationDictionary2.Add("Transport-moving", "*");
            occupationDictionary2.Add("Priv-house-servr", "*");
            occupationDictionary2.Add("Protective-serv", "*");
            occupationDictionary2.Add("Armed-Forces", "*");
            occupationDictionary2.Add("Priv-house-serv", "*");

            occupation.AddDictionary(occupationDictionary2);
            hierarchies.Add(occupation);

            GeneralizationHierarchy nativeCountry = new GeneralizationHierarchy(8);

            Dictionary<string, string> nativeCountryDictionary1 = new Dictionary<string, string>();
            nativeCountryDictionary1.Add("United-States", "North America");
            nativeCountryDictionary1.Add("Cambodia", "Asia");
            nativeCountryDictionary1.Add("England", "Europe");
            nativeCountryDictionary1.Add("Puerto-Rico", "North America");
            nativeCountryDictionary1.Add("Canada", "North America");
            nativeCountryDictionary1.Add("Germany", "Europe");
            nativeCountryDictionary1.Add("Outlying-US(Guam-USVI-etc)", "North America");
            nativeCountryDictionary1.Add("India", "Asia");
            nativeCountryDictionary1.Add("Japan", "Asia");
            nativeCountryDictionary1.Add("Greece", "Europe");
            nativeCountryDictionary1.Add("South", "Africa");
            nativeCountryDictionary1.Add("China", "Asia");
            nativeCountryDictionary1.Add("Cuba", "North America");
            nativeCountryDictionary1.Add("Iran", "Asia");
            nativeCountryDictionary1.Add("Honduras", "North America");
            nativeCountryDictionary1.Add("Philippines", "Asia");
            nativeCountryDictionary1.Add("Italy", "Europe");
            nativeCountryDictionary1.Add("Poland", "Europe");
            nativeCountryDictionary1.Add("Jamaica", "North America");
            nativeCountryDictionary1.Add("Vietnam", "Asia");
            nativeCountryDictionary1.Add("Mexico", "North America");
            nativeCountryDictionary1.Add("Portugal", "Europe");
            nativeCountryDictionary1.Add("Ireland", "Europe");
            nativeCountryDictionary1.Add("France", "Europe");
            nativeCountryDictionary1.Add("Dominican-Republic", "North America");
            nativeCountryDictionary1.Add("Laos", "Asia");
            nativeCountryDictionary1.Add("Ecuador", "South America");
            nativeCountryDictionary1.Add("Taiwan", "Asia");
            nativeCountryDictionary1.Add("Haiti", "North America");
            nativeCountryDictionary1.Add("Columbia", "South America");
            nativeCountryDictionary1.Add("Hungary", "Europe");
            nativeCountryDictionary1.Add("Guatemala", "North America");
            nativeCountryDictionary1.Add("Nicaragua", "South America");
            nativeCountryDictionary1.Add("Scotland", "Europe");
            nativeCountryDictionary1.Add("Thailand", "Asia");
            nativeCountryDictionary1.Add("Yugoslavia", "Europe");
            nativeCountryDictionary1.Add("El-Salvador", "North America");
            nativeCountryDictionary1.Add("Trinadad&Tobago", "South America");
            nativeCountryDictionary1.Add("Peru", "South America");
            nativeCountryDictionary1.Add("Hong", "Asia");
            nativeCountryDictionary1.Add("Holand-Netherlands", "Europe");
            nativeCountry.AddDictionary(nativeCountryDictionary1);

            Dictionary<string, string> nativeCountryDictionary2 = new Dictionary<string, string>();
            nativeCountryDictionary2.Add("United-States", "*");
            nativeCountryDictionary2.Add("Cambodia", "*");
            nativeCountryDictionary2.Add("England", "*");
            nativeCountryDictionary2.Add("Puerto-Rico", "*");
            nativeCountryDictionary2.Add("Canada", "*");
            nativeCountryDictionary2.Add("Germany", "*");
            nativeCountryDictionary2.Add("Outlying-US(Guam-USVI-etc)", "*");
            nativeCountryDictionary2.Add("India", "*");
            nativeCountryDictionary2.Add("Japan", "*");
            nativeCountryDictionary2.Add("Greece", "*");
            nativeCountryDictionary2.Add("South", "*");
            nativeCountryDictionary2.Add("China", "*");
            nativeCountryDictionary2.Add("Cuba", "*");
            nativeCountryDictionary2.Add("Iran", "*");
            nativeCountryDictionary2.Add("Honduras", "*");
            nativeCountryDictionary2.Add("Philippines", "*");
            nativeCountryDictionary2.Add("Italy", "*");
            nativeCountryDictionary2.Add("Poland", "*");
            nativeCountryDictionary2.Add("Jamaica", "*");
            nativeCountryDictionary2.Add("Vietnam", "*");
            nativeCountryDictionary2.Add("Mexico", "*");
            nativeCountryDictionary2.Add("Portugal", "*");
            nativeCountryDictionary2.Add("Ireland", "*");
            nativeCountryDictionary2.Add("France", "*");
            nativeCountryDictionary2.Add("Dominican-Republic", "*");
            nativeCountryDictionary2.Add("Laos", "*");
            nativeCountryDictionary2.Add("Ecuador", "*");
            nativeCountryDictionary2.Add("Taiwan", "*");
            nativeCountryDictionary2.Add("Haiti", "*");
            nativeCountryDictionary2.Add("Columbia", "*");
            nativeCountryDictionary2.Add("Hungary", "*");
            nativeCountryDictionary2.Add("Guatemala", "*");
            nativeCountryDictionary2.Add("Nicaragua", "*");
            nativeCountryDictionary2.Add("Scotland", "*");
            nativeCountryDictionary2.Add("Thailand", "*");
            nativeCountryDictionary2.Add("Yugoslavia", "*");
            nativeCountryDictionary2.Add("El-Salvador", "*");
            nativeCountryDictionary2.Add("Trinadad&Tobago", "*");
            nativeCountryDictionary2.Add("Peru", "*");
            nativeCountryDictionary2.Add("Hong", "*");
            nativeCountryDictionary2.Add("Holand-Netherlands", "*");
            nativeCountry.AddDictionary(nativeCountryDictionary2);

            hierarchies.Add(nativeCountry);

            return hierarchies;
        }


        public static DataTable LoadCsv(string path)
        {
            string CSVFilePathName = path;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ';' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ';' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }
            return dt;
        }

        public static void WriteCsvFile(DataTable dt, string path)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(";", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(";", fields));
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
```
