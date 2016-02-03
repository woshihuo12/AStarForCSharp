using UnityEngine;
using System.Collections;

/// <summary>
/// 邻节点
/// </summary>
public class AStarLinkNode
{
	/// <summary>
	/// 节点
	/// </summary>
	public AStarNode node;

	/// <summary>
	/// 花费代价
	/// </summary>
	public int cost;
	
	public AStarLinkNode(AStarNode node, int cost)
	{
		this.node = node;
		this.cost = cost;
	}
}
