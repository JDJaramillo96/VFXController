﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArisaController : MonoBehaviour {

    #region Properties

    private Animator animator;
    [SerializeField]
    private VFXController vfx;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        InputModule();
    }

    #endregion

    #region Class Functions

    private void InputModule()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (!vfx.isSpellRuning)
            {
                vfx.ExecuteSpell();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            animator.SetTrigger("spell2");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            animator.SetTrigger("spell3");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            animator.SetTrigger("spell4");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            animator.SetTrigger("spell5");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            animator.SetTrigger("spell6");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            animator.SetTrigger("spell7");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            animator.SetTrigger("spell8");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            animator.SetTrigger("spell9");
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            animator.SetTrigger("spell10");
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            animator.SetTrigger("spell11");
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            animator.SetTrigger("spell12");
        }
    }

    #endregion
}