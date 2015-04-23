using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrinketDisplay : MonoBehaviour {

	public Image[] images;
	public TrinketCollector collector;
			
	// Update is called once per frame
	void Update () {

		//Debug.Log (collector.trinketCount);
		for (int i=0; i < images.Length; i++) {
			if (i < collector.trinketCount){
				images[i].enabled = true;
			} else {
				images[i].enabled = false;
			}
		}

	}
}
