using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
functionalities:
- Heat
- SHield
- HP
- Weapon Overheat

(in progress)
- Grenade reload (& limited ammo??)
- Select shell type (stun)
- Show weapon overheat, next-shell reload time, and shell switch time holistically
- Limited (special-type) shells!
*/

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text heatText;
    [SerializeField] private TMP_Text shieldText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private Image HeatBar;
    [SerializeField] private Image ShieldBar;
    [SerializeField] private Image HpBar;
    [SerializeField] private Image WeaponBar;
    [SerializeField] private Image WeaponBar2;

    [SerializeField] private LauncherController myLauncherController;
    [SerializeField] private Image GrenadeBar;
    [SerializeField] private TMP_Text grenadeText;
    [SerializeField] private Image RocketBar;
    [SerializeField] private TMP_Text rocketText;

    [SerializeField] private Color safeColor;
    [SerializeField] private Color dangerColor;
    [SerializeField] private Color criticalColor;

    [SerializeField] private Image HitMarker;
    private float hitOpacityPercent = 0f;
    [SerializeField] private Image DestroyedMarker;
    private float destroyedOpacityPercent = 0f;

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
        updateHeat();
        updateHp();
        updateWeapon();
        updateWeapon2();
        updateShield();
        updateGrenade();
    }

    private void updateHeat() {
        float heat = mmcon.getHeatPercent();
        if(heatText) {
            heatText.text = $"HEAT\n{heat}%";
            // heatText.text = "HEAT - " + heat + "%";
            if(mmcon.getIsHeatVenting()) {
                // heatText.text += "\nHEAT VENTING";
            }
            if(heat>80f) {
                // heatText.text += "\nDANGER: VENT";
                heatText.color = criticalColor;
            }
            else if(heat > 60f) {
                heatText.color = dangerColor;
            }
            else {
                heatText.color = safeColor;
            }
        }
        if(HeatBar) {
            HeatBar.fillAmount = heat*0.01f;
        }
    }

    private void updateHp() {
        float hp = mmcon.getHealthPercent();
        if(damageText) {
            damageText.text = "DMG - " + hp + "%";
            if(hp<20f) {
                // damageText.text += "\nHEAVY DAMAGE";
                damageText.color = criticalColor;
            }
            else if(hp<40f) {
                damageText.color = dangerColor;
            }
            else {
                damageText.color = safeColor;
            }
        }
        if(HpBar) {
            HpBar.fillAmount = hp*0.01f;
        }
    }

    private void updateWeapon() {
        float wep = mmcon.getWeaponPercent();
        if(weaponText) {
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
        }
        if(WeaponBar) {
            WeaponBar.fillAmount = wep*0.01f;
        }
    }

    public void updateShield() {
        float shield = mmcon.getShieldPercent();
        if(shieldText) {
            shieldText.text = "SHLD - " + shield + "%";
            if(mmcon.getIsShielding()) {
                // shieldText.text += "\nSHIELDS UP";
            }
            if(shield<10f) {
                // shieldText.text += "\nSHIELDS LOW";
                shieldText.color = criticalColor;
            }
            else if(shield<30f) {
                shieldText.color = dangerColor;
            }
            else {
                shieldText.color = safeColor;
            }
        }
        if(ShieldBar) {
            ShieldBar.fillAmount = shield*0.01f;
        }
    }

    private void updateWeapon2() {
        if(WeaponBar2) {
            WeaponBar2.fillAmount = mmcon.getCooldownPercent();;
        }
    }

    private void updateGrenade() {
        GrenadeBar.fillAmount = myLauncherController.getCurrPercent();
        grenadeText.text = myLauncherController.getText();
    }

    public void showHitMarker() {
        print("hit marker!");
        // show htimarker
        // play hit sound
    }

    public void showDestroyedMarker() {
        print("destroy marker!");
        // show destroyed marker
    }
}
