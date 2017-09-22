﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Spell02 : Spell {

    #region Properties

    [Header("Spell Settings")] [Space(10f)]

    [SerializeField]
    private GameObject spellSystem;

    [Header("Main Particles Settings")] [Space(10f)]

    [SerializeField]
    private ParticleSystem mainParticles;
    [SerializeField]
    private float mainParticlesInitialSimulationSpeed;
    [SerializeField]
    private AnimationCurve mainParticlesSimulationSpeedCurve;
    [SerializeField]
    private Gradient mainParticlesGradient;

    //Cached Modules
    private ParticleSystem.MainModule mainParticles_Main;

    [Space(10f)] [Header("Body Particles Settings")]

    [SerializeField]
    private ParticleSystem bodyParticles;
    [SerializeField]
    private Gradient bodyParticlesGradient;

    //Cached Modules
    private ParticleSystem.MainModule bodyParticles_Main;

    [Header("Lights")] [Space(10f)]

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
    private float rimIntensity = 1.25f;
    [SerializeField]
    private float fillRange = 3f;
    [SerializeField]
    private float rimRange = 3f;
    [SerializeField]
    private AnimationCurve lightFadeInCurve;
    [SerializeField]
    private AnimationCurve lightFadeOutCurve;

    //Initial values
    private Color initialFillColor;
    private Color initialRim1Color;
    private Color initialRim2Color;

    [Header("Decal Settings")] [Space(10f)]

    [SerializeField]
    private GameObject decal;
    [SerializeField]
    private AnimationCurve decalFadeInCurve;
    [SerializeField]
    private AnimationCurve decalFadeOutCurve;
    [SerializeField]
    private float decalMaxSize;
    [SerializeField]
    private float rotationVelocity;
    [SerializeField]
    private AnimationCurve rotationAcelerationFadeIn;
    [SerializeField]
    private AnimationCurve rotationAcelerationFadeOut;

    [Space(10f)] [Header("Effects Settings")]

    [SerializeField]
    private float motionBlurFrameBlending = 0.8f;
    [SerializeField]
    private float initialMotionBlurFrameBlending = 0.1f;
    [SerializeField]
    private float grainIntensity = 0f;
    [SerializeField]
    private float initialGrainIntensity = 0f;
    [SerializeField]
    private float initialGrainSize = 0f;
    [SerializeField]
    private float vignetteIntensity = 0.45f;
    [SerializeField]
    private float initialVignetteIntensity = 0.225f;
    [SerializeField]
    private AnimationCurve effectsFadeInCurve;
    [SerializeField]
    private AnimationCurve effectsFadeOutCurve;
    [SerializeField] [Range(0, 1)]
    private float frameBlendingDelta = 0.25f;

    [Space(10f)] [Header("Audio Settings")]

    [SerializeField]
    private AudioSource spellAudio;
    [SerializeField]
    private AnimationCurve audioFadeInCurve;
    [SerializeField]
    private AnimationCurve audioFadeOutCurve;
    [SerializeField]
    private float volume = 0.25f;

    //Cached Components
    private MotionBlurModel.Settings motionBlurModel;
    private GrainModel.Settings grainModel;
    private VignetteModel.Settings vignetteModel;

    //Timming Settings
    private float timeUntilHalfOfAction;

    #endregion

    #region Class Functions

    protected override void SetupLengths()
    {
        base.SetupLengths();

        clipLength = 2.967f;
    }

    protected override void SetupParticles()
    {
        //Main Particles
        mainParticlesInitialSimulationSpeed *= (clipLength / globalLength);

        mainParticles_Main = mainParticles.main;
        mainParticles_Main.simulationSpeed = mainParticlesInitialSimulationSpeed;

        bodyParticles_Main = bodyParticles.main;
    }

    protected override void SetupLighting()
    {
        fill.range = fillRange;
        rim1.range = rimRange;
        rim2.range = rimRange;

        initialFillColor = fillGradient.colorKeys[0].color;
        initialRim1Color = rim1Gradient.colorKeys[0].color;
        initialRim2Color = rim2Gradient.colorKeys[0].color;
    }

    protected override void SetupEffects()
    {
        motionBlurModel = profile.motionBlur.settings;
        grainModel = profile.grain.settings;
        vignetteModel = profile.vignette.settings;

        EffectsSettings();

        //Parameters Compensation
        motionBlurFrameBlending -= initialMotionBlurFrameBlending;
        grainIntensity -= initialGrainIntensity;
        vignetteIntensity -= initialVignetteIntensity;
    }

    protected override void SetupAudio()
    {
        spellAudio.pitch = spellAudio.clip.length / globalLength;
    }

    protected override void EffectsSettings()
    {
        //Initial Parameters
        motionBlurModel.frameBlending = initialMotionBlurFrameBlending;
        grainModel.intensity = initialGrainIntensity;
        grainModel.size = initialGrainSize;
        vignetteModel.intensity = initialVignetteIntensity;

        //PostProcessingProfile Implementation
        profile.motionBlur.settings = motionBlurModel;
        profile.grain.settings = grainModel;
        profile.vignette.settings = vignetteModel;

        profile.grain.enabled = false;
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

        timeUntilHalfOfAction = globalTime / lengthUntilHalfOfAction;

        //Particles Pass
        if (!mainParticles.gameObject.activeInHierarchy)
            mainParticles.gameObject.SetActive(true);

        mainParticles_Main.startColor = mainParticlesGradient.Evaluate(timeUntilHalfOfAction);
        mainParticles_Main.simulationSpeed = mainParticlesInitialSimulationSpeed + (mainParticlesSimulationSpeedCurve.Evaluate(stateTime));

        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(stateTime);

        //Lighting Pass
        fill.color = fillGradient.Evaluate(stateTime);
        rim1.color = rim1Gradient.Evaluate(stateTime);
        rim2.color = rim2Gradient.Evaluate(stateTime);

        fill.intensity = lightFadeInCurve.Evaluate(stateTime) * fillIntensity;
        rim1.intensity = lightFadeInCurve.Evaluate(stateTime) * rimIntensity;
        rim2.intensity = lightFadeInCurve.Evaluate(stateTime) * rimIntensity;

        //Decal Pass
        decal.transform.localScale = Vector3.one * (decalFadeInCurve.Evaluate(timeUntilHalfOfAction) * decalMaxSize);
        decal.transform.Rotate(Vector3.up * (rotationVelocity * rotationAcelerationFadeIn.Evaluate(timeUntilHalfOfAction) * Time.deltaTime), Space.World);

        //Effects Pass
        if (!profile.grain.enabled)
            profile.grain.enabled = true;

        motionBlurModel.frameBlending = effectsFadeInCurve.Evaluate(stateTime) * motionBlurFrameBlending + initialMotionBlurFrameBlending;
        grainModel.intensity = effectsFadeInCurve.Evaluate(stateTime) * grainIntensity + initialGrainIntensity;
        vignetteModel.intensity = effectsFadeInCurve.Evaluate(stateTime) * vignetteIntensity + initialVignetteIntensity;

        profile.motionBlur.settings = motionBlurModel;
        profile.grain.settings = grainModel;
        profile.vignette.settings = vignetteModel;

        //Audio Pass
        spellAudio.volume = audioFadeInCurve.Evaluate(stateTime) * volume;

        //Others
        IncreaseTime();
    }

    protected override void SpellState2()
    {
        base.SpellState2();

        timeUntilHalfOfAction = globalTime / lengthUntilHalfOfAction;

        //Particles Pass
        mainParticles_Main.startColor = mainParticlesGradient.Evaluate(timeUntilHalfOfAction);

        //Decal Pass
        decal.transform.localScale = Vector3.one * (decalFadeInCurve.Evaluate(timeUntilHalfOfAction) * decalMaxSize);
        decal.transform.Rotate(Vector3.up * (rotationVelocity * rotationAcelerationFadeIn.Evaluate(timeUntilHalfOfAction) * Time.deltaTime), Space.World);

        //Others
        IncreaseTime();
    }

    protected override void SpellState3()
    {
        base.SpellState3();

        //Particles Pass
        mainParticles_Main.startColor = mainParticlesGradient.Evaluate(stateTimeComplement);
        bodyParticles_Main.startColor = bodyParticlesGradient.Evaluate(stateTimeComplement);

        //Lighting Pass
        fill.color = fillGradient.Evaluate(stateTimeComplement);
        rim1.color = rim1Gradient.Evaluate(stateTimeComplement);
        rim2.color = rim2Gradient.Evaluate(stateTimeComplement);

        fill.intensity = lightFadeOutCurve.Evaluate(stateTime) * fillIntensity;
        rim1.intensity = lightFadeOutCurve.Evaluate(stateTime) * rimIntensity;
        rim2.intensity = lightFadeOutCurve.Evaluate(stateTime) * rimIntensity;

        //Decal Pass
        decal.transform.localScale = Vector3.one * (decalFadeOutCurve.Evaluate(stateTime) * decalMaxSize);
        decal.transform.Rotate(Vector3.up * (rotationVelocity * rotationAcelerationFadeOut.Evaluate(stateTime) * Time.deltaTime), Space.World);

        //Effects Pass
        motionBlurModel.frameBlending = effectsFadeOutCurve.Evaluate(recuperationTime / (recuperationLength - frameBlendingDelta)) * motionBlurFrameBlending + initialMotionBlurFrameBlending;
        grainModel.intensity = effectsFadeOutCurve.Evaluate(stateTime) * grainIntensity + initialGrainIntensity;
        vignetteModel.intensity = effectsFadeOutCurve.Evaluate(stateTime) * vignetteIntensity + initialVignetteIntensity;

        //Audio Pass
        spellAudio.volume = audioFadeOutCurve.Evaluate(stateTime) * volume;

        profile.motionBlur.settings = motionBlurModel;
        profile.grain.settings = grainModel;
        profile.vignette.settings = vignetteModel;

        //Others
        IncreaseTime();
    }

    protected override void EndSpell()
    {
        base.EndSpell();

        //Particles
        mainParticles_Main.simulationSpeed = mainParticlesInitialSimulationSpeed;
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
        EffectsSettings();

        //Others
        spellSystem.SetActive(false);
    }

    #endregion
}