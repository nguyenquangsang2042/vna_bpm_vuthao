using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanTreeNode
    {
        public string Id { get; set; }
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public int NodeLevel { get; set; }
        public string FilePath { get; set; }
        public List<BeanTreeNode> Children { get; set; }
        public BeanTreeNode Parent { get; set; }
        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }
        public BeanTreeNode()
        {
            Children = new List<BeanTreeNode>();
        }
    }
}
