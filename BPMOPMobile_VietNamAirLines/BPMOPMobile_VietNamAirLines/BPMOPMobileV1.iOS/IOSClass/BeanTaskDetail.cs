using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using SQLite;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class BeanTaskDetail : BeanTask
    {
        public BeanTaskDetail()
        {
           
        }
        //thuyngo add
        public bool isExpand = false;
        public int index = 0;
        public int session = -1;
        public bool isRoorFinal = false;
        [Ignore]
        public int type { get; set; }
        //[Ignore]
        //public string icon { get; set; }
        public List<BeanTaskDetail> children = new List<BeanTaskDetail> { };


        public bool isRoot(long defaultExpandLevel)
        {
            return Parent == defaultExpandLevel;
        }

        public bool isParentExpand(BeanTaskDetail ParentNode)
        {
            if (ParentNode == null)
            {
                return false;
            }
            return (ParentNode.isExpand);
        }

        public bool isLeaf()
        {
            return children.Count == 0;
        }

        //public int getLevel()
        //{
        //    var result = BPMOPMobileV1.iOS.ViewControllers.RequestDetailsV2.lst_tasks.Find(s => s.ID == Parent);
        //    return result == null ? 0 : (result.getLevel()) + 1;
        //}

        public void setExpand(bool isExpand)
        {
            this.isExpand = isExpand;
            if (!isExpand)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].setExpand(isExpand);
                }
            }
        }
    }
}
