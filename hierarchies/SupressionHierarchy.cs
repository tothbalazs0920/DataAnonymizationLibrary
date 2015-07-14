﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationLibrary.hierarchies
{
    public class SupressionHierarchy : IHierarchy
    {

        private int qid;
        private int level;
        private int depth;
        
        /// <param name="qid">QI indecies</param>
        /// <param name="depth">Limits how much supression are allowed in total.</param>
        public SupressionHierarchy(int qid, int depth)
        {
            this.qid = qid;
            this.depth = depth + 1; // +1 because of the level 0 - the other hierarchies count the elements in the lists
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

        public int GetDepth()
        {
            return depth;
        }

        public string[] Generalize(string[] values)
        {
            if (this.level == 0) return values;
            StringBuilder builder = new StringBuilder(values[qid]);
            for (int k = 0; k < this.level; k++)
                builder[builder.Length - (k + 1)] = '*';
            
            values[qid] = builder.ToString();
            return values;
        }


    }
}
