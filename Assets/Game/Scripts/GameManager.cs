using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ShipController shipController;
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask clickableLayers;

    public EventSystem eventSystem;

    private CrewController activeCrew = null;
    public GraphicRaycaster activeGraphicRaycaster = null;


    private static GameManager instance = null;
    public static GameManager Instance => instance;

    private bool hoveringGameUI;

    public int coin {  get; private set; }

    private Label coinLabel, attackPrice, speedPrice, healthPrice;

    private int healtUpgradePrice = 10, speedUpgradePrice =10,attackUpgradePrice = 10;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        var root = GameObject.FindGameObjectWithTag("UI").GetComponent<UIDocument>().rootVisualElement;
        coinLabel = root.Q<Label>("Coin");
        coinLabel.text = coin.ToString();
        attackPrice = root.Q<Label>("DamagePrice");
        speedPrice = root.Q<Label>("SpeedPrice");
        healthPrice = root.Q<Label>("HealthPrice");
        attackPrice.text = attackUpgradePrice.ToString();
        speedPrice.text = speedUpgradePrice.ToString();
        healthPrice.text = healtUpgradePrice.ToString();
        var targetClassName = "hoverable";
        var hoverElements = root.Query<VisualElement>(className: targetClassName).ToList();

        foreach (var element in hoverElements)
        {
            element.RegisterCallback<MouseEnterEvent>(evt => 
            { 
                hoveringGameUI = true;
            });

            element.RegisterCallback<MouseLeaveEvent>(evt => 
            { 
                hoveringGameUI= false;
            });
        }
        coin = 0;
    }
    private void OnLeftClick()
    {
        if (activeCrew != null)
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            activeGraphicRaycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                return;
            }
        }

        RaycastHit hit;
        if (!hoveringGameUI && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            UnSelectCrew();
            switch (hit.collider.gameObject.layer)
            {
                case 3:
                    shipController.MoveShip(hit.point);
                    break;
                case 6:
                    SelectCrew(hit.collider.gameObject.GetComponent<CrewController>());
                    break;
                case 10:
                    shipController.Attack(hit.transform);
                    break;
            }
            if (hit.collider.gameObject.layer == 3)
                shipController.MoveShip(hit.point);
            if (hit.collider.gameObject.layer == 6)
                SelectCrew(hit.collider.gameObject.GetComponent<CrewController>());
            Debug.Log(hit.point);
        }
    }

    #region Crew management
    private void SelectCrew(CrewController crew)
    {
        if ( activeCrew == crew)
            return;
        crew.OnCrewSelected();
        activeCrew = crew;
        activeGraphicRaycaster = crew.gameObject.GetComponentInChildren<GraphicRaycaster>();
    }

    private void UnSelectCrew()
    {
        if (activeCrew == null) return;
        activeCrew.OnCrewUnselected();
        activeCrew = null;
        activeGraphicRaycaster = null;
    }

    public void AddCrew(CrewController.Activity activity)
    {
        if (activity != CrewController.Activity.NoActivity)
            shipController.AddCrew(activity);
    }

    public void RemoveCrew(CrewController.Activity activity)
    {
        if(activity != CrewController.Activity.NoActivity) 
            shipController.RemoveCrew(activity);
    }
    #endregion
    public void GetCoin(int value)
    {
        coin += value;
        coinLabel.text = coin.ToString();
    }

    public void RemoveCoin(int value)
    {
        coin -= value;
        coinLabel.text = coin.ToString();
    }

    public void UpgradeHealth()
    {
        if (!(coin >= healtUpgradePrice))
            return;
        Debug.Log("Upgrade Health");
        shipController.UpgradeHealth();
        RemoveCoin(healtUpgradePrice);
        healtUpgradePrice = Mathf.RoundToInt(healtUpgradePrice * 1.5f);
        healthPrice.text = healtUpgradePrice.ToString();
    }

    public void UpgradeSpeed()
    {
        if (!(coin >= speedUpgradePrice))
            return;
        Debug.Log("Upgrade Speed");
        shipController.UpgradeSpeed();
        RemoveCoin(speedUpgradePrice);
        speedUpgradePrice = Mathf.RoundToInt(speedUpgradePrice * 1.5f);
        speedPrice.text = speedUpgradePrice.ToString();
    }

    public void UpgradeAttack()
    {
        if (!(coin >= attackUpgradePrice))
            return;
        Debug.Log("Upgrade Attack");
        shipController.UpgradeAttack();
        RemoveCoin(attackUpgradePrice);
        attackUpgradePrice = Mathf.RoundToInt(attackUpgradePrice * 1.5f);
        attackPrice.text = attackUpgradePrice.ToString();
    }
}
