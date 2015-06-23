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
	public GameObject bloomToggle;	
	public GameObject motionBlurToggle;

	[Header("Settings")]
	public float fov = 60f;
	public float sensitivity = 2f;
	public bool viewModelOn = true;
	
	public float masterVolume = 10f;
	public float musicVolume = 10f;
	public float soundEffectsVolume = 10f;	

	public bool motionBlur = false;
	public bool bloom = true;

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
	
		N["video"]["bloom"].AsFloat = bloom ? 1 : 0;
		N["video"]["motionBlur"].AsFloat = motionBlur ? 1 : 0;		

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

		motionBlur = false;
		bloom = false;
	
		if(N["video"]["bloom"].AsFloat == 1)bloom=true;
		if(N["video"]["motionBlur"].AsFloat == 1)motionBlur=true;		

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

		motionBlur = motionBlurToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		bloom = bloomToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;

	}
	public void ChangeSensitivityText(float sens){		
		sensitivityText.text =sens.ToString("#.#");
	}
}
