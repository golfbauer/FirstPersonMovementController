using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : PlayerFeatureExecuteOverTime
{
    public float JetpackFuel { get; set; }
    public float TimeToDepletJetpackFuel { get; set; }
    public float TimeToRechargeJetpackFuel { get; set; }
    public float TimeToStartRecharge { get; set; }
    public float FallReductionFactor { get; set; } = 1;

    protected static float JetPackCapacity = 1f;

    protected override void Start()
    {
        base.Start();
        JetpackFuel = JetPackCapacity;
    }

    protected virtual bool CheckFuel()
    {
        return JetpackFuel > 0;
    }

    protected override bool CheckKeys()
    {
        return CheckAllInputGetKeys();
    }

    protected override void ExecuteAction()
    {
        if (!CheckDuringExecution())
        {
            FinishExecution();
            return;
        }
        manager.ProjectOnPlane = false;

        float velY = 0;
        if(manager.GetVelocity().y < 0)
        {
            velY = manager.GetVelocity().y * FallReductionFactor;
        }
        
        manager.AddVelocity(Vector3.up * (-velY + MoveSpeed), MoveCap);
        JetpackFuel -= Time.deltaTime / TimeToDepletJetpackFuel;

    }

    protected override void FinishExecution()
    {
        IsExecutingAction = false;
        EnableFeatures();
    }

    protected virtual bool CheckDuringExecution()
    {
        if (!CheckKeys()) return false;
        if (!CheckFuel()) return false;
        return true;
    }

    protected override void UpdateElapsedSince()
    {
        base.UpdateElapsedSince();

        if (TimeToStartRecharge < elapsedSinceLastExecution && JetpackFuel < JetPackCapacity)
        {
            JetpackFuel += Time.deltaTime / TimeToRechargeJetpackFuel;
            return;
        }
    }
}
