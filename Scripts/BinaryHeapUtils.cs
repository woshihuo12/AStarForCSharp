using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 最小二叉堆
/// </summary>
public class BinaryHeapUtils
{
    /// <summary>
    /// 数组,用于保持树的平衡
    /// </summary>
    public IList<BinaryHeapNode> nodes;

    /// <summary>
    /// 数组中正在使用的元素数目
    /// </summary>
    private int nodeLength;

    /// <summary>
    /// 头节点
    /// </summary>
    public BinaryHeapNode headNode;

    /// <summary>
    /// 节点对象池(缓存节点) 
    /// </summary>
    private IList<BinaryHeapNode> cacheNodes = new List<BinaryHeapNode>();

    /// <summary>
    /// 获得一个节点
    /// </summary>
    /// <returns>The node.</returns>
    /// <param name="data">Data.</param>
    /// <param name="parentNode">Parent node.</param>
    private BinaryHeapNode GetNode(AStarNode data, BinaryHeapNode parentNode)
    {
        BinaryHeapNode binaryHeapNode = null;

        if (this.cacheNodes.Count > 0)
        {
            binaryHeapNode = this.cacheNodes[this.cacheNodes.Count - 1];

            binaryHeapNode.data = data;
            binaryHeapNode.parentNode = parentNode;

            this.cacheNodes.RemoveAt(this.cacheNodes.Count - 1);
        }
        else
        {
            binaryHeapNode = new BinaryHeapNode(data, parentNode);
        }
        return binaryHeapNode;
    }

    /// <summary>
    /// 存储节点
    /// </summary>
    /// <param name="node">Node.</param>
    private void CacheNode(BinaryHeapNode node)
    {
        node.parentNode = node.leftNode = node.rightNode = null;
        node.data = null;

        this.cacheNodes.Add(node);
    }

    /// <summary>
    /// 向下修正节点(向树叶方向修正节点)
    /// </summary>
    /// <returns>The to leaf.</returns>
    /// <param name="node">Node.</param>
    private BinaryHeapNode ModifyToLeaf(BinaryHeapNode node)
    {
        AStarNode currentNodeData = node.data;
        int currentNodeValue = currentNodeData.f;

        BinaryHeapNode leftNode = null;
        BinaryHeapNode rightNode = null;

        while (true)
        {
            leftNode = node.leftNode;
            rightNode = node.rightNode;

            if (rightNode != null && leftNode != null && rightNode.data.f < leftNode.data.f)
            {
                if (currentNodeValue > rightNode.data.f)
                {
                    node.data = rightNode.data;
                    node.data.binaryHeapNode = node;
                    node = rightNode;
                }
                else
                {
                    break;
                }
            }
            else if (leftNode != null && leftNode.data.f < currentNodeValue)
            {
                node.data = leftNode.data;
                node.data.binaryHeapNode = node;
                node = leftNode;
            }
            else
            {
                break;
            }
        }
        node.data = currentNodeData;
        node.data.binaryHeapNode = node;

        return node;
    }

    /// <summary>
    /// 向上修正节点(向树根方向修正节点)
    /// </summary>
    /// <returns>The to root.</returns>
    /// <param name="node">Node.</param>
    private BinaryHeapNode ModifyToRoot(BinaryHeapNode node)
    {
        AStarNode currentNodeData = node.data;
        int currentNodeValue = currentNodeData.f;

        BinaryHeapNode parentNode = node.parentNode;
        while (parentNode != null)
        {
            if (currentNodeValue < parentNode.data.f)
            {
                node.data = parentNode.data;
                node.data.binaryHeapNode = node;

                node = node.parentNode;
                parentNode = node.parentNode;
            }
            else
            {
                break;
            }
        }
        node.data = currentNodeData;
        node.data.binaryHeapNode = node;

        return node;
    }

    /// <summary>
    /// 修正节点
    /// </summary>
    /// <returns>The node.</returns>
    /// <param name="node">Node.</param>
    public BinaryHeapNode ModifyNode(BinaryHeapNode node)
    {
        if (node.parentNode != null && node.parentNode.data.f > node.data.f)
        {
            return this.ModifyToRoot(node);
        }
        else
        {
            return this.ModifyToLeaf(node);
        }
    }

    /// <summary>
    /// 添加新节点
    /// </summary>
    /// <returns>The node.</returns>
    /// <param name="data">Data.</param>
    public BinaryHeapNode InsertNode(AStarNode data)
    {
        if (this.headNode != null)
        {
            BinaryHeapNode parentNode = this.nodes[this.nodeLength >> 1];
            BinaryHeapNode node = this.GetNode(data, parentNode);
            node.data.binaryHeapNode = node;

            if (parentNode.leftNode == null)
            {
                parentNode.leftNode = node;
            }
            else
            {
                parentNode.rightNode = node;
            }
            this.nodes[this.nodeLength] = node;
            this.nodeLength++;
            return this.ModifyToRoot(node);
        }
        else
        {
            this.nodes[1] = this.headNode = this.GetNode(data, null);
            this.nodes.Add(this.headNode);
            this.headNode.data.binaryHeapNode = this.headNode;

            this.nodeLength = 2;
            return this.headNode;
        }
    }

    /// <summary>
    /// 取出最小值
    /// </summary>
    /// <returns>The node.</returns>
    public AStarNode PopNode()
    {
        AStarNode minValue = this.headNode.data;

        BinaryHeapNode lastNode = this.nodes[--this.nodeLength];

        if (lastNode != this.headNode)
        {
            BinaryHeapNode parentNode = lastNode.parentNode;
            if (parentNode.leftNode == lastNode)
            {
                parentNode.leftNode = null;
            }
            else
            {
                parentNode.rightNode = null;
            }
            this.headNode.data = lastNode.data;
            this.headNode.data.binaryHeapNode = this.headNode;

            this.ModifyToLeaf(this.headNode);
        }
        else
        {
            this.headNode = null;
        }
        this.CacheNode(this.nodes[this.nodeLength]);
        this.nodes[this.nodeLength] = null;

        return minValue;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        for (int index = 1; index < this.nodeLength; index++)
        {
            this.CacheNode(this.nodes[index]);
            this.nodes[index] = null;
        }
        this.nodeLength = 1;
        this.headNode = null;
    }

    // 小二叉堆
    public BinaryHeapUtils(int cacheSize)
    {
        this.nodes = new List<BinaryHeapNode>(cacheSize);
        for (int index = 0; index < cacheSize; index++)
        {
            this.nodes.Add(null);
            this.cacheNodes.Add(new BinaryHeapNode(null, null));
        }
    }
}
