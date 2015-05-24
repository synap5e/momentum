using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class EffectsController : MonoBehaviour {
    public float MaxBlurVelocity;
    public float MaxVignetteVelocity;

    public float BlurDelay;
    public float VignetteDelay;

    private float effectTime; 
    private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        float velocity = player.GetComponent<Rigidbody>().velocity.magnitude;
        if (velocity > 0.1f)
        {
            effectTime += Time.deltaTime;
        } else {
            effectTime = 0;
        }
        float blurAmount = velocity / MaxBlurVelocity;
        float vignetteAmount = velocity / MaxVignetteVelocity;
        //PlayerCamera.GetComponent<VignetteAndChromaticAberration>().blur = Mathf.Lerp(0, blurAmount, effectTime / BlurDelay);
        GetComponent<VignetteAndChromaticAberration>().intensity = Mathf.Lerp(0, vignetteAmount, effectTime / VignetteDelay);
	}
}
