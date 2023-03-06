using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class TreeNode
    {
        public string Id { get; set; }
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public int NodeLevel { get; set; }
        public string FilePath { get; set; }
        public List<TreeNode> Children { get; set; }
        public TreeNode Parent { get; set; }
        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }
        public TreeNode()
        {
            Children = new List<TreeNode>();
        }
    }
}