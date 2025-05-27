using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private VisualElement upgradeWindow;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        upgradeWindow = root.Q<VisualElement>("UpgradeWindow");
        root.Q<Button>("UpgradeBtn").clicked += ToggleUpgradeWindow;
        root.Q<Button>("UpgradeSpeed").clicked += UpgradeSpeed;
        root.Q<Button>("UpgradeDamage").clicked += UpgradeAttack;
        root.Q<Button>("UpgradeHealth").clicked += UpgradeHealth;
    }

    private void UpgradeHealth()
    {
        GameManager.Instance.UpgradeHealth();
    }

    private void UpgradeAttack()
    {
        GameManager.Instance.UpgradeAttack();
    }

    private void UpgradeSpeed()
    {
        GameManager.Instance.UpgradeSpeed();
    }

    private void OnDisable()
    {
        root.Q<Button>("UpgradeBtn").clicked -= ToggleUpgradeWindow;
        root.Q<Button>("UpgradeSpeed").clicked -= GameManager.Instance.UpgradeSpeed;
        root.Q<Button>("UpgradeDamage").clicked -= GameManager.Instance.UpgradeAttack;
        root.Q<Button>("UpgradeHealth").clicked -= GameManager.Instance.UpgradeHealth;
    }

    private void ToggleUpgradeWindow()
    {
        upgradeWindow.style.visibility = upgradeWindow.style.visibility== Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
    }
}
