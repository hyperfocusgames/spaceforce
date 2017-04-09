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

	int pushEnabledStatic_ID;
	int pullEnabledStatic_ID;

	int pushEnabledLocal_ID;
	int pullEnabledLocal_ID;

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

		pushEnabledStatic_ID = Shader.PropertyToID("_PushEnabledStatic");
		pullEnabledStatic_ID = Shader.PropertyToID("_PullEnabledStatic");

		pushEnabledLocal_ID = Shader.PropertyToID("_PushEnabledLocal");
		pullEnabledLocal_ID = Shader.PropertyToID("_PullEnabledLocal");

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

	public static void SetStaticRipple(bool enabled, MagnetController.Polarity polarity, Vector3 center) {
		switch (polarity) {
			case MagnetController.Polarity.Push:
				Shader.SetGlobalVector(instance.pushCenter_ID, center);
				Shader.SetGlobalFloat(instance.pushEnabledStatic_ID, enabled ? 1 : 0);
				break;
			case MagnetController.Polarity.Pull:
				Shader.SetGlobalVector(instance.pullCenter_ID, center);
				Shader.SetGlobalFloat(instance.pullEnabledStatic_ID, enabled ? 1 : 0);
				break;
		}
	}

	private static MaterialPropertyBlock block;
	public static void SetLocalRipple(bool enabled, MagnetController.Polarity polarity, Vector3 center, Magnetic target) {
		if (block == null) block = new MaterialPropertyBlock();
		target.render.GetPropertyBlock(block);
		switch (polarity) {
			case MagnetController.Polarity.Push:
				block.SetVector(instance.pushCenter_ID, center);
				block.SetFloat(instance.pushEnabledLocal_ID, enabled ? 1 : 0);
				break;
			case MagnetController.Polarity.Pull:
				block.SetVector(instance.pullCenter_ID, center);
				block.SetFloat(instance.pullEnabledLocal_ID, enabled ? 1 : 0);
				break;
		}
		target.render.SetPropertyBlock(block);
	}

}