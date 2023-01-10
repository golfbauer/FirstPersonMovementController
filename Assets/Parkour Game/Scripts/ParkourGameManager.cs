using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParkourGameManager : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float lowestPoint;
    [SerializeField] private TMP_Text talker;

    private int deathCount = 0;
    private bool enabledJump;
    private string displayText;
    private float displayTime = 5f;
    private Coroutine displayMessage;
    private PlayerMovementManager movementManager;
    private Jumping jumpFeature;

    void Start()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        if(spawnPosition == Vector3.zero){
            spawnPosition = transform.position;
        }
        talker.text = "";
    }

    void Update()
    {
        CheckDeath();
        CheckDisplayMessage();

        if(!enabledJump){
            EnableJump();
            EnabledDoubleJump();
        }
    }


    void CheckDeath(){
        if(transform.position.y < lowestPoint){
            deathCount++;
            transform.position = spawnPosition;
            CheckCauseOfDeath();
        }
    }

    void EnableJump(){
        if(!jumpFeature){
            jumpFeature = GetComponent<Jumping>();
        }
        if(jumpFeature && deathCount > 0 && jumpFeature.Disabled == true){
            jumpFeature.Disabled = false;
        }
    }

    void EnabledDoubleJump()
    {
        if(jumpFeature && deathCount > 1 && jumpFeature.MaxJumpCount == 1){
            jumpFeature.MaxJumpCount = 2;
            enabledJump = true;
        }
    }

    public void CheckDisplayMessage(){
        if(displayText == "" || talker.text == displayText){
            return;
        }

        if(displayMessage != null){
            StopCoroutine(displayMessage);
        }
        displayMessage = StartCoroutine(DisplayMessage());
    }

    public IEnumerator DisplayMessage(){
        talker.text = displayText;
        
        yield return new WaitForSeconds(displayTime);

        talker.text = "";
        displayText = "";
    }

    void CheckCauseOfDeath(){
        if(!enabledJump){
            if(deathCount == 1) displayText = ParkourUtils.JumpDisabled;
            if(deathCount == 2) displayText = ParkourUtils.NoDoubleJump;
        }
    }
}
