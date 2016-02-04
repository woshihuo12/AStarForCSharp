using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SimpleAStarPathFind
{
    /// <summary>
    /// 二叉堆节点
    /// </summary>
    public class BinaryHeapNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public BinaryHeapNode parentNode;
        /// <summary>
        /// 左子节点
        /// </summary>
        public BinaryHeapNode leftNode;
        /// <summary>
        /// 右子节点
        /// </summary>
        public BinaryHeapNode rightNode;
        /// <summary>
        /// 节点数据
        /// </summary>
        public SimpleAStarNode data;
        public BinaryHeapNode(SimpleAStarNode data, BinaryHeapNode parentNode)
        {
            this.data = data;
            this.parentNode = parentNode;
        }
    }

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
        private BinaryHeapNode GetNode(SimpleAStarNode data, BinaryHeapNode parentNode)
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
            SimpleAStarNode currentNodeData = node.data;
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
                        node.data.mBinaryHeapNode = node;
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
                    node.data.mBinaryHeapNode = node;
                    node = leftNode;
                }
                else
                {
                    break;
                }
            }
            node.data = currentNodeData;
            node.data.mBinaryHeapNode = node;

            return node;
        }

        /// <summary>
        /// 向上修正节点(向树根方向修正节点)
        /// </summary>
        /// <returns>The to root.</returns>
        /// <param name="node">Node.</param>
        private BinaryHeapNode ModifyToRoot(BinaryHeapNode node)
        {
            SimpleAStarNode currentNodeData = node.data;
            int currentNodeValue = currentNodeData.f;

            BinaryHeapNode parentNode = node.parentNode;
            while (parentNode != null)
            {
                if (currentNodeValue < parentNode.data.f)
                {
                    node.data = parentNode.data;
                    node.data.mBinaryHeapNode = node;

                    node = node.parentNode;
                    parentNode = node.parentNode;
                }
                else
                {
                    break;
                }
            }
            node.data = currentNodeData;
            node.data.mBinaryHeapNode = node;

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
        public BinaryHeapNode InsertNode(SimpleAStarNode data)
        {
            if (this.headNode != null)
            {
                BinaryHeapNode parentNode = this.nodes[this.nodeLength >> 1];
                BinaryHeapNode node = this.GetNode(data, parentNode);
                node.data.mBinaryHeapNode = node;

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
                //this.nodes.Add(this.headNode);
                this.headNode.data.mBinaryHeapNode = this.headNode;

                this.nodeLength = 2;
                return this.headNode;
            }
        }

        /// <summary>
        /// 取出最小值
        /// </summary>
        /// <returns>The node.</returns>
        public SimpleAStarNode PopNode()
        {
            SimpleAStarNode minValue = this.headNode.data;

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
                this.headNode.data.mBinaryHeapNode = this.headNode;

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

    public interface ISimpleAStarHeuristic
    {
        int Heuristic(int x1, int y1, int x2, int y2);
    }

    public class SimpleAStarManhattanHeuristic : ISimpleAStarHeuristic
    {
        public int Heuristic(int x1, int y1, int x2, int y2)
        {
            return (
                (x1 > x2 ? x1 - x2 : x2 - x1)
                +
                (y1 > y2 ? y1 - y2 : y2 - y1)
                ) * SimpleAStarManager.STRAIGHT_COST;
        }
    }

    public class SimpleAStarDiagonalHeuristic : ISimpleAStarHeuristic
    {
        public int Heuristic(int x1, int y1, int x2, int y2)
        {
            int dx = x1 > x2 ? x1 - x2 : x2 - x1;
            int dy = y1 > y2 ? y1 - y2 : y2 - y1;

            return dx > dy ? SimpleAStarManager.DIAG_COST * dy + SimpleAStarManager.STRAIGHT_COST * (dx - dy) : SimpleAStarManager.DIAG_COST * dx + SimpleAStarManager.STRAIGHT_COST * (dy - dx);
        }
    }

    public interface ISimpleAStarUnit
    {
        void SetWorldPos(int x, int y);
        int GetWorldPosX();
        int GetWorldPosY();
        void SetIsPassable(bool isPassable);
        bool GetIsPassable();
        void SetPassCost(int costNum);
        int GetPassCost();
        void AddWorldPosChangedEvent(System.Action callBack);
        void RemoveWorldPosChangedEvent(System.Action callBack);

        void AddIsPassableChangedEvent(System.Action callBack);
        void RemoveIsPassableChangedEvent(System.Action callBack);

        void AddPassCostChangedEvent(System.Action callBack);
        void RemovePassCostChangedEvent(System.Action callBack);
    }

    public class SimpleAStarUint : ISimpleAStarUnit
    {
        public System.Action OnWorldPosChangedEvent;

        protected int mWorldPosX = -1;
        protected int mWorldPosY = -1;

        public System.Action OnIsPassableChangedEvent;
        protected bool mIsPassable = true;

        public System.Action OnPassCostChangedEvent;
        protected int mPassCost;


        public void SetWorldPos(int x, int y)
        {
            if (mWorldPosX != x || mWorldPosY != y)
            {
                mWorldPosX = x;
                mWorldPosY = y;

                if (OnWorldPosChangedEvent != null)
                {
                    OnWorldPosChangedEvent();
                }
            }
        }

        public int GetWorldPosX()
        {
            return mWorldPosX;
        }

        public int GetWorldPosY()
        {
            return mWorldPosY;
        }

        public void SetIsPassable(bool isPassable)
        {
            if (mIsPassable != isPassable)
            {
                mIsPassable = isPassable;
                if (OnIsPassableChangedEvent != null)
                {
                    OnIsPassableChangedEvent();
                }
            }
        }

        public bool GetIsPassable()
        {
            return mIsPassable;
        }

        public void SetPassCost(int costNum)
        {
            if (mPassCost != costNum)
            {
                mPassCost = costNum;
                if (OnPassCostChangedEvent != null)
                {
                    OnPassCostChangedEvent();
                }
            }
        }

        public int GetPassCost()
        {
            return mPassCost;
        }

        public void AddWorldPosChangedEvent(System.Action callBack)
        {
            OnWorldPosChangedEvent += callBack;
        }
        public void RemoveWorldPosChangedEvent(System.Action callBack)
        {
            OnWorldPosChangedEvent -= callBack;
        }
        public void AddIsPassableChangedEvent(System.Action callBack)
        {
            OnIsPassableChangedEvent += callBack;
        }

        public void RemoveIsPassableChangedEvent(System.Action callBack)
        {
            OnIsPassableChangedEvent -= callBack;
        }

        public void AddPassCostChangedEvent(System.Action callBack)
        {
            OnPassCostChangedEvent += callBack;
        }

        public void RemovePassCostChangedEvent(System.Action callBack)
        {
            OnPassCostChangedEvent -= callBack;
        }
    }

    /// <summary>
    /// 邻节点
    /// </summary>
    public class SimpleAStarLinkNode
    {
        /// <summary>
        /// 节点
        /// </summary>
        public SimpleAStarNode mNode;

        /// <summary>
        /// 花费代价
        /// </summary>
        public int mCost;

        public SimpleAStarLinkNode(SimpleAStarNode node, int cost)
        {
            mNode = node;
            mCost = cost;
        }
    }

    public class SimpleAStarNode
    {
        /// <summary>
        /// 坐标 x
        /// </summary>
        public int mNodeX;

        /// <summary>
        /// 坐标 y
        /// </summary>
        public int mNodeY;

        /// <summary>
        /// 父节点
        /// </summary>
        public SimpleAStarNode mParentNode;

        /// <summary>
        /// 二叉堆节点
        /// </summary>
        public BinaryHeapNode mBinaryHeapNode;

        /// <summary>
        /// 与此节点相邻的可通过的邻节点
        /// </summary>
        public IList<SimpleAStarLinkNode> mLinks;

        /// <summary>
        /// 搜索路径的检查编号(确定是否被检查过)
        /// </summary>
        public int mSearchPathCheckNum;

        /// <summary>
        /// 可移动范围的检查编号(确定是否被检查过)
        /// </summary>
        public int mWalkableRangeCheckNum;

        /// <summary>
        /// 是否能被穿越
        /// </summary>
        public bool mWalkable;

        /**此节点代价系数*/
        public System.Action<SimpleAStarNode> OnCostMultiplierChanged;

        public void AddCostMultiplierCallback(System.Action<SimpleAStarNode> callBack)
        {
            OnCostMultiplierChanged += callBack;
        }

        public void RemoveCostMultiplierCallback(System.Action<SimpleAStarNode> callBack)
        {
            OnCostMultiplierChanged -= callBack;
        }

        public int mCostMultiplier;

        /// <summary>
        /// 从此节点到目标节点的代价(A星算法使用)
        /// </summary>
        public int f;

        /// <summary>
        /// 从起点到此节点的代价
        /// </summary>
        public int g;

        /// <summary>
        /// 在此节点上的单位
        /// </summary>
        private IList<ISimpleAStarUnit> mUnits;

        /// <summary>
        /// 刷新穿越代价
        /// </summary>
        private void RefreshPassCost()
        {
            int oldValue = mCostMultiplier;

            mCostMultiplier = 0;
            foreach (ISimpleAStarUnit unit in mUnits)
            {
                if (!unit.GetIsPassable())
                {
                    if (mWalkable)
                    {
                        mWalkable = false;
                        if (OnCostMultiplierChanged != null)
                        {
                            OnCostMultiplierChanged(this);
                        }
                    }
                    return;
                }
                mCostMultiplier += unit.GetPassCost();
            }

            if (mCostMultiplier != oldValue)
            {
                if (OnCostMultiplierChanged != null)
                {
                    OnCostMultiplierChanged(this);
                }
            }
        }
        /// <summary>
        /// 单位的 isPassable 属性被改变
        /// </summary>
        /// <returns><c>true</c> if this instance is passable change; otherwise, <c>false</c>.</returns>
        private void IsUnitPassableChanged()
        {
            RefreshPassCost();
        }
        /// <summary>
        /// 单位的 PassCost 属性被改变
        /// </summary>
        /// <returns><c>true</c> if this instance is passable change; otherwise, <c>false</c>.</returns>
        private void IsUnitPassCostChanged()
        {
            RefreshPassCost();
        }

        public bool AddUnit(ISimpleAStarUnit unit)
        {
            if (mWalkable)
            {
                if (!mUnits.Contains(unit))
                {
                    unit.AddIsPassableChangedEvent(IsUnitPassableChanged);
                    unit.AddPassCostChangedEvent(IsUnitPassCostChanged);

                    mUnits.Add(unit);
                    RefreshPassCost();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveUnit(ISimpleAStarUnit unit)
        {
            if (mUnits.Contains(unit))
            {
                unit.RemoveIsPassableChangedEvent(IsUnitPassableChanged);
                unit.RemovePassCostChangedEvent(IsUnitPassCostChanged);
                mUnits.Remove(unit);
                return true;
            }
            return false;
        }

        public ISimpleAStarUnit GetUnit(int index)
        {
            if (index < mUnits.Count)
            {
                return mUnits[index];
            }
            return null;
        }

        public int GetUnitIndex(ISimpleAStarUnit unit)
        {
            return mUnits.IndexOf(unit);
        }

        public int GetUnitNum()
        {
            return mUnits.Count;
        }

        public SimpleAStarNode(int nodeX, int nodeY)
        {
            mNodeX = nodeX;
            mNodeY = nodeY;

            mWalkable = true;
            mUnits = new List<ISimpleAStarUnit>();
        }
    }

    public class SimpleAStarManager
    {
        /**直角移动的Cost值*/
        public const int STRAIGHT_COST = 100;

        /**对角移动的Cost值*/
        public const int DIAG_COST = 140;

        /**直角移动的Cost值的十分之一*/
        public const int STRAIGHT_COST_DIVIDE10 = 10;

        /**对角移动的Cost值的十分之一*/
        public const int DIAG_COST_DIVIDE10 = 14;

        /// <summary>
        /// 地图节点
        /// </summary>
        private Dictionary<string, SimpleAStarNode> mNodes;

        /// <summary>
        /// 地图的宽度(列数)
        /// </summary>
        private int mNumCols;

        /// <summary>
        /// 地图的高度(行数)
        /// </summary>
        private int mNumRows;

        /// <summary>
        /// 当前节点到结束节点的估价函数
        /// </summary>
        private ISimpleAStarHeuristic mAStarHeuristic;

        /// <summary>
        /// 当前的寻路编号 
        /// </summary>
        private int mSearchPathCheckNum;

        /// <summary>
        /// 当前查找可移动范围的编号
        /// </summary>
        private int mWalkableRangeCheckNum;

        /// <summary>
        /// 是否是四向寻路，默认为八向寻路
        /// </summary>
        private bool mIsFourWay;

        /// <summary>
        /// 存放 "openList" 的最小二叉堆
        /// </summary>
        private BinaryHeapUtils mBinaryHeapUtils;

        public SimpleAStarNode GetNode(int nodeX, int nodeY)
        {
            string nodeKey = GetNodeKey(nodeX, nodeY);
            if (mNodes.ContainsKey(nodeKey))
            {
                return mNodes[nodeKey];
            }
            return null;
        }

        private string GetNodeKey(int nodeX, int nodeY)
        {
            return nodeX + ":" + nodeY;
        }

        /// <summary>
        /// 获取节点的相邻节点
        /// </summary>
        private IList<SimpleAStarNode> GetAdjacentNodes(SimpleAStarNode node)
        {
            IList<SimpleAStarNode> adjacentNodes = new List<SimpleAStarNode>();
            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;

            startX = Mathf.Max(0, node.mNodeX - 1);
            endX = Mathf.Min(mNumCols - 1, node.mNodeX + 1);
            startY = Mathf.Max(0, node.mNodeY - 1);
            endY = Mathf.Min(mNumRows - 1, node.mNodeY + 1);

            SimpleAStarNode varNode = null;
            for (int i = startX; i <= endX; ++i)
            {
                for (int j = startY; j <= endY; ++j)
                {
                    string nodeKey = GetNodeKey(i, j);
                    if (mNodes.TryGetValue(nodeKey, out varNode))
                    {
                        if (varNode != null && varNode != node)
                        {
                            if (mIsFourWay)
                            {
                                if (i != node.mNodeX && j != node.mNodeY)
                                {
                                    continue;
                                }
                            }
                            adjacentNodes.Add(varNode);
                        }
                    }
                }
            }

            return adjacentNodes;
        }

        /// <summary>
        /// 刷新节点的 links 属性
        /// </summary>
        private void RefreshNodeLinks(SimpleAStarNode node)
        {
            IList<SimpleAStarNode> adjacentNodes = GetAdjacentNodes(node);

            int cost = 0;
            List<SimpleAStarLinkNode> linkNodes = new List<SimpleAStarLinkNode>();

            SimpleAStarNode tmpNode = null;
            for (int i = 0; i < adjacentNodes.Count; ++i)
            {
                tmpNode = adjacentNodes[i];
                if (tmpNode == null) continue;
                if (tmpNode.mWalkable)
                {
                    if (node.mNodeX != tmpNode.mNodeX && node.mNodeY != tmpNode.mNodeY)
                    {
                        if (!mNodes[GetNodeKey(node.mNodeX, tmpNode.mNodeY)].mWalkable
                            || !mNodes[GetNodeKey(tmpNode.mNodeX, node.mNodeY)].mWalkable)
                        {
                            continue;
                        }
                        else
                        {
                            cost = DIAG_COST;
                        }
                    }
                    else
                    {
                        cost = STRAIGHT_COST;
                    }
                    linkNodes.Add(new SimpleAStarLinkNode(tmpNode, cost));
                }
            }

            node.mLinks = linkNodes;
        }

        /// <summary>
        /// 刷新节点的相邻节点的 links 属性
        /// </summary>
        /// <param name="node">Node.</param>
        private void RefreshLinksOfAdjacentNodes(SimpleAStarNode node)
        {
            IList<SimpleAStarNode> adjacentNodes = GetAdjacentNodes(node);

            SimpleAStarNode tmpNode = null;
            for (int i = 0; i < adjacentNodes.Count; ++i)
            {
                tmpNode = adjacentNodes[i];
                if (tmpNode == null) continue;
                RefreshNodeLinks(tmpNode);
            }
        }

        /// <summary>
        /// 刷新所有节点的 links 属性
        /// </summary>
        private void RefreshLinksOfAllNodes()
        {
            for (int i = 0; i < mNumCols; i++)
            {
                for (int j = 0; j < mNumRows; j++)
                {
                    RefreshNodeLinks(mNodes[GetNodeKey(i, j)]);
                }
            }
        }

        /// <summary> 
        /// 搜索路径 
        /// </summary> 
        private bool SearchBaseBinaryHeap(SimpleAStarNode startNode, SimpleAStarNode endNode, int nowCheckNum)
        {
            /**已进入关闭列表(从openList中移除即进入关闭列表)*/
            int STATUS_CLOSED = nowCheckNum + 1;
            mBinaryHeapUtils.Reset();

            startNode.g = 0;
            startNode.f = startNode.g + mAStarHeuristic.Heuristic(startNode.mNodeX, startNode.mNodeY, endNode.mNodeX, endNode.mNodeY);
            startNode.mSearchPathCheckNum = STATUS_CLOSED;

            int g = 0;
            SimpleAStarNode node = startNode;
            SimpleAStarNode tmpNode = null;
            while (node != endNode)
            {
                IList<SimpleAStarLinkNode> linkNodes = node.mLinks;
                for (int i = 0; i < linkNodes.Count; ++i)
                {
                    tmpNode = linkNodes[i].mNode;
                    if (tmpNode == null) continue;
                    g = node.g + linkNodes[i].mCost;

                    // 如果已被检查过
                    if (tmpNode.mSearchPathCheckNum >= nowCheckNum)
                    {
                        if (tmpNode.g > g)
                        {
                            tmpNode.f = g + mAStarHeuristic.Heuristic(tmpNode.mNodeX, tmpNode.mNodeY, endNode.mNodeX, endNode.mNodeY);
                            tmpNode.g = g;
                            tmpNode.mParentNode = node;
                            if (tmpNode.mSearchPathCheckNum == nowCheckNum)
                            {
                                mBinaryHeapUtils.ModifyNode(tmpNode.mBinaryHeapNode);
                            }
                        }
                    }
                    else
                    {
                        tmpNode.f = g + mAStarHeuristic.Heuristic(tmpNode.mNodeX, tmpNode.mNodeY, endNode.mNodeX, endNode.mNodeY);
                        tmpNode.g = g;
                        tmpNode.mParentNode = node;

                        tmpNode.mBinaryHeapNode = mBinaryHeapUtils.InsertNode(tmpNode);
                        tmpNode.mSearchPathCheckNum = nowCheckNum;
                    }
                }
                if (mBinaryHeapUtils.headNode != null)
                {
                    node = mBinaryHeapUtils.PopNode();
                    node.mSearchPathCheckNum = STATUS_CLOSED;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 寻路
        /// </summary>
        public IList<SimpleAStarNode> FindPath(SimpleAStarNode startNode, SimpleAStarNode endNode)
        {
            mSearchPathCheckNum += 2;
            if (SearchBaseBinaryHeap(startNode, endNode, mSearchPathCheckNum))
            {
                SimpleAStarNode currentNode = endNode;
                IList<SimpleAStarNode> pathList = new List<SimpleAStarNode>() { endNode };
                while (currentNode != startNode)
                {
                    currentNode = currentNode.mParentNode;
                    pathList.Add(currentNode);
                }
                return pathList;
            }
            return null;
        }

        public SimpleAStarManager(int numCols, int numRows, bool isFourWay = false)
        {
            mNumCols = numCols;
            mNumRows = numRows;
            mIsFourWay = isFourWay;
            //mAStarHeuristic = new SimpleAStarManhattanHeuristic();
            mAStarHeuristic = new SimpleAStarDiagonalHeuristic();

            SimpleAStarNode node = null;
            mNodes = new Dictionary<string, SimpleAStarNode>();
            for (int i = 0; i < mNumCols; i++)
            {
                for (int j = 0; j < mNumRows; j++)
                {
                    node = new SimpleAStarNode(i, j);
                    node.AddCostMultiplierCallback(RefreshLinksOfAdjacentNodes);
                    mNodes.Add(GetNodeKey(i, j), node);
                }
            }
            RefreshLinksOfAllNodes();
            mBinaryHeapUtils = new BinaryHeapUtils(numCols * numRows / 2);
        }


        public void SetIsFourWay(bool isFourWay)
        {
            if (mIsFourWay != isFourWay)
            {
                mIsFourWay = isFourWay;
                RefreshLinksOfAllNodes();
            }
        }
    }
}