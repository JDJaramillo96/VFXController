using UnityEngine;
using UnityEngine.PostProcessing;

public class Spell01 : Spell {

    #region Properties

    [Space(10f)] [Header("Spell Settings")]

    [SerializeField]
    private GameObject spellSystem;

    [Space(10f)] [Header("Main Particles Settings")]

    [SerializeField]
    private ParticleSystem mainParticles;
    [SerializeField]
    private float mainParticlesSize = 0.8f;
    [SerializeField]
    private float mainParticlesRadius = 0.8f;
    [SerializeField]
    private float mainParticlesSpeed = 4f;
    [SerializeField]
    private float mainParticlesAcceleration = 0.025f;
    [SerializeField]
    private AnimationCurve mainParticlesSimulationSpeedCurve;
    [SerializeField]
    private Color mainParticlesColor;
    [SerializeField]
    private Gradient mainParticlesColorOverLifeTime;
    
    //Cached Modules
    private ParticleSystem.MainModule mainParticles_Main;

    [Space(10f)] [Header("Body Particles Settings")]

    [SerializeField]
    private ParticleSystem bodyParticles;
    [SerializeField]
    private float bodyParticlesInitialSize = 0.25f;
    [SerializeField]
    private Gradient bodyParticlesGradient;

    //Cached Modules
    private ParticleSystem.MainModule bodyParticles_Main;

    [Space(10f)] [Header("Lights")]

    [SerializeField]
    private Light fill;
    [SerializeField]
    private Light rim1;
    [SerializeField]
    private Light rim2;
    [SerializeField]
    private Gradient fillGradient;
    [SerializeField]
    private Gradient rim1Gradient;
    [SerializeField]
    private Gradient rim2Gradient;
    [SerializeField]
    private float fillIntensity = 2f;
    [SerializeField]
    private float rimIntensity = 1.5f;
    [SerializeField]
    private AnimationCurve lightFadeInCurve;
    [SerializeField]
    private AnimationCurve lightFadeOutCurve;

    //Initial values
    private Color initialFillColor;
    private Color initialRim1Color;
    private Color initialRim2Color;

    [Space(10f)] [Header("Decal Settings")]

    [SerializeField]
    private GameObject decal;
    [SerializeField]
    private AnimationCurve decalFadeInCurve;
    [SerializeField]
    private AnimationCurve decalFadeOutCurve;
    [SerializeField]
    private float decalMaxSize = 5f;

    [Space(10f)] [Header("Effects Settings")]

    [SerializeField]
    private float motionBlurFrameBlending = 0.8f;
    [SerializeField]
    private float initialMotionBlurFrameBlending = 0.1f;
    [SerializeField]
    private float bloomIntensity = 0.6f;
    [SerializeField]
    private float initialBloomIntensity = 0.15f;
    [SerializeField]
    private float initialBloomThreshold = 1f;
    [SerializeField]
    private float initialVignetteIntensity = 0.225f;
    [SerializeField]
    private AnimationCurve effectsFadeInCurve;
    [SerializeField]
    private AnimationCurve inverseFadeInCurve;
    [SerializeField]
    private AnimationCurve effectsFadeOutCurve;
    [SerializeField]
    private AnimationCurve inverseFadeOutCurve;
    [SerializeField] [Range(0,1)]
    private float frameBlendingDelta = 0.25f;

    //Temp

    float motionBlurFrameBlendingTemp;
    float bloomIntensityTemp;

    //Cached Components
    private MotionBlurModel.Settings motionBlurModel;
    private BloomModel.Settings bloomModel;
    private VignetteModel.Settings vignetteModel;

    [Space(10f)] [Header("Audio Settings")]

    [SerializeField]
    private AudioSource spellAudio;
    [SerializeField]
    private AnimationCurve audioFadeInCurve;
    [SerializeField]
    private AnimationCurve audioFadeOutCurve;
    [SerializeField]
    private float volume = 0.25f;

    //Speed Settings
    private float initialSpeed;
    private float scaledSpeed;
    private float scaledAcceleration;
    private float decalNewScale;

    #endregion

    #region Class Functions

