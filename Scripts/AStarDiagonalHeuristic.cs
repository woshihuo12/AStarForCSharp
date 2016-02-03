using UnityEngine;
using System.Collections;

public class AStarDiagonalHeuristic : IAStarHeuristic
{
	public int Heuristic(int x1, int y1, int x2, int y2)
	{
		int dx = x1 > x2 ? x1 - x2 : x2 - x1;
		int dy = y1 > y2 ? y1 - y2 : y2 - y1;
		
		return dx > dy ? AStarUtils.DIAG_COST * dy + AStarUtils.STRAIGHT_COST * (dx - dy) : AStarUtils.DIAG_COST * dx + AStarUtils.STRAIGHT_COST * (dy - dx);
	}
}
