using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start(){
        Unit.OnAnyAtionPointsChanged += Unit_OnAnyAtionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void UpdateActionPointsText(){
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void Unit_OnAnyAtionPointsChanged(object sender, EventArgs e){
        UpdateActionPointsText();
    }

    private void UpdateHealthBar(){
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e){
        UpdateHealthBar();
    }
}
