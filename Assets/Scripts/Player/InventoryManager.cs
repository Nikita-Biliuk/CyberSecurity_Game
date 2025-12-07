using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemsUISlots = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();    // List of weapon upgrade options
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); // List of passive item upgrade options
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();    // List of upgrade UI options

    PlayerStats player; // Reference to the PlayerStats script
    void Start()
    {
        player = GetComponent<PlayerStats>(); // Get the PlayerStats component attached to the player
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;  // Set the initial level to 1 when adding a new weapon
        weaponUISlots[slotIndex].enabled = true; 
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon; // Set the icon of the weapon in the UI

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level; // Set the initial level to 1 when adding a new passive item
        passiveItemsUISlots[slotIndex].enabled = true;
        passiveItemsUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon; // Set the icon of the passive item in the UI

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if(weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if(!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("No next level for "+ weapon.name); // Check if the weapon has a next level prefab
                return;
            }
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); // Set the new weapon as a child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>()); // Add the new weapon to the inventory
            Destroy(weapon.gameObject); // Destroy the old weapon
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; // Increment the level of the weapon
            Debug.Log("Upgraded weapon to level: " + upgradedWeapon.GetComponent<WeaponController>().weaponData.Level);

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData; // Update the weapon upgrade options with the new weapon data

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
            }

        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if(passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if(!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("No next level for "+ passiveItem.name); // Check if the passive item has a next level prefab
                return;
            }
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); // Set the new passive item as a child of the player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>()); // Add the new passive item to the inventory
            Destroy(passiveItem.gameObject); // Destroy the old passive item
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level; // Increment the level of the passive item
            Debug.Log("Upgraded passive item to level: " + upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level);

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData; // Update the passive item upgrade options with the new passive item data

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions); // Create a copy of the weapon upgrade options
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions); // Create a copy of the passive item upgrade options

        foreach(var upgradeOption in upgradeUIOptions)
        {
            if(availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return; // Break the loop if there are no available upgrades
            }

            int upgradeType;

            if(availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2; // Only passive item upgrades are available
            }
            else if(availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1; // Only weapon upgrades are available
            }
            else
            {
                upgradeType = Random.Range(1, 3); // Randomly choose between weapon and passive item upgrades
            }

            if(upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];

                availableWeaponUpgrades.Remove(chosenWeaponUpgrade); // Remove the chosen weapon upgrade from the list to avoid duplicates

                if(chosenWeaponUpgrade != null)
                {

                    EnableUpgradeUI(upgradeOption); // Enable the upgrade UI for the chosen weapon upgrade
                
                    bool newWeapon = false;
                    for(int i = 0; i < weaponSlots.Count; i++)
                    {
                        if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData) // Check if the weapon slot is not empty and matches the chosen weapon upgrade
                        {
                            newWeapon = false;
                            if(!newWeapon)
                            {

                                if(!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); // Disable the upgrade UI if the weapon has no next level prefab
                                    break; // Break the loop if the weapon has no next level prefab
                                }
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex)); // Assign the level up function to the button
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description; // Set the description of the weapon in the UI
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name; // Set the name of the weapon in the UI
                            }
                            break; // Break the loop if a weapon is found
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if(newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon)); // Assign the spawn function to the button
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description; // Set the description of the weapon in the UI
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name; // Set the name of the weapon in the UI
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon; // Set the icon of the weapon in the UI
                }
            }
            else if(upgradeType == 2)
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];

                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade); // Remove the chosen passive item upgrade from the list to avoid duplicates

                if(chosenPassiveItemUpgrade != null)
                {

                    EnableUpgradeUI(upgradeOption); // Enable the upgrade UI for the chosen passive item upgrade

                    bool newPassiveItem = false;
                    for(int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if(passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)

                        {
                            newPassiveItem = false;
                            if(!newPassiveItem)
                            {
                                if(!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); // Disable the upgrade UI if the passive item has no next level prefab
                                    break; // Break the loop if the passive item has no next level prefab
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex)); // Assign the level up function to the button

                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description; // Set the description of the passive item in the UI
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name; // Set the name of the passive item in the UI
                            }
                            break; // Break the loop if a passive item is found
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if(newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem)); // Assign the spawn function to the button

                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description; // Set the description of the passive item in the UI
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name; // Set the name of the passive item in the UI

                        upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon; // Set the icon of the passive item in the UI
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach(var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners(); // Remove all listeners from the button
            DisableUpgradeUI(upgradeOption); // Disable the upgrade UI
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions(); // Remove all upgrade options
        ApplyUpgradeOptions(); // Apply new upgrade options
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false); // Disable the upgrade UI
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true); // Enable the upgrade UI
    }
}
