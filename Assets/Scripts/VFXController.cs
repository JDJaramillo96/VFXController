using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class VFXController : MonoBehaviour {

    #region Properties

    [SerializeField]
    private Animator animator;

    [Space(10f)] [Header("Spell Settings")]

    [SerializeField]
    private GameObject spellSystem;

    [Space(10f)] [Header("States")]

    [SerializeField] [Range(0,1)]
    private float anticipationPercentage = 0.2f;
    [SerializeField] [Range(0,1)]
    private float actionPercentage = 0.25f;
    [SerializeField] [Range(0,1)]
    private float recuperationPercentage = 0.55f;

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
    private float bodyParticlesSize = 0.25f;
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
    private float fillIntensity = 2f;
    [SerializeField]
    private float rimIntensity = 1.5f;
    [SerializeField]
    private AnimationCurve lightFadeInCurve;
    [SerializeField]
    private AnimationCurve lightFadeOutCurve;

    [Space(10f)] [Header("Decal")]

    [SerializeField]
    private GameObject decal;
    [SerializeField]
    private AnimationCurve decalFadeInCurve;
    [SerializeField]
    private AnimationCurve decalFadeOutCurve;
    [SerializeField]
    private float decalMaxSize = 5f;

    [Space(10f)] [Header("Image Effects")]

    [SerializeField]
    private PostProcessingProfile profile;
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
    [SerializeField] [Range(0,2)]
    private float frameBlendingDelta = 0.25f;

    //Cached Components
    private MotionBlurModel.Settings motionBlurModel;
    private BloomModel.Settings bloomModel;
    private VignetteModel.Settings vignetteModel;

    [Space(10f)] [Header("Audio")]

    [SerializeField]
    private AudioSource spellAudio;
    [SerializeField]
    private AnimationCurve audioFadeInCurve;
    [SerializeField]
    private AnimationCurve audioFadeOutCurve;
    [SerializeField]
    private float volume = 0.1f;

    [Space(10f)] [Header("Others")]

    [SerializeField]
    private float globalLength = 2.25f;

    //Times
    private float globalTime = 0f;
    private float anticipationTime = 0f;
    private float actionTime = 0f;
    private float recuperationTime = 0f;
    private float stateEvaluateTime = 0f;

    //Times Lengths
    private float anticipationLength;
    private float actionLength;
    private float recuperationLength;
    private float lengthUntilAction;
    private float lengthUntilHalfOfAction;
    private float lengthUntilRecuperation;

    //Cached Components
    private float initialSpeed;
    private float scaledSpeed;
    private float scaledAcceleration;
    private float newScale;
    private IEnumerator spellCoroutine;

    //Readonly
    private readonly float clipLength = 2.267f;

    //Hidden
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public bool isSpellRuning = false;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        Setup();
    }

    private void OnDisable()
    {
        EffectsSettings();
    }

    #endregion

    #region Class Functions

    private void Setup()
    {
        SetupParticles();
        SetupLengths();
        SetupImageEffects();
        SetupAudio();
        SetupOthers();
    }

    private void SetupParticles()
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
        bodyParticles_Main.startSize = bodyParticlesSize;
    }

    private void SetupLengths()
    {
        //States Length
        anticipationLength = (globalLength * anticipationPercentage);
        actionLength = (globalLength * actionPercentage);
        recuperationLength = (globalLength * recuperationPercentage);

        //Lengths Until
        lengthUntilAction = anticipationLength + actionLength;
        lengthUntilHalfOfAction = anticipationLength + (actionLength / 2);
        lengthUntilRecuperation = lengthUntilAction + recuperationLength;
    }

    private void SetupImageEffects()
    {
        //Model Settings
        motionBlurModel = profile.motionBlur.settings;
        bloomModel = profile.bloom.settings;
        vignetteModel = profile.vignette.settings;

        EffectsSettings();

        //Parameters Compensation
        motionBlurFrameBlending -= initialMotionBlurFrameBlending;
        bloomIntensity -= initialBloomIntensity;
    }

    private void SetupAudio()
    {
        spellAudio.pitch = spellAudio.clip.length / globalLength;
    }

    private void SetupOthers()
    {
        scaledSpeed = mainParticlesSpeed * (clipLength / globalLength);
        scaledAcceleration = mainParticlesAcceleration * (clipLength / globalLength);
        initialSpeed = scaledSpeed;
    }

    private void EffectsSettings()
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

    #endregion

    #region Spell Functions

    public void ExecuteSpell()
    {
        animator.SetTrigger("spell1");

        //Coroutine Setup
        if (spellCoroutine != null)
            StopCoroutine(spellCoroutine);

        spellCoroutine = SpellCoroutine();

        StartCoroutine(spellCoroutine);
    }

    private void SetupSpell()
    {
        isSpellRuning = true;

        animator.speed = clipLength / globalLength;
        state = 1;

        spellSystem.SetActive(true);
    }

    private void SpellState1()
    {
        Debug.Log("STATE 1");

        stateEvaluateTime = anticipationTime / anticipationLength;

        if (!spellAudio.isPlaying)
            spellAudio.Play();

        //Particles Pass
        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(stateEvaluateTime);

        //Light Pass
        fill.intensity = lightFadeInCurve.Evaluate(stateEvaluateTime) * fillIntensity;
        rim1.intensity = lightFadeInCurve.Evaluate(stateEvaluateTime) * rimIntensity;
        rim2.intensity = lightFadeInCurve.Evaluate(stateEvaluateTime) * rimIntensity;

        //Decal Pass
        newScale = decalFadeInCurve.Evaluate(globalTime / lengthUntilHalfOfAction) * decalMaxSize;
        decal.transform.localScale = Vector3.one * newScale;

        //Image Effects Pass
        motionBlurModel.frameBlending = effectsFadeInCurve.Evaluate(stateEvaluateTime) * motionBlurFrameBlending + initialMotionBlurFrameBlending;
        bloomModel.bloom.intensity = effectsFadeInCurve.Evaluate(stateEvaluateTime) * bloomIntensity + initialBloomIntensity;
        bloomModel.bloom.threshold = inverseFadeInCurve.Evaluate(stateEvaluateTime) * initialBloomThreshold;
        vignetteModel.intensity = inverseFadeInCurve.Evaluate(stateEvaluateTime) * initialVignetteIntensity;

        profile.motionBlur.settings = motionBlurModel;
        profile.bloom.settings = bloomModel;
        profile.vignette.settings = vignetteModel;

        //Audio Pass
        spellAudio.volume = audioFadeInCurve.Evaluate(stateEvaluateTime) * volume;

        IncreaseTime();
    }

    private void SpellState2()
    {
        Debug.Log("STATE 2");

        stateEvaluateTime = actionTime / actionLength;

        //Particles Pass
        if (!mainParticles.gameObject.activeInHierarchy)
            mainParticles.gameObject.SetActive(true);

        mainParticles_Main.simulationSpeed = mainParticlesSimulationSpeedCurve.Evaluate(stateEvaluateTime) * scaledSpeed;

        //Decal Pass
        newScale = decalFadeInCurve.Evaluate(globalTime / lengthUntilHalfOfAction) * decalMaxSize;
        decal.transform.localScale = Vector3.one * newScale;

        IncreaseTime();
    }

    private void SpellState3()
    {
        Debug.Log("STATE 3");

        stateEvaluateTime = recuperationTime / recuperationLength;

        //Particles Pass
        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(1 - stateEvaluateTime);

        //Light Pass
        fill.intensity = lightFadeOutCurve.Evaluate(stateEvaluateTime) * fillIntensity;
        rim1.intensity = lightFadeOutCurve.Evaluate(stateEvaluateTime) * rimIntensity;
        rim2.intensity = lightFadeOutCurve.Evaluate(stateEvaluateTime) * rimIntensity;

        //Decal Pass
        newScale = decalFadeOutCurve.Evaluate(stateEvaluateTime) * decalMaxSize;
        decal.transform.localScale = Vector3.one * newScale;

        //Image Effects Pass
        motionBlurModel.frameBlending = effectsFadeOutCurve.Evaluate(recuperationTime / (recuperationLength - frameBlendingDelta)) * motionBlurFrameBlending + initialMotionBlurFrameBlending;
        bloomModel.bloom.intensity = effectsFadeOutCurve.Evaluate(stateEvaluateTime) * bloomIntensity + initialBloomIntensity;
        bloomModel.bloom.threshold = inverseFadeOutCurve.Evaluate(stateEvaluateTime) * initialBloomThreshold;
        vignetteModel.intensity = inverseFadeOutCurve.Evaluate(stateEvaluateTime) * initialVignetteIntensity;

        profile.motionBlur.settings = motionBlurModel;
        profile.bloom.settings = bloomModel;
        profile.vignette.settings = vignetteModel;

        //Audio Pass
        spellAudio.volume = audioFadeOutCurve.Evaluate(stateEvaluateTime) * volume;

        IncreaseTime();
    }

    private void EndSpell()
    {
        isSpellRuning = false;

        animator.speed = 1f;
        state = 0;

        //Timing
        globalTime = 0f;
        anticipationTime = 0f;
        actionTime = 0f;
        recuperationTime = 0f;
        stateEvaluateTime = 0f;

        //Particles
        mainParticles_Main.simulationSpeed = 0f;
        mainParticles.gameObject.SetActive(false);

        //Lighting
        fill.intensity = 0f;
        rim1.intensity = 0f;
        rim2.intensity = 0f;

        //Decal
        decal.transform.localScale = Vector3.zero;

        //Image Effects
        EffectsSettings();

        //Audio
        spellAudio.volume = 0f;
        
        //Others
        scaledSpeed = initialSpeed;

        spellSystem.SetActive(false);
    }

    private void IncreaseTime()
    {
        globalTime += Time.deltaTime;

        switch (state)
        {
            case 1:
                anticipationTime += Time.deltaTime;
                break;

            case 2:
                actionTime += Time.deltaTime;
                break;

            case 3:
                recuperationTime += Time.deltaTime;
                break;

            default:
                break;
        }

        scaledSpeed += scaledAcceleration;
    }

    private bool EvaluateState(float stateTime, float stateLength)
    {
        if (stateTime < stateLength)
            return true;
        else
            return false;
    }

    #endregion

    #region Spell Coroutines

    private IEnumerator SpellCoroutine()
    {
        SetupSpell();

        while (state == 1)
        {
            SpellState1();

            if (!EvaluateState(globalTime, anticipationLength))
                state = 2;

            yield return null;
        }

        while (state == 2)
        {
            SpellState2();

            if (!EvaluateState(globalTime, lengthUntilAction))
                state = 3;

            yield return null;
        }

        while (state == 3)
        {
            SpellState3();

            if (!EvaluateState(globalTime, lengthUntilRecuperation))
                EndSpell();

            yield return null;
        }
    }

    #endregion
}
