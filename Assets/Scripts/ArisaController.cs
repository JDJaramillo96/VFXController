using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ArisaController : MonoBehaviour {

    #region Properties

    private Animator animator;

    [Header("Spells")] [Space(10f)]

    [SerializeField]
    private Spell01 spell1;
    [SerializeField]
    private Spell02 spell2;

    //Hidden
    private Slider slider;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        animator = GetComponent<Animator>();
        slider = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
    }

    private void Start()
    {
        spell1.SetGlobalLength(slider.value);
        spell2.SetGlobalLength(slider.value);
    }

    private void Update()
    {
        InputModule();
    }

    #endregion

    #region Class Functions

    private void InputModule()
    {
        slider.interactable = !IsSpellActive() ? true : false;

        //Spell inputs
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (!IsSpellActive())
            {
                spell1.ExecuteSpell();
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
            if (!IsSpellActive())
            {
                spell2.ExecuteSpell();
            }
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

    private bool IsSpellActive()
    {
        if (spell1.isSpellRuning)
            return true;
        else if (spell2.isSpellRuning)
            return true;
        else
            return false;
    }

    #endregion
}
