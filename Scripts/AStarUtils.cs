using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A 星算法，公式：f = g + h;
/// </summary>
public class AStarUtils : MonoBehaviour 
{
	/// <summary>
	/// 直角移动的 g 值
	/// </summary>
	public const int STRAIGHT_COST = 10;
	
	/// <summary>
	/// 对角移动的 g 值
	/// </summary>
	public const int DIAG_COST = 14;
	
	/// <summary>
	/// 地图节点
	/// </summary>
	private Dictionary<string, AStarNode> nodes;
	
	/// <summary>
	/// 地图的宽度(列数)
	/// </summary>
	private int numCols;
	
	/// <summary>
	/// 地图的高度(行数)
	/// </summary>
	private int numRows;
	
	/// <summary>
	/// 当前节点到结束节点的估价函数
	/// </summary>
	private IAStarHeuristic iAStarHeuristic;
	
	/// <summary>
	/// 当前的寻路编号 
	/// </summary>
	private int searchPathCheckNum;
	
	/// <summary>
	/// 当前查找可移动范围的编号
	/// </summary>
	private int walkableRangeCheckNum;
	
	/// <summary>
	/// 是否是四向寻路，默认为八向寻路
	/// </summary>
	private bool isFourWay;
	
	/// <summary>
	/// 存放 "openList" 的最小二叉堆
	/// </summary>
	private BinaryHeapUtils binaryHeapUtils;

	/// <summary>
	/// 获取节点
	/// </summary>
	/// <returns>The node.</returns>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	public AStarNode GetNode(int nodeX, int nodeY)
	{
		string nodeKey = this.GetNodeKey (nodeX, nodeY);
		if (this.nodes.ContainsKey (nodeKey)) 
		{
			return this.nodes[nodeKey];
		}
		return null;
	}

	/// <summary>
	/// 组装 Star Key
	/// </summary>
	/// <returns>The node key.</returns>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	private string GetNodeKey(int nodeX, int nodeY)
	{
		return nodeX + ":" + nodeY;
	}
	
	/// <summary>
	/// 获取节点的相邻节点
	/// </summary>
	/// <returns>The adjacent nodes.</returns>
	/// <param name="node">Node.</param>
	private IList<AStarNode> GetAdjacentNodes(AStarNode node)
	{
		IList<AStarNode> adjacentNodes = new List<AStarNode> ();
		
		int startX = 0;
		int endX = 0;
		int startY = 0;
		int endY = 0;
		
		startX = Mathf.Max(0, node.nodeX - 1);
		endX = Mathf.Min(this.numCols - 1, node.nodeX + 1);
		
		startY = Mathf.Max(0, node.nodeY - 1);
		endY = Mathf.Min(this.numRows - 1, node.nodeY + 1);
		
		AStarNode varNode = null;
		for(int i = startX; i <= endX; i++)
		{
			for(int j = startY; j <= endY; j++)
			{
				varNode = this.nodes[this.GetNodeKey(i, j)];
				if(varNode != node)
				{
					if(this.isFourWay)
					{
						if(!(i == node.nodeX || j == node.nodeY))
						{
							continue;
						}
					}
					adjacentNodes.Add(varNode);
				}
			}
		}
		return adjacentNodes;
	}
	
	/// <summary>
	/// 刷新节点的 links 属性
	/// </summary>
	/// <param name="node">Node.</param>
	private void RefreshNodeLinks(AStarNode node)
	{
		IList<AStarNode> adjacentNodes = this.GetAdjacentNodes(node);
		
		int cost = 0;
		List<AStarLinkNode> links = new List<AStarLinkNode> ();
		foreach(AStarNode nodeItem in adjacentNodes)
		{
			if(nodeItem.walkable)
			{
				if(node.nodeX != nodeItem.nodeX && node.nodeY != nodeItem.nodeY)
				{
					if(!this.nodes[this.GetNodeKey(node.nodeX, nodeItem.nodeY)].walkable || !this.nodes[this.GetNodeKey(nodeItem.nodeX, node.nodeY)].walkable)
					{
						continue;
					}else
					{
						cost = DIAG_COST;
					}
				}else
				{
					cost = STRAIGHT_COST;
				}
				links.Add(new AStarLinkNode(nodeItem, cost));
			}
		}

		node.links = links;
	}

	/// <summary>
	/// 刷新节点的相邻节点的 links 属性
	/// </summary>
	/// <param name="node">Node.</param>
	private void RefreshLinksOfAdjacentNodes(AStarNode node)
	{
		IList<AStarNode> adjacentNodes = this.GetAdjacentNodes(node);
		foreach(AStarNode adjacentNode in adjacentNodes)
		{
			this.RefreshNodeLinks(adjacentNode);
		}
	}
	
	/// <summary>
	/// 刷新所有节点的 links 属性
	/// </summary>
	private void RefreshLinksOfAllNodes()
	{
		for(int i = 0; i < this.numCols; i++)
		{
			for(int j = 0; j < this.numRows; j++)
			{
				this.RefreshNodeLinks(this.nodes[this.GetNodeKey(i, j)]);
			}
		}
	}
	
