using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake(){
        if(Instance != null){
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
 
    //Col cambio di turno si seleziona automaticamente la prima unità disponibile per non incorre in errore
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            if (selectedUnit == null)
            {
                if (UnitManager.Instance.GetFriendlyUnitList().Count > 0)
                {
                    SetSelectedUnit(UnitManager.Instance.GetFriendlyUnitList()[0]);
                } else
                {
                    SetSelectedUnit(null);
                }
            }
        }
    }

    private void Update(){
        if(!isBusy){
            if(!TurnSystem.Instance.IsPlayerTurn()){
                return;
            }

            //Per verificare che il mouse non sia sopra un elemento della UI
            if(EventSystem.current.IsPointerOverGameObject()){
                return;
            }

            if(TryHandleUnitSelection()){
                return;
            }
            
            HandleSelectedAction();
        }
    }

    private void HandleSelectedAction(){
        if (InputManager.Instance.IsMouseButtonDownThisFrame()){
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if(!selectedAction.IsValidActionGridPosition(mouseGridPosition)){
                return;
            }

            if(!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)){
                return;
            }
                
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy(){
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy(){
        isBusy = false;
        
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection(){
        if (InputManager.Instance.IsMouseButtonDownThisFrame()){
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)){
                //Usiamo TryGetComponent per evitarci un null check dedicato
                if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit)){
                    if(unit == selectedUnit){
                        //Per evitare la riselezione ed il reset delle azioni
                        return false;
                    }

                    if(unit.IsEnemy()){
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit){
        selectedUnit = unit;

        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction){
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit(){
        return selectedUnit;
    }

    public BaseAction GetSelectedAction(){
        return selectedAction;
    }
}
