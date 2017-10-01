using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public abstract class Spell : MonoBehaviour {

    #region Properties

    public float spellSpeed
    {
        get; protected set;
    }

    [Space(10f)] [Header("Animator Settings")]

    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected string animationTriggerName;

    [Space(10f)] [Header("States")]

    [SerializeField] [Range(0, 1)]
    protected float anticipationPercentage;
    [SerializeField] [Range(0, 1)]
    protected float actionPercentage;
    [SerializeField] [Range(0, 1)]
    protected float recuperationPercentage;

    [Space(10f)] [Header("PostProcessing Profile")]

    [SerializeField]
    protected PostProcessingProfile profile;

    //Times
    protected float globalTime = 0f;
    protected float anticipationTime = 0f;
    protected float actionTime = 0f;
    protected float recuperationTime = 0f;
    protected float stateTime = 0f;
    protected float stateTimeComplement = 1f;

    //Times Lengths
    protected float anticipationLength;
    protected float actionLength;
    protected float recuperationLength;
    protected float lengthUntilAction;
    protected float lengthUntilHalfOfAction;
    protected float lengthUntilRecuperation;

    //Cached Components
    protected IEnumerator spellCoroutine;

    //Hidden
    [HideInInspector]
    public int state
    {
        get; protected set;
    }
    [HideInInspector]
    public bool isSpellRuning
    {
        get; protected set;
    }

    protected float clipLength;

    #endregion

    #region Unity Functions

    protected void Awake()
    {
        if (!Mathf.Approximately(anticipationPercentage + actionPercentage + recuperationPercentage, 1))
            Debug.LogWarning("The sum of percentages is not equal to 1");

        SetTempProperties();

        Setup();
    }

    protected void OnDisable()
    {
        InitialEffectsSettings();
        InitialMaterialSettings();
    }

    #endregion

    #region Class Functions

    protected void Setup()
    {
        SetupLengths();
        SetupParticles();
        SetupLighting();
        SetupDecal();
        SetupEffects();
        SetupCamera();
        SetupMaterials();
        SetupAudio();
        SetupOthers();
    }

    protected virtual void SetupLengths()
    {
        //States Length
        anticipationLength = (spellSpeed * anticipationPercentage);
        actionLength = (spellSpeed * actionPercentage);
        recuperationLength = (spellSpeed * recuperationPercentage);

        //Lengths Until
        lengthUntilAction = anticipationLength + actionLength;
        lengthUntilHalfOfAction = anticipationLength + (actionLength / 2);
        lengthUntilRecuperation = lengthUntilAction + recuperationLength;
    }

    protected virtual void SetupParticles()
    {
        //SetupParticles
    }

    protected virtual void SetupLighting()
    {
        //SetupLights
    }

    protected virtual void SetupDecal()
    {
        //SetupDecals
    }

    protected virtual void SetupEffects()
    {
        //SetupImageEffects
    }

    protected virtual void SetupCamera()
    {
        //SetupCamera
    }

    protected virtual void SetupMaterials()
    {
        //SetupMaterials
    }

    protected virtual void SetupAudio()
    {
        //SetupAudio
    }

    protected virtual void SetupOthers()
    {
        //SetupOthers
    }

    protected virtual void InitialEffectsSettings()
    {
        //EffectsSettings
    }

    protected virtual void InitialMaterialSettings()
    {
        //MaterialSettings
    }

    protected virtual void SetTempProperties()
    {
        //SetTempProperties
    }

    protected virtual void ResetTempProperties()
    {
        //GetTempProperties
    }

    #endregion

    #region Spell Functions

    public void SetGlobalLength(float spellSpeed)
    {
        if (isSpellRuning)
            return;

        ResetTempProperties();

        this.spellSpeed = spellSpeed;

        Setup();
    }

    public void ExecuteSpell()
    {
        animator.SetTrigger(animationTriggerName);

        //Coroutine Setup
        if (spellCoroutine != null)
            StopCoroutine(spellCoroutine);

        spellCoroutine = SpellCoroutine();

        StartCoroutine(spellCoroutine);
    }

    protected virtual void SetupSpell()
    {
        isSpellRuning = true;

        //Animator
        animator.speed = clipLength / spellSpeed;
        state = 1;
    }

    protected virtual void SpellState1()
    {
        //Debug.Log("STATE 1");

        stateTime = anticipationTime / anticipationLength;
        stateTimeComplement = 1 - stateTime;
    }

    protected virtual void SpellState2()
    {
        //Debug.Log("STATE 2");

        stateTime = actionTime / actionLength;
        stateTimeComplement = 1 - stateTime;
    }

    protected virtual void SpellState3()
    {
        //Debug.Log("STATE 3");

        stateTime = recuperationTime / recuperationLength;
        stateTimeComplement = 1 - stateTime;
    }

    protected virtual void EndSpell()
    {
        isSpellRuning = false;

        //Animator
        animator.speed = 1f;
        state = 0;

        //Timing
        globalTime = 0f;
        anticipationTime = 0f;
        actionTime = 0f;
        recuperationTime = 0f;
        stateTime = 0f;
        stateTimeComplement = 1f;
    }

    protected virtual void IncreaseTime()
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
    }

    protected bool EvaluateState(float stateTime, float stateLength)
    {
        if (stateTime < stateLength)
            return true;
        else
            return false;
    }

    #endregion

    #region Spell Coroutines

    protected IEnumerator SpellCoroutine()
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

        StopCoroutine(spellCoroutine);
    }

    #endregion
}
