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

	public GameObject LoadingScreen;

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
		LoadingScreen.SetActive(false);
	}

	public void GoBackToMenu() {
		Application.LoadLevel("Menu");
	}

	public float resolveMapSize() {
		return (float) (500.0f * (currentSizeSelected + 1)); 
	}

	public void StartGame() {
		//resolve size map, 1500.0f 
		StupidVector3<float>[] astero = GameController.getRandomAsteroids(resolveMapSize() , (int) AsteroidsSlider.value);
		StupidVector3<float> massE = GameController.getRandomMassRelayE(resolveMapSize());
		StupidVector3<float> massP = GameController.getRandomMassRelayP(resolveMapSize());
		float startCredits = Player1Slider.value;
		float pA = 1.0f;
		float pForKilling = 50.0f;
		float pointsToWin = Player2Slider.value;
		float minX = 0.0f;
		float minZ = 0.0f;
		float maxX = resolveMapSize();
		float maxZ = resolveMapSize();
		float cB = 1.0f;

		GameController.Presets preset = new GameController.Presets(startCredits, pA,
		                                                           pForKilling,
		                                                           cB,
		                                                           pointsToWin,
		                                                           minX,
		                                                           maxX,
		                                                           minZ,
		                                                           maxZ,
		                                                           astero,
		                                                           massP,
		                                                           massE);
		                                                          
		GameController.savePresets(preset);
		LoadingScreen.SetActive(true);
		Application.LoadLevel("16");
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
