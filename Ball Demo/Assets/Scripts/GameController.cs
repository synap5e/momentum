using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject enemyPrefab;

	// Use this for initialization
	void Start () {
		GameObject stage = GameObject.Find("Stage");
		Bounds stageMeshBounds = stage.GetComponent<Collider>().bounds;
		float stageY = stageMeshBounds.max.y + 0.1f + (enemyPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.y / 2f);
		stageY = 1f;

		for(int i = 0; i != 50; i++){
			float randomX = Random.Range(stageMeshBounds.min.x, stageMeshBounds.max.x);
			float randomZ = Random.Range(stageMeshBounds.min.z, stageMeshBounds.max.z);

			Vector3 position = new Vector3(randomX, stageY, randomZ);
			Instantiate(enemyPrefab, position, Quaternion.identity);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
