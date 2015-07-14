using AnonymizationLibrary.hierarchies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.data
{

    public class Tuple
    {

        private string[] values;
        private List<int> qid;


        public Tuple()
        {
            values = new string[0];
        }

        public Tuple(string[] values)
        {
            SetValues(values);
            qid = new List<int>();
        }

        public Tuple(string[] values, List<int> qid)
        {
            SetValues(values);
            this.qid = qid;
        }

        public void SetQid(List<int> qid)
        {
            this.qid = qid;
        }

        public string[] GetValues()
        {
            return this.values;
        }

        public void SetValues(string[] values)
        {
            this.values = values;
            this.values = new String[values.Count()];
            for (int i = 0; i < this.values.Count(); i++)
                this.values[i] = values[i].Trim();
        }

        public string GetValue(int dimension)
        {
            return this.values[dimension];
        }

        public void SetValue(int dimension, string value)
        {
            this.values[dimension] = value;
        }


        public override string ToString()
        {
            String buffer = "";
            int i;
            for (i = 0; i < this.values.Count() - 1; i++)
                buffer += this.values[i] + ";";
            buffer += this.values[i];
            return buffer;
        }


        public bool Equals(Tuple other, int[] qid)
        {
            foreach (int d in qid)
            {
                if (!this.GetValue(d).Equals(other.GetValue(d)))
                    return false;
            }
            return true;
        }

        public override bool Equals(Object other)
        {
            Tuple otherTuple = (Tuple)other;
            foreach (int d in qid)
            {
                if (!this.GetValue(d).Equals(otherTuple.GetValue(d)))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Generalizes the values of the Tuple with the help of domain generalization hierarchies.
        /// </summary>
        /// <param name="hierarchies">domain generalization hierarchies</param>
        /// <returns>Returns an array of generalized version of the values of the tuple.</returns>
        public string[] Generalize(List<IHierarchy> hierarchies)
        {
            //copy the array
            string[] anonymizedTuple = new string[values.Count()];

            for (int i = 0; i < this.values.Count(); i++) anonymizedTuple[i] = this.values[i];

            foreach (hierarchies.IHierarchy hierarchy in hierarchies)
            {
                anonymizedTuple = hierarchy.Generalize(anonymizedTuple);
            }
            return anonymizedTuple;
        }

        public int GetSimpleHash(int[] qid)
        {
            int hash = 0;
            foreach (int d in this.qid)
            {
                hash += values[d].GetHashCode();
            }
            return hash;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (int d in this.qid)
                {
                    hash = hash * 31 + values[d].GetHashCode();
                }
                return hash;
            }
        }


        /// <summary>
        /// aAdd an additional value to the values array.
        /// </summary>
        public void AddValue(string value)
        {
            string[] temp = new string[values.Count() + 1];
            for (int i = 0; i < values.Count(); i++)
            {
                temp[i] = values[i];
            }
            temp[values.Count()] = value;
            values = temp;
        }




        public void RemoveAttribute(int dimension)
        {
            string[] temp = new string[values.Count() - 1];
            for (int i = 0; i < values.Count(); i++)
            {
                if (i < dimension)
                    temp[i] = values[i];
                else if (i > dimension)
                    temp[i - 1] = values[i];
            }
            values = temp;
        }

        public int GetNumberOfAttributes()
        {
            return this.values.Count();
        }

    }
}
