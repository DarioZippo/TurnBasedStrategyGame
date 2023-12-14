using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;
    
    private MeshRenderer meshRenderer;

    private void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; 

        UpdateVisual();   
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty){
        UpdateVisual();
    }

    private void UpdateVisual(){
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if(unit != null){
            if(selectedUnit == unit){
                meshRenderer.enabled = true;
            }
            else{
                meshRenderer.enabled = false;
            }
        }
    }

    private void OnDestroy() {
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedUnitChanged;    
    }
}
