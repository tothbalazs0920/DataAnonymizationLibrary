using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.hierarchies
{
    public class GeneralizationHierarchy : IHierarchy
    {
        private int qid;
        List<Dictionary<string, string>> dictionaryList;
        private int level;

        public GeneralizationHierarchy(int qid)
        {
            this.qid = qid;
            dictionaryList = new List<Dictionary<string, string>>();
            //add 0 level hierarchy
            dictionaryList.Add(new Dictionary<string, string>());
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
      

        public void AddDictionary(Dictionary<string, string> dictionary)
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
            Dictionary<string, string> dictionary = dictionaryList[level];

            // if it has the key, generalize it
            if (dictionary.ContainsKey(values[qid]))
            {
                values[qid] = dictionary[values[qid]]; // here is the generalization
                return values;
            }
            return values;
        }

    }
}
