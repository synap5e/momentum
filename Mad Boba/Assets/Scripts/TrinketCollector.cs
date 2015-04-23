using UnityEngine;
using System.Collections;

public class TrinketCollector : MonoBehaviour {
	
	public GameObject[] trinkets;
	public TrinketDisplay display;
	
	static Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
	
	public int trinketCount {
		get;
		set;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject trinket in trinkets){
			if (Vector3.Magnitude (trinket.transform.position - transform.position) < 3 && trinket.activeSelf && trinket.transform.position.y < 0.5f){
				trinket.SetActive(false);
				trinketCount++;
			}
		}
		
		if (Input.GetButton("Fire1") && trinketCount > 0) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			if(worldPlane.Raycast (ray, out rayDistance)) {
				//Debug.Log("place");
				Vector3 hitPoint = ray.GetPoint(rayDistance);
				hitPoint.y = 0;
				GameObject trinketToThrow = null;
				foreach (GameObject t in trinkets){
					if (!t.activeSelf){
						trinketToThrow = t;
						//Debug.Log("found");
						break;
					}
				}

				trinketToThrow.SetActive(true);
				trinketToThrow.transform.position = transform.position + Vector3.up*2;
				trinketToThrow.GetComponent<Rigidbody>().AddForce(30.0f * (Vector3.Normalize(hitPoint - transform.position) + Vector3.up * 0.5f), ForceMode.Impulse);
				trinketCount--;
				
			}
		}
	}
}
