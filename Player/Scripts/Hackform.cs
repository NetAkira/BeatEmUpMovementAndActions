using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hackform : MonoBehaviour
{

    [SerializeField] public enum hackForm {normalForm , MibmanForm, CthulhuForm, TetsuoForm}
    [SerializeField] private hackForm currentHackForm;
    private string currentHackformSkillName;

    [Header("Meshes")]
    [SerializeField] private GameObject [] normalFormMeshes;
    [SerializeField] private GameObject [] mibmanFormMeshes;
    [SerializeField] private GameObject [] cthulhuFormMeshes;
    [SerializeField] private GameObject [] tetsuoFormMeshes;

    [SerializeField] private float skillCharge;

    [Header("HUD")]
    [SerializeField] private TMPro.TextMeshProUGUI text_skillName;
    [SerializeField] private TMPro.TextMeshProUGUI text_skillAmount;


    //On Hackform Hit land
    public void hittedHackform(hackForm hackformType)
    {
        changeHackForm(hackformType);
    }

    // 
    private void changeHackForm(hackForm newForm)
    {
        currentHackForm = newForm;

        meshesDesactivation();
        meshesActivation(newForm);

        switch(currentHackForm)
        {
            case hackForm.normalForm:
                currentHackformSkillName = " - / - ";
                break;
            case hackForm.MibmanForm:
                currentHackformSkillName = "Kamehameha";
                break;
            case hackForm.CthulhuForm:
                currentHackformSkillName = "Hollow Purple";
                break;
            case hackForm.TetsuoForm:
                currentHackformSkillName = "Force Pull";
                break;
        }

        gameHud();
    }


    private void meshesDesactivation()
    {
        for(int i = 0; i < normalFormMeshes.Length; i++)
        {
            normalFormMeshes[i].SetActive(false);
        }
        for (int i = 0; i < mibmanFormMeshes.Length; i++)
        {
            mibmanFormMeshes[i].SetActive(false);
        }
        for (int i = 0; i < cthulhuFormMeshes.Length; i++)
        {
            cthulhuFormMeshes[i].SetActive(false);
        }
        for (int i = 0; i < tetsuoFormMeshes.Length; i++)
        {
            tetsuoFormMeshes[i].SetActive(false);
        }
    }

    //Updates the HUD every skill charge change && out of charges
    private void meshesActivation(hackForm newHackformMeshes)
    {
        switch(newHackformMeshes)
        {
            case hackForm.normalForm:
                for(int i =0; i < normalFormMeshes.Length; i ++)
                {
                    normalFormMeshes[i].SetActive(true);
                }
                break;

            case hackForm.MibmanForm:
                for (int i = 0; i < mibmanFormMeshes.Length; i++)
                {
                    mibmanFormMeshes[i].SetActive(true);
                }
                break;

            case hackForm.CthulhuForm:
                for (int i = 0; i < cthulhuFormMeshes.Length; i++)
                {
                    cthulhuFormMeshes[i].SetActive(true);
                }
                break;

            case hackForm.TetsuoForm:
                for (int i = 0; i < tetsuoFormMeshes.Length; i++)
                {
                    tetsuoFormMeshes[i].SetActive(true);
                }
                break;


        }


    }

    private void outOfCharges()
    {
        meshesDesactivation();
        meshesActivation(hackForm.normalForm);
        changeHackForm(hackForm.normalForm);

        //Updates hackformHUD
        gameHud();
    }

    // ---------/---------/---------/---------/ In game HUD

    private void gameHud()
    {
        text_skillName.text = currentHackformSkillName;
        text_skillAmount.text = "" + skillCharge;

    }

    // ----------// ----------// ----------// Setters

    public void setSkillCharge(float amount)
    {
        skillCharge += amount;

        if (skillCharge > 2) skillCharge = 2;
        if (skillCharge <= 0) outOfCharges();

        //Updates GameHUD
        gameHud();
    }


    // ----------// ----------// ----------// Getters

    public hackForm getCurrentHackform()
    {
        return this.currentHackForm;
    }
    public float getSkillCharge()
    {
        return this.skillCharge;
    }
}
