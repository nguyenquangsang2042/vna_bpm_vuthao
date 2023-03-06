
using System;
using Foundation;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;

class Custom_TableTaskHelper
{
    private static Custom_TableTaskHelper _instance;
    public List<BeanTaskDetail> lst_tasks;
    long defaultExpandLevel;
    public static Custom_TableTaskHelper Instance()
    {
        if (_instance == null)
        {
            _instance = new Custom_TableTaskHelper();
        }
        return _instance;
    }

    public List<BeanTaskDetail> getSortedNodes(long _defaultExpandLevel, List<BeanTaskDetail> _lst_tasks, bool isfirst = false)
    {
        lst_tasks = _lst_tasks;
        defaultExpandLevel = _defaultExpandLevel;
        List<BeanTaskDetail> result = new List<BeanTaskDetail> { };
        var nodes = convetData2Node();// (defaultExpandLevel, isfirst);
        var rootNodes = getRootNodes(nodes);
        foreach (var item in rootNodes)
        {
            addNode(ref result, node: item, currentLevel: 1);
        }
        return result;
    }
    List<BeanTaskDetail> getRootNodes(List<BeanTaskDetail> nodes)
    {
        List<BeanTaskDetail> root = new List<BeanTaskDetail> { };
        foreach (var item in nodes)
        {
            if (item.isRoot(defaultExpandLevel))
            {
                root.Add(item);
            }
        }
        return root;
    }

    public List<BeanTaskDetail> getSortedNodes1(long _defaultExpandLevel,List<BeanTaskDetail> _lst_tasks)
    {
        lst_tasks = _lst_tasks;
        defaultExpandLevel = _defaultExpandLevel;
        var nodes = convetData2Node();// (defaultExpandLevel, false);
        return nodes;
    }
    //lấy những item hợp lệ :có parent là null hoặc parent.isexpand == true
    public List<BeanTaskDetail> filterVisibleNode(List<BeanTaskDetail> nodes, long _defaultExpandLevel)
    {
        defaultExpandLevel = _defaultExpandLevel;
        lst_tasks = nodes;
        List<BeanTaskDetail> result = new List<BeanTaskDetail> { };
        foreach (var item in nodes)
        {
            var parentNode = lst_tasks.Find(s => s.ID == item.Parent);
            if (item.isRoot(defaultExpandLevel) || item.isParentExpand(parentNode))
            {
                //setNodeIcon(item);
                result.Add(item);
            }
        }
        return result;
    }

    List<BeanTaskDetail> convetData2Node()
    {
        BeanTaskDetail n;
        BeanTaskDetail m;

        for (int i = 0; i < lst_tasks.Count; i++)
        {
            n = lst_tasks[i];
            for (int j = i + 1; j < lst_tasks.Count; j++)
            {
                m = lst_tasks[j];
                if (m.Parent == n.ID)
                {
                    n.isExpand = true;
                    n.children.Add(m);
                }
                else if (n.Parent == m.ID)
                {
                    m.isExpand = true;
                    m.children.Add(n);
                }
            }
        }
        return lst_tasks;
    }
    //sắp xếp lại cây 
    void addNode(ref List<BeanTaskDetail> nodes, BeanTaskDetail node, int currentLevel)
    {
        nodes.Add(node);
        if (node.isLeaf())
        {
            return;
        }
        for (int i = 0; i < node.children.Count; i++)
        {
            addNode(ref nodes, node.children[i], currentLevel + 1);
        }
    }
   
}
