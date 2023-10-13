using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;



public class GUIManager : Singleton<GUIManager>
{
    public GameObject inventoryGUI;
    private InputManager _inputManager;

    public GameObject crosshairGUI;
    public GameObject collectionGUI;
    public Slider stroageSliderGUI;
    public TextMeshProUGUI storageCountTextMeshGUI;
    public bool InventoryIsOpen { get; private set; }
    public bool InventoryFull { get; set; }
    private void Awake()
    {
        _inputManager = new InputManager();
    }
    private void OnEnable()
    {
        _inputManager.Player.Enable();
        InitializeInputAction();
    }
    private void ShowInventoryGUI(InputAction.CallbackContext context)
    {
        InventoryIsOpen = !InventoryIsOpen;
        inventoryGUI.SetActive(InventoryIsOpen);
    }
    private void InitializeInputAction()
    {
        _inputManager.Player.Inventory.started += ShowInventoryGUI;
    }
    private void DeinitializeInputAction()
    {
        _inputManager.Player.Inventory.started -= ShowInventoryGUI;
    }
    private void OnDisable()
    {
        _inputManager.Player.Disable();
        DeinitializeInputAction();
    }
}
