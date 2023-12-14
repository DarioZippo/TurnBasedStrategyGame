using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 3;

    public static event EventHandler OnAnyAtionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update(){
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition){
            //Unit changed grid position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction{
        foreach (BaseAction baseAction in baseActionArray){
            if(baseAction is T){
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition(){
        return gridPosition;
    }

    public Vector3 GetWorldPosition(){
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray(){
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if(CanSpendActionPointsToTakeAction(baseAction)){
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction){
        if(actionPoints >= baseAction.GetActionPointCost()){
            return true;
        }
        else{
            return false;
        }
    }

    private void SpendActionPoints(int amount){
        actionPoints -= amount;

        OnAnyAtionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints(){
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e){
        if(IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
        (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())){
            actionPoints = ACTION_POINTS_MAX;

            OnAnyAtionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy(){
        return isEnemy;
    }

    public void Damage(int damageAmount){
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized(){
        return healthSystem.GetHealthNormalized();
    }
}
