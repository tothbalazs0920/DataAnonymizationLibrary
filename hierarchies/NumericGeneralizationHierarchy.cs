using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.hierarchies
{
    public class NumericGeneralizationHierarchy : IHierarchy
    {
        private int qid;
        List<Dictionary<int, string>> dictionaryList;
        private int level;

        public NumericGeneralizationHierarchy(int qid)
        {
            this.qid = qid;
            dictionaryList = new List<Dictionary<int, string>>();
            //add 0 level hierarchy
            dictionaryList.Add(new Dictionary<int, string>());
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }


        public int GetQid()
        {
            return qid;
        }

        public void SetQid(int qid)
        {
            this.qid = qid;
        }

        public void AddDictionary(Dictionary<int, string> dictionary)
        {
            dictionaryList.Add(dictionary);
        }

        public int GetDepth()
        {
            return dictionaryList.Count;
        }

        public string[] Generalize(string[] values)
        {
            if (this.level == 0) return values;

            // find the current dictionary
            Dictionary<int, string> dictionary = dictionaryList[level];

            int[] keys = dictionary.Keys.ToArray();

            //get the actual value of the tuple with the right index
            int integerKey = Int32.Parse(values[qid]);

            //the upperbound are given in the keys 
            if (integerKey < keys[0])
            {
                values[qid] = dictionary[keys[0]]; // here is the generalization -first entry of the hierarchy
                return values;
            }

            for (int j = 0; j < keys.Length - 1; j++)
            {
                if (integerKey > keys[j] && integerKey <= keys[j + 1]) values[qid] = dictionary[keys[j]]; // here is the anoymization
            }
            return values;
        }

    }
}
