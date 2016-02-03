using UnityEngine;
using System.Collections;

public class AStarCallback
{
	//
	public delegate void IsPassableChangeCallback();

	// 
	public delegate void HeuristicCallback(AStarNode aStarNode);

	public event HeuristicCallback OnHeuristic;

	public event IsPassableChangeCallback OnIsPassableChange;

	public void InvokeHeuristic(AStarNode callAStarNode)
	{
		if (this.OnHeuristic != null) 
		{
			this.OnHeuristic(callAStarNode);
		}
	}
	
	public void InvokeIsPassableChange()
	{
		if (this.OnIsPassableChange != null) 
		{
			this.OnIsPassableChange();
		}
	}
}
