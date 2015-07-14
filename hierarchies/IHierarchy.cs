using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.hierarchies
{
    public interface IHierarchy
    {

        /// <returns>Returns the index of the QI that belongs to the hierarchy.</returns>
        int GetQid();
        void SetQid(int qid);

        /// <returns>Returns the current level of the hierarchy.</returns>
        int GetLevel();
        void SetLevel(int level);

        /// <returns>Returns the number of levels that the hierarchy has.</returns>
        int GetDepth();

        /// <summary>
        /// Generalizes the input values according to the current level of the hierarchy.
        /// </summary>
        /// <returns>Returns an array of strings that contains the generalized versions of the input values.</returns>
        string[] Generalize(string[] values);

    }
}
