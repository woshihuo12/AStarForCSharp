using UnityEngine;
using System.Collections.Generic;

public class TestWorld : MonoBehaviour 
{
	public GameObject cubeObject;
	public GameObject pathObject;

	public Camera mainCamera;
	public SceneGrid sceneGrid;

	private AStarUtils aStarUtils;

	private AStarNode beginNode;

	private int cols = 20;
	private int rows = 20;

	private IList<GameObject> pathList;

	void Awake()
	{
		this.pathList = new List<GameObject> ();
		this.aStarUtils = new AStarUtils (this.cols, this.rows);

		// cols
		for(int i = 0; i < this.cols; i++)
		{
			// rows
			for(int j = 0; j < this.rows; j++)
			{
				AStarUnit aStarUnit = new AStarUnit();

				if(i != 0 && j != 0 && Random.Range(1, 10) <= 3)
				{
					aStarUnit.isPassable = false;

					GameObject gameObject = (GameObject)Instantiate(cubeObject);
					if(gameObject != null)
					{
						gameObject.transform.localPosition = new Vector3(i - this.cols * 0.5f + 0.5f, 0f, j - this.cols * 0.5f + 0.5f);
					}

				}else{
					aStarUnit.isPassable = true;
				}

				this.aStarUtils.GetNode(i,j).AddUnit(aStarUnit);
			}
		}
	}

	private void FindPath(int x, int y)
	{
		AStarNode endNode = this.aStarUtils.GetNode(x, y);

		if (this.beginNode == null) 
		{
			this.beginNode = endNode;
			return;
		}

		if (this.pathList != null && this.pathList.Count > 0) 
		{
			foreach (GameObject xxObject in this.pathList) 
			{
				Destroy(xxObject);
			}
		}
		
		if(endNode != null && endNode.walkable)
		{
			System.DateTime dateTime = System.DateTime.Now;

			IList<AStarNode> pathList = this.aStarUtils.FindPath(this.beginNode, endNode);

			System.DateTime currentTime = System.DateTime.Now;

			System.TimeSpan timeSpan = currentTime.Subtract(dateTime);

			Debug.Log(timeSpan.Seconds + "秒" + timeSpan.Milliseconds + "毫秒");

			if(pathList != null && pathList.Count > 0)
			{
				foreach(AStarNode nodeItem in pathList)
				{
					GameObject gameObject = (GameObject)Instantiate(this.pathObject);
					this.pathList.Add(gameObject);
					gameObject.transform.localPosition = new Vector3(nodeItem.nodeX - this.cols * 0.5f + 0.5f, 0f, nodeItem.nodeY - this.cols * 0.5f + 0.5f);
				}
			}
			this.beginNode = endNode;
		}
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

			RaycastHit raycastHit = new RaycastHit();
			if(Physics.Raycast(ray, out raycastHit))
			{
				if(raycastHit.collider.gameObject.tag == "Plane")
				{
					Vector3 pointItem = this.sceneGrid.transform.InverseTransformPoint(raycastHit.point) * 2f;

					pointItem.x = this.cols * 0.5f + Mathf.Ceil(pointItem.x) - 1f;
					pointItem.z = this.cols * 0.5f + Mathf.Ceil(pointItem.z) - 1f;

					this.FindPath((int)pointItem.x, (int)pointItem.z);
				}
			}
		}
	}
}
