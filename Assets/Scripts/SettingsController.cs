using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;

public class SettingsController : MonoBehaviour {

	[Header("UI Elements")]
	public UnityEngine.UI.Text sensitivityText;

	[Header("Game Settings Elements")]
	public GameObject fovSlider;
	public GameObject sensSlider;
	public GameObject viewmodelToggle;

	[Header("Audio Settings Elements")]
	public GameObject masterSlider;
	public GameObject musicSlider;
	public GameObject soundEffectSlider;

	[Header("Video Settings Elements")]
	public GameObject ambientOcclusionToggle;	
	public GameObject bloomToggle;	
	public GameObject edgeDetectionToggle;
	public GameObject motionBlurToggle;
	public GameObject vignetteAberrationToggle;

	[Header("Settings")]
	public float fov = 60f;
	public float sensitivity = 2f;
	public bool viewModelOn = true;
	
	public float masterVolume = 10f;
	public float musicVolume = 10f;
	public float soundEffectsVolume = 10f;	

	public bool ambientOcclusion = false;
	public bool vignetteAberration = false;
	public bool edgeDetection = false;
	public bool motionBlur = false;
	public bool bloom = false;

	private string filename = "settings.json";

	public string SaveToString()
	{
		JSONNode N = new JSONClass(); // Start with JSONArray or JSONClass
		N["settings"]["fov"].AsFloat = fov;
		N["settings"]["sensitivity"].AsFloat = sensitivity;
		N["settings"]["viewmodel"].AsFloat = viewModelOn ? 1 : 0;
		
		N["audio"]["master"].AsFloat = masterVolume;
		N["audio"]["music"].AsFloat = musicVolume;
		N["audio"]["sound effects"].AsFloat = soundEffectsVolume;

		N["video"]["ambientOcclusion"].AsFloat = ambientOcclusion ? 1 : 0;		
		N["video"]["bloom"].AsFloat = bloom ? 1 : 0;
		N["video"]["edgeDetection"].AsFloat = edgeDetection ? 1 : 0;
		N["video"]["motionBlur"].AsFloat = motionBlur ? 1 : 0;		
		N["video"]["vignetteAberration"].AsFloat = vignetteAberration ? 1 : 0;

		return N.ToJSON(4);
	}
	
	public void LoadString(String text)
	{
		JSONNode N = JSON.Parse(text);
		fov = N["settings"]["fov"].AsFloat;
		sensitivity = N["settings"]["sensitivity"].AsFloat;
		viewModelOn = false;
		if (N["settings"]["viewmodel"].AsFloat == 1) viewModelOn = true;
		
		masterVolume = N["audio"]["master"].AsFloat;
		musicVolume = N["audio"]["music"].AsFloat;
		soundEffectsVolume = N["audio"]["sound effects"].AsFloat;

		ambientOcclusion = false;
		vignetteAberration = false;
		edgeDetection = false;
		motionBlur = false;
		bloom = false;

		if(N["video"]["ambientOcclusion"].AsFloat == 1)ambientOcclusion=true;		
		if(N["video"]["bloom"].AsFloat == 1)bloom=true;
		if(N["video"]["edgeDetection"].AsFloat == 1)edgeDetection=true;
		if(N["video"]["motionBlur"].AsFloat == 1)motionBlur=true;		
		if(N["video"]["vignetteAberration"].AsFloat == 1)vignetteAberration=true;

	}

	public void Save(){
		updateValues ();
		File.WriteAllText(filename, SaveToString());
	}
	
	public void Load(){
		if (!File.Exists (filename)) {
			Save ();
		}
		string text = File.ReadAllText(filename);
		LoadString(text);
		fovSlider.GetComponent<UnityEngine.UI.Slider> ().value = fov;
		sensSlider.GetComponent<UnityEngine.UI.Slider> ().value = sensitivity;
		viewmodelToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = viewModelOn;

		masterSlider.GetComponent<UnityEngine.UI.Slider> ().value = masterVolume;
		musicSlider.GetComponent<UnityEngine.UI.Slider> ().value = musicVolume;
		soundEffectSlider.GetComponent<UnityEngine.UI.Slider> ().value = soundEffectsVolume;

		ambientOcclusionToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = ambientOcclusion;
		vignetteAberrationToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = vignetteAberration;
		edgeDetectionToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = edgeDetection;
		motionBlurToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = motionBlur;
		bloomToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = bloom;
	}

	public void updateValues(){
		fov = fovSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		sensitivity = sensSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		viewModelOn = viewmodelToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;

		masterVolume = masterSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		musicVolume = musicSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		soundEffectsVolume = soundEffectSlider.GetComponent<UnityEngine.UI.Slider> ().value;	

		ambientOcclusion = ambientOcclusionToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		vignetteAberration = vignetteAberrationToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		edgeDetection = edgeDetectionToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		motionBlur = motionBlurToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		bloom = bloomToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;

	}
	public void ChangeSensitivityText(float sens){		
		sensitivityText.text =sens.ToString("#.#");
	}
}
