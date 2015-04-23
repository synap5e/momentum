using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBadArgo : MonoBehaviour {
	
	public float speed = 1.0f;
	public float aggroDistance = 0.0f;
	public float rotateSpeed = 10.0f;
	public GameObject player;
	
	public GameObject[] chickenTokens;
	
	private enum State
	{
		Patroling,
		Aggro,
		Chicken,
		Pathing
	}
	
	private State state = State.Patroling;
	private WorldGrid grid;
	List<WorldGrid.Node> path;
	
	float transition;
	
	int pathIndex;
	
	public void Start(){
		grid = new WorldGrid (transform.position.x - 100, transform.position.z - 100, 200, 200, 2);
		
		/*foreach (AIBadArgo.WorldGrid.Node n in grid.SolvePath (transform.position.x, transform.position.z, player.transform.position.x, player.transform.position.z)) {
			Debug.Log (n.x + ", " + n.z);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		if (state == State.Patroling) {
			GetComponent<ArgoAnimationController> ().SignalStartMoving ();
			transform.rotation *= Quaternion.AngleAxis (-rotateSpeed * Time.deltaTime, Vector3.up);
			if (Vector3.Magnitude (transform.position - player.transform.position) < aggroDistance) {
				state = State.Aggro;
			}
			GetComponent<Rigidbody> ().AddForce (transform.forward * speed * 60.0f * Time.deltaTime, ForceMode.Impulse);
		} else if (state == State.Aggro) {
			GetComponent<ArgoAnimationController> ().SignalStartMoving ();
			Vector3 toTarget = player.transform.position - transform.position;
			Quaternion lookRotation = Quaternion.LookRotation (toTarget, Vector3.up);
			float rotate = Quaternion.Angle (transform.rotation, lookRotation);
			float donePercentage = Mathf.Min (1F, Time.deltaTime / (rotate / (rotateSpeed * 5.0f)));
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, donePercentage);
			GetComponent<Rigidbody> ().AddForce (transform.forward * speed * 100.0f * Time.deltaTime, ForceMode.Impulse);
			
			foreach (GameObject ct in chickenTokens) {
				if (Vector3.Magnitude (transform.position - ct.transform.position) < aggroDistance) {
					state = State.Chicken;
				}
			}
			
		} else if (state == State.Chicken) {
			GetComponent<ArgoAnimationController> ().SignalStopMoving ();
		} else if (state == State.Pathing) {
			if (path == null){
				path = grid.SolvePath (transform.position.x, transform.position.z, player.transform.position.x, player.transform.position.z);
				pathIndex = 1;
				transition = 0;
			}
			Vector3 prev = new Vector3(path[pathIndex-1].x, transform.position.y, path[pathIndex-1].z);
			Vector3 destination = new Vector3(path[pathIndex].x, transform.position.y, path[pathIndex].z);

			Quaternion lookRotation = Quaternion.LookRotation (destination-prev, Vector3.up);
			float rotate = Quaternion.Angle (transform.rotation, lookRotation);
			float donePercentage = Mathf.Min (1F, Time.deltaTime / (rotate / (rotateSpeed * 7.0f)));
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, donePercentage);
			
			transform.position = Vector3.Lerp(prev, destination, transition);

			Debug.Log (transition);
			transition += Time.deltaTime * 2.0f;
			if (transition > 1){
				transition -= 1;
				pathIndex++;
				if (pathIndex == path.Count){
					state = State.Patroling;
				}
			}
		}
		
		if (Input.GetKeyDown ("tab")) {
			state = State.Pathing;
			path = null;
		}
	}
	
	public class WorldGrid{
		
		public class Node
		{
			public bool walkable;
			public float x, z;
			public List<Node> connections = new List<Node> ();
		}
		
		private Node[,] map;
		
		private float x, z, w, h, step;
		
		public WorldGrid(float x, float z, float w, float h, float step){
			this.x = x;
			this.z = z;
			this.w = w;
			this.h = h;
			this.step = step;
			
			int xm = (int)Mathf.Ceil (w/step)+1;
			int zm = (int)Mathf.Ceil(h/step)+1;
			map = new Node[xm, zm];
			for (int xi=0;xi<xm;xi++){
				for (int zi=0;zi<zm;zi++){
					map[xi,zi] = new Node();
				}
			}
			
			x = this.x;
			for (int xi=0; x < this.x+w; x+=step)
			{
				z = this.z;
				for (int zi=0; z < this.z+h; z+=step)
				{
					Node n = map[xi,zi];
					
					n.walkable = !Physics.Raycast(new Vector3(x, -100, z), Vector3.up, Mathf.Infinity, 1<<8); // environment is layer 8
					n.x = x;
					n.z = z;
					
					if (xi > 0) n.connections.Add(map[xi-1,zi]);
					if (zi > 0) n.connections.Add(map[xi,zi-1]);
					if (xi < xm) n.connections.Add(map[xi+1,zi]);
					if (zi < zm) n.connections.Add(map[xi,zi+1]);
					++zi;
				}
				++xi;
			}
			
		}
		
		
		static float Distance(Node start, Node goal)
		{
			if(!start.walkable || !goal.walkable)
				return float.MaxValue;
			return Vector2.Distance(new Vector2(start.x, start.z), new Vector2(goal.x, goal.z));
		}
		
		// Find the current lowest score path
		static Node LowestScore(List<Node> openset, Dictionary<Node, float> scores)
		{
			int index = 0;
			float lowScore = float.MaxValue;
			
			for(int i = 0; i < openset.Count; i++)
			{
				if(scores[openset[i]] > lowScore)
					continue;
				index = i;
				lowScore = scores[openset[i]];
			}
			
			return openset[index];
		}
		
		
		public List<Node> SolvePath(float x, float z, float xto, float zto){
			int xi = (int)((x - this.x) / this.step);
			int zi = (int)((z - this.z) / this.step);
			
			int xito = (int)((xto - this.x) / this.step);
			int zito = (int)((zto - this.z) / this.step);
			
			Debug.Log (this.x + ", " + x);
			
			Node start = map [xi, zi];
			Node goal = map [xito, zito];
			// adapted from http://wiki.unity3d.com/index.php?title=AStarHelper
			
			Debug.Log (start.connections.Count);
			
			List<Node> closedset = new List<Node>();
			List<Node> openset = new List<Node>();
			
			openset.Add(start);
			
			Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();    // The map of navigated nodes.
			
			
			Dictionary<Node, float> g_score = new Dictionary<Node, float>();
			g_score[start] = 0.0f; // Cost from start along best known path.
			
			Dictionary<Node, float> h_score = new Dictionary<Node, float>();
			h_score[start] = Distance(start, goal); 
			
			Dictionary<Node, float> f_score = new Dictionary<Node, float>();
			f_score[start] = h_score[start]; // Estimated total cost from start to goal through y.
			
			while(openset.Count != 0)
			{
				Node n = LowestScore(openset, f_score);
				if(n == goal)
				{
					List<Node> result = new List<Node>();
					ReconstructPath(came_from, n, ref result);
					return result;
				}
				openset.Remove(n);
				closedset.Add(n);
				foreach(Node y in n.connections)
				{
					if(!y.walkable || closedset.Contains(y))
						continue;
					float tentative_g_score = g_score[n] + Distance(n, y);
					
					bool tentative_is_better = false;
					if(!openset.Contains(y))
					{
						openset.Add(y);
						tentative_is_better = true;
					}
					else if (tentative_g_score < g_score[y])
						tentative_is_better = true;
					
					if(tentative_is_better)
					{
						came_from[y] = n;
						g_score[y] = tentative_g_score;
						h_score[y] = Distance(y, goal);
						f_score[y] = g_score[y] + h_score[y];
					}
				}
			}
			
			return null;
		}
		
		static void ReconstructPath(Dictionary<Node, Node> came_from, Node current_node, ref List<Node> result)
		{
			if(came_from.ContainsKey(current_node))
			{
				ReconstructPath(came_from, came_from[current_node], ref result);
				result.Add(current_node);
				return;
			}
			result.Add(current_node);
		}
		
		
		
		
		
	}
	
	
	
	
}
