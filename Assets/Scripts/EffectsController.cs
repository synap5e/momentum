using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class EffectsController : MonoBehaviour {
    public Camera PlayerCamera;
    public float MaxBlurVelocity;
    public float MaxVignetteVelocity;

    public float BlurDelay;
    public float VignetteDelay;

    private float effectTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float velocity = GetComponent<Rigidbody>().velocity.magnitude;
        if (velocity > 0.1f)
        {
            effectTime += Time.deltaTime;
        } else {
            effectTime = 0;
        }
        float blurAmount = velocity / MaxBlurVelocity;
        float vignetteAmount = velocity / MaxVignetteVelocity;
        //PlayerCamera.GetComponent<VignetteAndChromaticAberration>().blur = Mathf.Lerp(0, blurAmount, effectTime / BlurDelay);
        PlayerCamera.GetComponent<VignetteAndChromaticAberration>().intensity = Mathf.Lerp(0, vignetteAmount, effectTime / VignetteDelay);
	}
}
