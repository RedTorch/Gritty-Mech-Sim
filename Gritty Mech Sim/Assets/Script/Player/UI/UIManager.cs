using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text heatText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text shieldText;

    [SerializeField] private Color safeColor;
    [SerializeField] private Color dangerColor;
    [SerializeField] private Color criticalColor;

    [SerializeField] private MechMovementController mmcon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!mmcon) {
            return;
        }

        float heat = mmcon.getHeatPercent();
        heatText.text = "HEAT - " + heat + "%";
        if(mmcon.getIsHeatVenting()) {
            heatText.text += "\nHEAT VENTING";
        }
        if(heat>80f) {
            heatText.text += "\nDANGER: VENT";
            heatText.color = criticalColor;
        }
        else if(heat > 60f) {
            heatText.color = dangerColor;
        }
        else {
            heatText.color = safeColor;
        }

        float hp = mmcon.getHealthPercent();
        damageText.text = "DMG - " + hp + "%";
        if(hp<20f) {
            damageText.text += "\nHEAVY DAMAGE";
            damageText.color = criticalColor;
        }
        else if(hp<40f) {
            damageText.color = dangerColor;
        }
        else {
            damageText.color = safeColor;
        }

        float wep = mmcon.getWeaponPercent();
        weaponText.text = "WEP - " + wep + "%";
        if(wep>90f) {
            weaponText.color = criticalColor;
            weaponText.text += "\nOVERHEAT";
        }
        else if(wep>70f) {
            weaponText.color = dangerColor;
            weaponText.text += "\nOVERHEAT";
        }
        else {
            weaponText.color = safeColor;
        }
        
        float shield = mmcon.getShieldPercent();
        shieldText.text = "SHLD - " + shield + "%";
        if(mmcon.getIsShielding()) {
            shieldText.text += "\nSHIELDS UP";
        }
        if(shield<10f) {
            shieldText.text += "\nSHIELDS LOW";
            shieldText.color = criticalColor;
        }
        else if(shield<30f) {
            shieldText.color = dangerColor;
        }
        else {
            shieldText.color = safeColor;
        }
    }

    public void showHitMarker() {
        // show htimarker
        // play hit sound
    }

    public void showDestroyedMarker() {
        // show destroyed marker
    }
}
