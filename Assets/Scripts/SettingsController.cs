using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;

public class SettingsController : MonoBehaviour {

	[Header("UI Elements")]
	public UnityEngine.UI.Text sensitivityText;
	
	public GameObject fovSlider;
	public GameObject sensSlider;
	public GameObject viewmodelToggle;
	
	public GameObject masterSlider;
	public GameObject musicSlider;
	public GameObject soundEffectSlider;

	[Header("Settings")]
	public float fov = 60f;
	public float sensitivity = 2f;
	public bool viewModelOn = true;
	
	public float masterVolume = 10f;
	public float musicVolume = 10f;
	public float soundEffectsVolume = 10f;	

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
	}

	public void updateValues(){
		fov = fovSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		sensitivity = sensSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		viewModelOn = viewmodelToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn;
		masterVolume = masterSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		musicVolume = musicSlider.GetComponent<UnityEngine.UI.Slider> ().value;
		soundEffectsVolume = soundEffectSlider.GetComponent<UnityEngine.UI.Slider> ().value;		
	}
	public void ChangeSensitivityText(float sens){		
		sensitivityText.text =sens.ToString("#.#");
	}
}