    protected override void SetupLengths()
    {
        if (clipLength != 2.267f)
            clipLength = 2.267f;

        base.SetupLengths();
    }

    protected override void SetupParticles()
    {
        //Main Particles
        mainParticles_Main = mainParticles.main;
        mainParticles_Main.startSize = mainParticlesSize;
        mainParticles_Main.startColor = mainParticlesColor;
        ParticleSystem.ShapeModule mainParticles_Shape = mainParticles.shape;
        mainParticles_Shape.radius = mainParticlesRadius;
        ParticleSystem.ColorOverLifetimeModule mainParticles_ColorOverLifeTime = mainParticles.colorOverLifetime;
        mainParticles_ColorOverLifeTime.color = mainParticlesColorOverLifeTime;

        //Body Particles
        bodyParticles_Main = bodyParticles.main;
        bodyParticles_Main.startSize = bodyParticlesInitialSize;
    }

    protected override void SetupLighting()
    {
        initialFillColor = fillGradient.colorKeys[0].color;
        initialRim1Color = rim1Gradient.colorKeys[0].color;
        initialRim2Color = rim2Gradient.colorKeys[0].color;
    }

    protected override void SetupEffects()
    {
        //Model Settings
        motionBlurModel = profile.motionBlur.settings;
        bloomModel = profile.bloom.settings;
        vignetteModel = profile.vignette.settings;

        InitialEffectsSettings();

        //Parameters Compensation
        motionBlurFrameBlending -= initialMotionBlurFrameBlending;
        bloomIntensity -= initialBloomIntensity;
    }

    protected override void SetupAudio()
    {
        spellAudio.pitch = spellAudio.clip.length / Mathf.Clamp(globalLength, 0.0001f, 1f);
    }

    protected override void SetupOthers()
    {
        scaledSpeed = mainParticlesSpeed * (clipLength / globalLength);
        scaledAcceleration = mainParticlesAcceleration * (clipLength / globalLength);
        initialSpeed = scaledSpeed;
    }

    protected override void InitialEffectsSettings()
    {
        //Initial Parameters
        motionBlurModel.frameBlending = initialMotionBlurFrameBlending;
        bloomModel.bloom.intensity = initialBloomIntensity;
        bloomModel.bloom.threshold = initialBloomThreshold;
        vignetteModel.intensity = initialVignetteIntensity;

        //PostProcessingProfile Implementation
        profile.motionBlur.settings = motionBlurModel;
        profile.bloom.settings = bloomModel;
        profile.vignette.settings = vignetteModel;
    }

    protected override void SetTempProperties()
    {
        motionBlurFrameBlendingTemp = motionBlurFrameBlending;
        bloomIntensityTemp = bloomIntensity;
    }

    protected override void ResetTempProperties()
    {
        motionBlurFrameBlending = motionBlurFrameBlendingTemp;
        bloomIntensity = bloomIntensityTemp;
    }

    #endregion

    #region Spell Functions

    protected override void SetupSpell()
    {
        base.SetupSpell();

        spellSystem.SetActive(true);

        if (!spellAudio.isPlaying)
            spellAudio.Play();
    }

    protected override void SpellState1()
    {
        base.SpellState1();

        //Particles Pass
        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(stateTime);

        //Lighting Pass
        fill.color = fillGradient.Evaluate(stateTime);
        rim1.color = rim1Gradient.Evaluate(stateTime);
        rim2.color = rim2Gradient.Evaluate(stateTime);
        fill.intensity = lightFadeInCurve.Evaluate(stateTime) * fillIntensity;
        rim1.intensity = lightFadeInCurve.Evaluate(stateTime) * rimIntensity;
        rim2.intensity = lightFadeInCurve.Evaluate(stateTime) * rimIntensity;

        //Decal Pass
        decalNewScale = decalFadeInCurve.Evaluate(globalTime / lengthUntilHalfOfAction) * decalMaxSize;
        decal.transform.localScale = Vector3.one * decalNewScale;

        //Effects Pass
        motionBlurModel.frameBlending = Mathf.Clamp01(effectsFadeInCurve.Evaluate(stateTime) * motionBlurFrameBlending + initialMotionBlurFrameBlending);
        bloomModel.bloom.intensity = Mathf.Clamp01(effectsFadeInCurve.Evaluate(stateTime) * bloomIntensity + initialBloomIntensity);
        bloomModel.bloom.threshold = Mathf.Clamp01(inverseFadeInCurve.Evaluate(stateTime) * initialBloomThreshold);
        vignetteModel.intensity = Mathf.Clamp01(inverseFadeInCurve.Evaluate(stateTime) * initialVignetteIntensity);

        profile.motionBlur.settings = motionBlurModel;
        profile.bloom.settings = bloomModel;
        profile.vignette.settings = vignetteModel;

        //Audio Pass
        spellAudio.volume = audioFadeInCurve.Evaluate(stateTime) * volume;

        //Others
        IncreaseTime();
    }

