using UnityEngine;
using System.Collections;

public class AStarUnit : IAStarUnit 
{
	/// <summary>
	/// 是否可以通过
	/// </summary>
	private bool _isPassable;

	private AStarCallback aStarCallback = new AStarCallback();

	/// <summary>
	/// 添加通过回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void AddIsPassableChange(AStarCallback.IsPassableChangeCallback callback)
	{
		this.aStarCallback.OnIsPassableChange += callback;
	}

	/// <summary>
	/// 移除通过回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RemoveIsPassableChange(AStarCallback.IsPassableChangeCallback callback)
	{
		this.aStarCallback.OnIsPassableChange -= callback;
	}

	/// <summary>
	/// 是否可以通过
	/// </summary>
	/// <value>true</value>
	/// <c>false</c>
	public bool isPassable 
	{ 
		get { return this._isPassable; } 
		set 
		{ 
			if(this._isPassable != value)
			{
				this._isPassable = value;
				this.aStarCallback.InvokeIsPassableChange();
			}
		} 
	}
}
