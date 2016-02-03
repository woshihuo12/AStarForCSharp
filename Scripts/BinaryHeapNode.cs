using UnityEngine;
using System.Collections;

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
	public AStarNode data;

	public BinaryHeapNode(AStarNode data, BinaryHeapNode parentNode)
	{
		this.data = data;
		this.parentNode = parentNode;
	}
}
