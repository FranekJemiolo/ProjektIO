using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkirmishOptions : MonoBehaviour {

	private const int MinNumberOfAsteroids = 1;
	private const int MaxNumberOfAsteroids = 10;
	private const int MinNumberOfCredits = 1000;
	private const int MaxNumberOfCredits = 10000;
	private const int StepValue = 500;

	public Slider AsteroidsSlider;
	public Slider Player1Slider;
	public Slider Player2Slider;

	public Text AsteroidsText;
	public Text Player1Text;
	public Text Player2Text;
	
	public Text SmallText;
	public Text MediumText;
	public Text LargeText;

	// 0 - small, 1 - medium, 2 - large
	private int currentSizeSelected; 

	public void MakeActiveText(Text t) {
		t.color = Color.cyan;	
	}

	public void MakeDisabledText(Text t) {
		t.color = Color.gray;
	}

	// 0 - small, 1 - medium, 2 - large
	public void ActivateSize(int i) {
		currentSizeSelected = i;

		// love thy hardcoded code
		if (i == 0) {
			MakeActiveText(SmallText);
			MakeDisabledText(MediumText);
			MakeDisabledText(LargeText);
		} else if (i == 1) {
			MakeActiveText(MediumText);
			MakeDisabledText(SmallText);
			MakeDisabledText(LargeText);
		} else {
			MakeActiveText(LargeText);
			MakeDisabledText(MediumText);
			MakeDisabledText(SmallText);
		}

	}

	private void SetupSlider(Slider s, int min, int max, int step) {
		s.minValue = min;
		s.maxValue = max;
		// no function for step;
	}

	public void DrawInfo() {
		AsteroidsText.text = AsteroidsSlider.value.ToString();
		Player1Text.text = Player1Slider.value.ToString();
		Player2Text.text = Player2Slider.value.ToString();
	}

	// Use this for initialization
	void Start () {
		ActivateSize(0);
		SetupSlider(AsteroidsSlider, MinNumberOfAsteroids, MaxNumberOfAsteroids, 1);
		SetupSlider (Player1Slider, MinNumberOfCredits, MaxNumberOfCredits, 100);
		SetupSlider (Player2Slider, MinNumberOfCredits, MaxNumberOfCredits, 100);
	}

	public void GoBackToMenu() {
		Application.LoadLevel("Menu");
	}

	public void StartGame() {
		//TODO
	}

	private void SnapToDesiredValues() {
		Player1Slider.value = Player1Slider.value - (Player1Slider.value % StepValue);
		Player2Slider.value = Player2Slider.value - (Player2Slider.value % StepValue);
	}

	// Update is called once per frame
	void Update () {
		// me being lazy
		SnapToDesiredValues();
		DrawInfo ();
	}
}
