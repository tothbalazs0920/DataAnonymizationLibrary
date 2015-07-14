using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.data
{

    /// <summary>
    /// This class represents a generalization vertice/node in the generalization graph.
    /// </summary>
    public class Node : IComparable
    {
        //list of generalization levels corresponding to each dimensions
        public List<int> generalizations { get; set; }
        public int id { get; set; }
        public bool visited { get; set; }

        public Node()
        {
            visited = false;
            generalizations = new List<int>();
        }

        public override string ToString()
        {
            string result = id + " {";
            foreach (int i in generalizations)
            {
                result += i + " ";

            }
            result += "}";

            return result;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Node otherNode = obj as Node;
            if (otherNode != null)
            {
                int sum = 0;
                foreach (int i in this.generalizations)
                {
                    sum += i;
                }

                int otherSum = 0;
                foreach (int i in otherNode.generalizations)
                {
                    otherSum += i;
                }
                if (sum == otherSum) return 0;
                else if (sum < otherSum) return -1;
                else return 1;
            }
            else
                throw new ArgumentException("Object is not a Node");
        }


        public int SumOfLevels()
        {
            int sum = 0;
            foreach (int i in generalizations)
                sum += i;
            return sum;
        }

    }
}
