﻿using UnityEngine;
using System.Collections;

public class CastMagic : MonoBehaviour {

	static Plane worldPlane = new Plane(Vector3.up, Vector3.zero);

	[System.Serializable]
	public class CastableSpell {
		public GameObject spellPrefab;
		public int cooldownFrames = 100;
		public float manaCost = 0.25f;
		public int coinCost = 0;
	}
	public CastableSpell[] spells;
	public GameObject player;

	public KeyCode fireButton; 

	public float manaRecoveryRate = 0.1f;

	private float manaValue = 1.0f;
	public float mana {
		get { return manaValue; }
	}

	public float mouseDeadZone;

	private int[] cooldownTimers;
	private int spellIndex = 0;


	// Use this for initialization
	void Start () {
		cooldownTimers = new int[spells.Length];
	}

	public void switchTo (int i)
	{
		spellIndex = i;
	}

	public int coins {
		get;
		set;
	}

	// Update is called once per frame
	void Update () {
		manaValue = Mathf.Min (1, manaValue +  manaRecoveryRate * Time.deltaTime);
		for(int i=0; i < cooldownTimers.Length; i++)
		{
			cooldownTimers[i]--;
		}
		if (Input.GetButton("Fire1") && Input.mousePosition.y > mouseDeadZone) {
			//Debug.Log(spellIndex);
			CastableSpell spell = spells[spellIndex];
			if (cooldownTimers[spellIndex] <= 0 && manaValue - spell.manaCost >= 0 && coins - spell.coinCost >= 0){
				coins -= spell.coinCost;
				manaValue -= spell.manaCost;
					cooldownTimers[spellIndex] = spell.cooldownFrames;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					float rayDistance;
					if(worldPlane.Raycast (ray, out rayDistance)) {
						Vector3 hitPoint = ray.GetPoint(rayDistance);
						hitPoint.y = 0;
						GameObject mb = Instantiate(spell.spellPrefab, transform.position + new Vector3(0, 2.1f, 0) + transform.forward * 0.5f, Quaternion.LookRotation(hitPoint - transform.position)) as GameObject;
						if (mb.GetComponent<PickupableCoin>()){
							mb.GetComponent<PickupableCoin>().pickerUpper = gameObject;
							mb.GetComponent<PickupableCoin>().coinThrower = this;
							mb.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Impulse);
						}
					//mb.GetComponent<MagicBehaviour>().speed = 
				}
			}
		}
	}
}