	/// <summary>
	/// 搜索路径
	/// </summary>
	/// <returns><c>true</c>, if base binary heap was searched, <c>false</c> otherwise.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	/// <param name="nowCheckNum">Now check number.</param>
	private bool SearchBaseBinaryHeap(AStarNode startNode, AStarNode endNode, int nowCheckNum)
	{
		int STATUS_CLOSED = nowCheckNum + 1;

		this.binaryHeapUtils.Reset ();
		
		startNode.g = 0;
		startNode.f = startNode.g + this.iAStarHeuristic.Heuristic(startNode.nodeX, startNode.nodeY, endNode.nodeX, endNode.nodeY);
		startNode.searchPathCheckNum = STATUS_CLOSED;

		int g = 0;
		AStarNode node = startNode;
		AStarNode nodeItem;

		while(node != endNode)
		{
			IList<AStarLinkNode> links = node.links;
			foreach(AStarLinkNode link in links)
			{
				nodeItem = link.node;
				g = node.g + link.cost;

				// 如果已被检查过
				if(nodeItem.searchPathCheckNum >= nowCheckNum)
				{
					if(nodeItem.g > g)
					{
						nodeItem.f = g + this.iAStarHeuristic.Heuristic(nodeItem.nodeX, nodeItem.nodeY, endNode.nodeX, endNode.nodeY);
						nodeItem.g = g;
						nodeItem.parentNode = node;
						if(nodeItem.searchPathCheckNum == nowCheckNum)
						{
							this.binaryHeapUtils.ModifyNode(nodeItem.binaryHeapNode);
						}
					}
				}else{
					nodeItem.f = g + this.iAStarHeuristic.Heuristic(nodeItem.nodeX, nodeItem.nodeY, endNode.nodeX, endNode.nodeY);
					nodeItem.g = g;
					nodeItem.parentNode = node;
					
					nodeItem.binaryHeapNode = this.binaryHeapUtils.InsertNode(nodeItem);
					nodeItem.searchPathCheckNum = nowCheckNum;
				}
			}
			if(this.binaryHeapUtils.headNode != null)
			{
				node = this.binaryHeapUtils.PopNode();

				node.searchPathCheckNum = STATUS_CLOSED;
			}else
			{
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// 寻路
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	public IList<AStarNode> FindPath(AStarNode startNode, AStarNode endNode)
	{
		this.searchPathCheckNum += 2;
		if(this.SearchBaseBinaryHeap(startNode, endNode, searchPathCheckNum))
		{
			AStarNode currentNode = endNode;
			IList<AStarNode> pathList = new List<AStarNode>(){
				startNode
			};
			while(currentNode != startNode)
			{
				currentNode = currentNode.parentNode;
				pathList.Add(currentNode);
			}

			return pathList;
		}
		return null;
	}
	
	/// <summary>
	/// 返回节点在指定的代价内可移动的范围
	/// </summary>
	/// <returns>The range.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="costLimit">Cost limit.</param>
	public IList<AStarNode> WalkableRange(AStarNode startNode, int costLimit)
	{
		this.walkableRangeCheckNum ++;

		int maxStep = (int)(costLimit / STRAIGHT_COST);
		
		int startX = Mathf.Max(startNode.nodeX - maxStep, 0);
		int endX = Mathf.Min(startNode.nodeX + maxStep, this.numCols - 1);
		int startY = Mathf.Max(startNode.nodeY - maxStep, 0);
		int endY = Mathf.Min(startNode.nodeY + maxStep, this.numRows - 1);
		
		IList<AStarNode> rangeList = new List<AStarNode> ();
		for(int i = startX; i <= endX; i++)
		{
			for(int j = startY; j <= endY; j++)
			{
				AStarNode nodeItem = this.nodes[this.GetNodeKey(i, j)];
				if(nodeItem.walkable && nodeItem.walkableRangeCheckNum != walkableRangeCheckNum)
				{
					IList<AStarNode> pathList = this.FindPath(startNode, nodeItem);
					if(pathList != null && pathList[pathList.Count - 1].f <= costLimit)
					{
						foreach(AStarNode node in pathList)
						{
							if(node.walkableRangeCheckNum != walkableRangeCheckNum)
							{
								node.walkableRangeCheckNum = walkableRangeCheckNum;
								rangeList.Add(node);
							}
						}
					}
				}
			}
		}
		return rangeList;
	}

	public AStarUtils(int numCols, int numRows, bool isFourWay = false)
	{
		this.numCols = numCols;
		this.numRows = numRows;
		this.isFourWay = isFourWay;
		this.iAStarHeuristic = new AStarManhattanHeuristic ();
		//this.iAStarHeuristic = new AStarDiagonalHeuristic ();
		
		AStarNode node = null;
		this.nodes = new Dictionary<string, AStarNode> ();
		for(int i = 0; i < this.numCols; i++)
		{
			for(int j = 0; j < this.numRows; j++)
			{
				node = new AStarNode(i, j);
				node.AddHeuristic(this.RefreshLinksOfAdjacentNodes, node);
				this.nodes.Add(this.GetNodeKey(i, j), node);
			}
		}
		this.RefreshLinksOfAllNodes();
		this.binaryHeapUtils = new BinaryHeapUtils(numCols * numRows / 2);
	}
}