    protected override void SpellState2()
    {
        base.SpellState2();

        //Particles Pass
        if (!mainParticles.gameObject.activeInHierarchy)
            mainParticles.gameObject.SetActive(true);

        mainParticles_Main.simulationSpeed = mainParticlesSimulationSpeedCurve.Evaluate(stateTime) * scaledSpeed;

        //Decal Pass
        decalNewScale = decalFadeInCurve.Evaluate(globalTime / lengthUntilHalfOfAction) * decalMaxSize;
        decal.transform.localScale = Vector3.one * decalNewScale;

        //Others
        IncreaseTime();
    }

    protected override void SpellState3()
    {
        base.SpellState3();

        //Particles Pass
        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(stateTimeComplement);

        //Lighting Pass
        fill.color = fillGradient.Evaluate(stateTimeComplement);
        rim1.color = rim1Gradient.Evaluate(stateTimeComplement);
        rim2.color = rim2Gradient.Evaluate(stateTimeComplement);

        fill.intensity = lightFadeOutCurve.Evaluate(stateTime) * fillIntensity;
        rim1.intensity = lightFadeOutCurve.Evaluate(stateTime) * rimIntensity;
        rim2.intensity = lightFadeOutCurve.Evaluate(stateTime) * rimIntensity;

        //Decal Pass
        decalNewScale = decalFadeOutCurve.Evaluate(stateTime) * decalMaxSize;
        decal.transform.localScale = Vector3.one * decalNewScale;

        //Effects Pass
        motionBlurModel.frameBlending = Mathf.Clamp01(effectsFadeOutCurve.Evaluate(recuperationTime / (recuperationLength - frameBlendingDelta)) * motionBlurFrameBlending + initialMotionBlurFrameBlending);
        bloomModel.bloom.intensity = Mathf.Clamp01(effectsFadeOutCurve.Evaluate(stateTime) * bloomIntensity + initialBloomIntensity);
        bloomModel.bloom.threshold = Mathf.Clamp01(inverseFadeOutCurve.Evaluate(stateTime) * initialBloomThreshold);
        vignetteModel.intensity = Mathf.Clamp01(inverseFadeOutCurve.Evaluate(stateTime) * initialVignetteIntensity);

        profile.motionBlur.settings = motionBlurModel;
        profile.bloom.settings = bloomModel;
        profile.vignette.settings = vignetteModel;

        //Audio Pass
        spellAudio.volume = audioFadeOutCurve.Evaluate(stateTime) * volume;

        //Others
        IncreaseTime();
    }

    protected override void EndSpell()
    {
        base.EndSpell();

        //Particles
        mainParticles_Main.simulationSpeed = 0f;
        mainParticles.gameObject.SetActive(false);

        //Lighting
        fill.color = initialFillColor;
        rim1.color = initialRim1Color;
        rim2.color = initialRim2Color;

        fill.intensity = 0f;
        rim1.intensity = 0f;
        rim2.intensity = 0f;

        //Decal
        decal.transform.localScale = Vector3.zero;

        //Effects
        InitialEffectsSettings();

        //Audio
        spellAudio.volume = 0f;
        
        //Others
        scaledSpeed = initialSpeed;

        spellSystem.SetActive(false);
    }

    protected override void IncreaseTime()
    {
        base.IncreaseTime();

        scaledSpeed += scaledAcceleration;
    }

    #endregion
}
