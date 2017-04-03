using UnityEngine;

public class SurfaceRippleEffect : SingletonBehaviour<SurfaceRippleEffect> {

	public float wavelength = 0.1f;
	public float speed = 15;
	public float sharpness = 5;
	public float radius = 1.5f;
	public float pulseWidth = 0.25f;

	public Color pushColor = Color.red;
	public Color pullColor = Color.blue;

	Camera mainCam;
	Camera effectCam;
	Shader rippleEffectShader;

	int wavelength_ID;
	int speed_ID;
	int sharpness_ID;
	int radius_ID;
	int pulseWidth_ID;

	int pushColor_ID;
	int pullColor_ID;

	int pushCenter_ID;
	int pullCenter_ID;

	int pushEnabled_ID;
	int pullEnabled_ID;

	void Awake() {

		wavelength_ID = Shader.PropertyToID("_Wavelength");
		speed_ID = Shader.PropertyToID("_Speed");
		sharpness_ID = Shader.PropertyToID("_Sharpness");
		radius_ID = Shader.PropertyToID("_Radius");
		pulseWidth_ID = Shader.PropertyToID("_PulseWidth");

		pushColor_ID = Shader.PropertyToID("_PushColor");
		pullColor_ID = Shader.PropertyToID("_PullColor");

		pushCenter_ID = Shader.PropertyToID("_PushCenter");
		pullCenter_ID = Shader.PropertyToID("_PullCenter");

		pushEnabled_ID = Shader.PropertyToID("_PushEnabled");
		pullEnabled_ID = Shader.PropertyToID("_PullEnabled");

		mainCam = GetComponent<Camera>();
		effectCam = new GameObject("surface ripple effect camera").AddComponent<Camera>();
		effectCam.transform.parent = transform;
		rippleEffectShader = Shader.Find("Magnet/Surface Ripples");
		UpdateEffectCamera();
		UpdateShaderGlobals();
	}

	public void UpdateEffectCamera() {
		effectCam.CopyFrom(mainCam);
		effectCam.clearFlags = CameraClearFlags.Depth;
		effectCam.SetReplacementShader(rippleEffectShader, "");
	}

	public void UpdateShaderGlobals() {
		Shader.SetGlobalFloat(wavelength_ID, wavelength);
		Shader.SetGlobalFloat(speed_ID, speed);
		Shader.SetGlobalFloat(sharpness_ID, sharpness);
		Shader.SetGlobalFloat(radius_ID, radius);
		Shader.SetGlobalFloat(pulseWidth_ID, pulseWidth);

		Shader.SetGlobalColor(pushColor_ID, pushColor);
		Shader.SetGlobalColor(pullColor_ID, pullColor);
	}

	public static void EnableRipple(MagnetController.Polarity polarity, Vector3 center) {
		switch (polarity) {
			case MagnetController.Polarity.Push:
				Shader.SetGlobalVector(instance.pushCenter_ID, center);
				Shader.SetGlobalFloat(instance.pushEnabled_ID, 1);
				break;
			case MagnetController.Polarity.Pull:
				Shader.SetGlobalVector(instance.pullCenter_ID, center);
				Shader.SetGlobalFloat(instance.pullEnabled_ID, 1);
				break;
		}
	}

	public static void DisableRipple(MagnetController.Polarity polarity) {
		switch (polarity) {
			case MagnetController.Polarity.Push:
				Shader.SetGlobalFloat(instance.pushEnabled_ID, 0);
				break;
			case MagnetController.Polarity.Pull:
				Shader.SetGlobalFloat(instance.pullEnabled_ID, 0);
				break;
		}
	}

}