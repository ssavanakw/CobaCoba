using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public Animator myAnim;
    public bool isAttacking = false;
    public static CombatManager instance;

    private PlayerMovement playerMove;

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

    // Pake animation event buat stop move nya
    void StopMovement()
    {
        playerMove.enabled = false; // Disable player movement during attack
    }

    // Pake animation event buat start move nya
    void MoveAgain()
    {
        playerMove.enabled = true; // Disable player movement during attack
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            isAttacking = true;
        }
    }

}
