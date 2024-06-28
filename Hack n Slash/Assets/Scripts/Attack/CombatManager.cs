using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public Animator myAnim;
    public bool isAttacking = false;
    public static CombatManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            isAttacking = true;
        }
    }

}
