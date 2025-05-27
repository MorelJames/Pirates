using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour, IDamageable
{
    private NavMeshAgent agent;
    private int currentHealth;
    private Collider collider;
    private bool readyToFire;

    [Header("Repair")]
    [SerializeField] 
    private int repairRatePerCrew;
    [SerializeField]
    private float repairInterval;
    [SerializeField]
    private int maxHealth;
    private int numberOfCrewRepairing;

    [Header("Reload")]
    [SerializeField]
    private float ReloadSpeedFactorPerCrew;
    [SerializeField]
    private float baseReloadSpeed;
    [SerializeField]
    private float reloadInterval;
    [SerializeField]
    private int maxReloading;
    private int numberOfCrewReloading;
    private float currentReloadingState;

    [Header("Row")]
    [SerializeField]
    private float ShipSpeedFactorPerCrew;
    [SerializeField]
    private float baseShipSpeed;
    private int numberOfCrewRowing;
    
    private Coroutine repairCoroutine;
    private Coroutine reloadCoroutine;

    private ProgressBar lifeBar;
    private ProgressBar reloadbar;

    [Header("Canon")]
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Transform canonBallSpawnPos;
    private float damageMultiplier = 1;
    private void OnEnable()
    {
        var rootVisualElement = GameObject.FindGameObjectWithTag("UI").GetComponent<UIDocument>().rootVisualElement;
        lifeBar = rootVisualElement.Q<ProgressBar>("LifeBar"); 
        reloadbar = rootVisualElement.Q<ProgressBar>("ReloadBar");
        lifeBar.highValue = maxHealth;
        lifeBar.value = currentHealth;
        reloadbar.value = maxReloading;
        reloadbar.value = currentReloadingState;
        collider = GetComponent<Collider>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseShipSpeed;
        //Todo : mettre max health
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void MoveShip(Vector3 pos) 
    { 
        agent.destination = pos;
    }

    #region Crew Management
    public void AddCrew(CrewController.Activity activity) {
        Debug.Log("Adding crew");
        switch (activity) {
            case CrewController.Activity.Repair:
                {
                    numberOfCrewRepairing++;
                    Debug.Log("Repair : "+numberOfCrewRepairing);
                    break;
                }
            case CrewController.Activity.Row:
                {
                    numberOfCrewRowing++;
                    agent.speed = baseShipSpeed + numberOfCrewRowing * ShipSpeedFactorPerCrew;
                    Debug.Log("Row : "+numberOfCrewRowing);
                    break;
                }
            case CrewController.Activity.Reload: 
                {
                    numberOfCrewReloading++;
                    Debug.Log("Reload : "+numberOfCrewReloading);
                    break;
                } 
        }
        CheckForRoutines();
    }

    public void RemoveCrew(CrewController.Activity activity)
    {
        switch (activity)
        {
            case CrewController.Activity.Repair:
                {
                    numberOfCrewRepairing = Mathf.Max(0, numberOfCrewRepairing - 1);
                    break;
                }
            case CrewController.Activity.Row:
                {
                    numberOfCrewRowing = Mathf.Max(0, numberOfCrewRowing - 1);
                    agent.speed = baseShipSpeed + numberOfCrewRowing * ShipSpeedFactorPerCrew;
                    break;
                }
            case CrewController.Activity.Reload:
                {
                    numberOfCrewReloading = Mathf.Max(0, numberOfCrewReloading - 1);
                    break;
                }
        }
        CheckForRoutines();
    }
    #endregion

    #region Coroutines
    private void CheckForRoutines()
    {
        if (numberOfCrewRepairing > 0 && repairCoroutine == null && currentHealth < maxHealth)
            StartRepair();
        if (numberOfCrewRepairing == 0) StopRepair();
        if (numberOfCrewReloading > 0 && repairCoroutine == null && !readyToFire)
            StartReload();
        if (numberOfCrewReloading == 0) StopReload();
    }

    private IEnumerator RepairShip()
    {
        while (currentHealth < maxHealth)
        {
            int repairAmount = numberOfCrewRepairing * repairRatePerCrew;
            currentHealth += repairAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            UpdateHealthUI();
            Debug.Log("Repairing : " + currentHealth);
            yield return new WaitForSeconds(repairInterval);
        }
        repairCoroutine = null;
    }

    private IEnumerator Reload() {
        while (currentReloadingState < maxReloading)
        {
            float reloadAmount = numberOfCrewReloading * ReloadSpeedFactorPerCrew;
            currentReloadingState += reloadAmount;
            currentReloadingState = Mathf.Min(currentReloadingState, maxReloading);
            UpdateReloadingUI();
            Debug.Log("Reloading : "+currentReloadingState);
            yield return new WaitForSeconds(reloadInterval);
        }
        readyToFire = true;
        reloadCoroutine = null;
    }

    public void StartRepair()
    {
        if (repairCoroutine == null)
            repairCoroutine = StartCoroutine(RepairShip());
    }

    public void StopRepair()
    {
        if (repairCoroutine != null)
        {
            StopCoroutine(repairCoroutine);
            repairCoroutine = null;
        }
    }

    private void StopReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }

    private void StartReload()
    {
        if (reloadCoroutine == null)
            reloadCoroutine = StartCoroutine(Reload());
    }
    #endregion

    public void Attack(Transform target) {
        if (readyToFire)
        {
            Rigidbody rb = Instantiate(projectile, canonBallSpawnPos.position, Quaternion.identity).GetComponent<Rigidbody>();
            Physics.IgnoreCollision(rb.gameObject.GetComponent<Collider>(), collider);
            rb.GetComponent<CanonBall>().SetDamageMultiplier(damageMultiplier);
            rb.AddForce((target.position - transform.position).normalized * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            readyToFire=false;
            currentReloadingState = 0;
            if(numberOfCrewReloading>0) StartReload();
        }
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        if (numberOfCrewRepairing > 0) StartRepair();
        UpdateHealthUI();
        Debug.Log("Player get damaged");
    }

    private void UpdateHealthUI()
    {
        lifeBar.value = currentHealth;
    }

    private void UpdateReloadingUI()
    {
        reloadbar.value = currentReloadingState;
        if (currentReloadingState == maxReloading)
            reloadbar.title = "Ready to shoot";
        else
            reloadbar.title = "Reload";
    }

    public void UpgradeHealth()
    {
        maxHealth = Mathf.RoundToInt(maxHealth*1.2f);
        lifeBar.highValue = maxHealth;
        StartRepair();
    }

    public void UpgradeAttack() 
    {
        damageMultiplier += .5f;
    }

    public void UpgradeSpeed()
    {
        baseShipSpeed *= 1.2f;
    }
    
}
