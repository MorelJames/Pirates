using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewController : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;

    private Outline outline;

    private Activity activity = Activity.NoActivity;

    [SerializeField]
    private Color buttonColor, selectedButtonColor;
    [SerializeField]
    private Image RepairBtnBG, ReloadBtnBG, RowBtnBG;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void OnCrewSelected()
    {
        canvas.SetActive(true);
    }

    public void OnCrewUnselected()
    {
        canvas.SetActive(false);
    }

    public void OnRepareShip()
    {
        GameManager.Instance.RemoveCrew(activity);
        if (activity == Activity.Repair)
            activity = Activity.NoActivity;
        else
            activity = Activity.Repair;
            
        GameManager.Instance.AddCrew(activity);
        changeOutlineColor();
        ChangeSelectedButton();
        OnCrewUnselected();
    }

    public void OnReload()
    {
        GameManager.Instance.RemoveCrew(activity);
        if (activity == Activity.Reload)
            activity = Activity.NoActivity;
        else
            activity = Activity.Reload;
            GameManager.Instance.AddCrew(activity);
        changeOutlineColor();
        ChangeSelectedButton();
        OnCrewUnselected();
    }

    public void OnRow()
    {
        GameManager.Instance.RemoveCrew(activity);
        if (activity == Activity.Row)
            activity = Activity.NoActivity;
        else
            activity = Activity.Row;
            GameManager.Instance.AddCrew(activity);
        changeOutlineColor();
        ChangeSelectedButton();
        OnCrewUnselected();
    }

    private void changeOutlineColor()
    {
        switch (activity)
        {
            case Activity.Repair:
                outline.OutlineColor = Color.red;
                break;
            case Activity.NoActivity:
                outline.OutlineColor = Color.white;
                break;
            case Activity.Row:
                outline.OutlineColor = Color.blue;
                break;
            case Activity.Reload:
                outline.OutlineColor = Color.green;
                break;
        }
    }

    private void ChangeSelectedButton()
    {
        switch (activity)
        {
            case Activity.Repair:
                RepairBtnBG.color = selectedButtonColor;
                ReloadBtnBG.color = buttonColor;
                RowBtnBG.color = buttonColor;
                break;
            case Activity.NoActivity:
                RepairBtnBG.color = buttonColor;
                ReloadBtnBG.color = buttonColor;
                RowBtnBG.color = buttonColor;
                break;
            case Activity.Row:
                RepairBtnBG.color = buttonColor;
                ReloadBtnBG.color = buttonColor;
                RowBtnBG.color = selectedButtonColor;
                break;
            case Activity.Reload:
                RepairBtnBG.color = buttonColor;
                ReloadBtnBG.color = selectedButtonColor;
                RowBtnBG.color = buttonColor;
                break;
        }
    }
    public enum Activity { Repair,Row,Reload,NoActivity}
}
