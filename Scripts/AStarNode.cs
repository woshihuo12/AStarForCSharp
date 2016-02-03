using UnityEngine;
using System.Collections.Generic;

public class AStarNode
{
	/// <summary>
	/// 坐标 x
	/// </summary>
	public int nodeX;
	
	/// <summary>
	/// 坐标 y
	/// </summary>
	public int nodeY;
	
	/// <summary>
	/// 父节点
	/// </summary>
	public AStarNode parentNode;
	
	/// <summary>
	/// 二叉堆节点
	/// </summary>
	public BinaryHeapNode binaryHeapNode;
	
	/// <summary>
	/// 与此节点相邻的可通过的邻节点
	/// </summary>
	public IList<AStarLinkNode> links;
	
	/// <summary>
	/// 搜索路径的检查编号(确定是否被检查过)
	/// </summary>
	public int searchPathCheckNum;
	
	/// <summary>
	/// 可移动范围的检查编号(确定是否被检查过)
	/// </summary>
	public int walkableRangeCheckNum;
	
	/// <summary>
	/// 是否能被穿越
	/// </summary>
	public bool walkable;
	
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
	private IList<IAStarUnit> units;

	/// <summary>
	/// 通过回调函数
	/// </summary>
	private AStarCallback aStarCallback = new AStarCallback ();

	/// <summary>
	/// 回调函数参数
	/// </summary>
	private AStarNode aStarNodeParam;

	public int unitCount
	{
		get { return this.units.Count; }
	}
	
	/// <summary>
	/// 添加穿越代价被修改后的回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	/// <param name="aStarNodeParam">A star node parameter.</param>
	public void AddHeuristic(AStarCallback.HeuristicCallback callback, AStarNode aStarNodeParam)
	{
		this.aStarNodeParam = aStarNodeParam;
		this.aStarCallback.OnHeuristic += callback;
	}
	
	/// <summary>
	/// 移除穿越代价被修改后的回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RemoveHeuristic(AStarCallback.HeuristicCallback callback)
	{
		this.aStarCallback.OnHeuristic -= callback;
	}
	
	/// <summary>
	/// 刷新穿越代价
	/// </summary>
	private void RefreshPassCost()
	{
		foreach(IAStarUnit unit in this.units)
		{
			if(!unit.isPassable)
			{
				if(this.walkable)
				{
					this.walkable = false;
					this.aStarCallback.InvokeHeuristic(this.aStarNodeParam);
				}
				return;
			}
		}
	}

	/// <summary>
	/// 单位的 isPassable 属性被改变
	/// </summary>
	/// <returns><c>true</c> if this instance is passable change; otherwise, <c>false</c>.</returns>
	/*private void IsPassableChange()
	{
		this.RefreshPassCost();
	}*/
	
	/// <summary>
	/// 添加单位
	/// </summary>
	/// <returns><c>true</c>, if unit was added, <c>false</c> otherwise.</returns>
	/// <param name="unit">Unit.</param>
	public bool AddUnit(IAStarUnit unit)
	{
		if(this.walkable)
		{
			if(this.units.IndexOf(unit) == -1)
			{
				//unit.AddIsPassableChange(this.IsPassableChange);
				this.units.Add(unit);
				RefreshPassCost();
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// 移除单位
	/// </summary>
	/// <returns><c>true</c>, if unit was removed, <c>false</c> otherwise.</returns>
	/// <param name="unit">Unit.</param>
	public bool RemoveUnit(IAStarUnit unit)
	{
		int index = this.units.IndexOf(unit);
		if(index != -1)
		{
			//unit.RemoveIsPassableChange(this.IsPassableChange);
			this.units.RemoveAt(index);
			this.RefreshPassCost();
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// 地图节点
	/// </summary>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	public AStarNode(int nodeX, int nodeY)
	{
		this.nodeX = nodeX;
		this.nodeY = nodeY;
		
		this.walkable = true;
		this.units = new List<IAStarUnit> ();
	}
}
