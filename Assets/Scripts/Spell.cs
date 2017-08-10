using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour {

    #region Properties

    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected string animationTriggerName;

    [Space(10f)]
    [Header("States")]

    [SerializeField]
    [Range(0, 1)]
    protected float anticipationPercentage = 0.2f;
    [SerializeField]
    [Range(0, 1)]
    protected float actionPercentage = 0.25f;
    [SerializeField]
    [Range(0, 1)]
    protected float recuperationPercentage = 0.55f;

    [Space(10f)]
    [Header("Others")]

    [SerializeField]
    private float globalLength = 2.25f;

    //Times
    protected float globalTime = 0f;
    protected float anticipationTime = 0f;
    protected float actionTime = 0f;
    protected float recuperationTime = 0f;
    protected float stateTime = 0f;

    //Times Lengths
    protected float anticipationLength;
    protected float actionLength;
    protected float recuperationLength;
    protected float lengthUntilAction;
    protected float lengthUntilHalfOfAction;
    protected float lengthUntilRecuperation;

    //Cached Components
    protected float initialSpeed;
    protected float scaledSpeed;
    protected float scaledAcceleration;
    protected float newScale;
    protected IEnumerator spellCoroutine;

    //Hidden
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public bool isSpellRuning = false;

    #endregion

    #region Unity Functions

    protected void Awake()
    {
        if (anticipationPercentage + actionPercentage + recuperationPercentage != 1)
            Debug.LogWarning("The sum of percentages is not equal to 1");

        Setup();
    }

    #endregion

    #region Class Functions

    protected void Setup()
    {
        SetupLengths();
        SetupParticles();
        SetupLights();
        SetupDecals();
        SetupImageEffects();
        SetupAudio();
        SetupOthers();
    }

    protected virtual void SetupLengths()
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

    protected virtual void SetupParticles()
    {
        //SetupParticles
    }

    protected virtual void SetupLights()
    {
        //SetupLights
    }

    protected virtual void SetupDecals()
    {
        //SetupDecals
    }

    protected virtual void SetupImageEffects()
    {
        //SetupImageEffects
    }

    protected virtual void SetupAudio()
    {
        //SetupAudio
    }

    protected virtual void SetupOthers()
    {
        //SetupOthers
    }

    #endregion

    #region Spell Functions

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
        animator.SetTrigger("spell1");

        //Coroutine Setup
        if (spellCoroutine != null)
            StopCoroutine(spellCoroutine);

        spellCoroutine = SpellCoroutine();

        StartCoroutine(spellCoroutine);
    }

    protected virtual void SpellState1()
    {
        Debug.Log("STATE 1");

        stateTime = anticipationTime / anticipationLength;
    }

    protected virtual void SpellState2()
    {
        Debug.Log("STATE 2");

        stateTime = actionTime / actionLength;
    }

    protected virtual void SpellState3()
    {
        Debug.Log("STATE 3");

        stateTime = recuperationTime / recuperationLength;
    }

    protected virtual void EndSpell()
    {
        isSpellRuning = false;

        animator.speed = 1f;
        state = 0;

        //Timing
        globalTime = 0f;
        anticipationTime = 0f;
        actionTime = 0f;
        recuperationTime = 0f;
        stateTime = 0f;
    }

    protected void IncreaseTime()
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
    }

    #endregion
}
