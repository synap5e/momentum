using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public Slider health;
	public Slider mana;
	public Button[] abilities;

	public GameObject magicUser;

	private CastMagic castMagicScript;

	// Use this for initialization
	void Start () {
		castMagicScript = magicUser.GetComponent<CastMagic> ();
/*		for (int i = 0; i < abilities.Length; i++) {
			// Y U no clousure?

			int copyOfI = i;
			abilities[i].onClick.RemoveAllListeners();
			abilities[i].onClick.AddListener(() => selectButton(copyOfI));
			Debug.Log (abilities[i].onClick);
		}*/
		selectButton (0);
	}

	public void showCooldown (float f)
	{

		//throw new System.NotImplementedException ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			selectButton (0);
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			selectButton (1);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			selectButton (2);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			selectButton (3);
		}

		mana.value = castMagicScript.mana;
	}

	public void selectButton (int buttonIndex)
	{
		Debug.Log (buttonIndex);
		castMagicScript.switchTo(buttonIndex);

		for (int i = 0; i < abilities.Length; i++) {
			abilities [i].interactable = true;
		}
		abilities [buttonIndex].interactable = false;
		//.normalColor  = selectedColor;
	}
}
