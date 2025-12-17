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
        // Validate slot index
        if (slotIndex < 0 || slotIndex >= weaponSlots.Count)
        {
            Debug.LogError($"Invalid weapon slot index: {slotIndex}");
            return;
        }

        // Check if slot is already occupied (should not happen with proper logic)
        if (weaponSlots[slotIndex] != null)
        {
            Debug.LogWarning($"Weapon slot {slotIndex} is already occupied! Overwriting...");
        }

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
        // Validate slot index
        if (slotIndex < 0 || slotIndex >= passiveItemSlots.Count)
        {
            Debug.LogError($"Invalid passive item slot index: {slotIndex}");
            return;
        }

        // Check if slot is already occupied (should not happen with proper logic)
        if (passiveItemSlots[slotIndex] != null)
        {
            Debug.LogWarning($"Passive item slot {slotIndex} is already occupied! Overwriting...");
        }

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
        if(slotIndex < 0 || slotIndex >= weaponSlots.Count || weaponSlots[slotIndex] == null)
        {
            Debug.LogError($"Invalid weapon slot index: {slotIndex}");
            return;
        }

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
        
        Debug.Log($"Upgraded weapon to level: {weaponLevels[slotIndex]}");

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if(slotIndex < 0 || slotIndex >= passiveItemSlots.Count || passiveItemSlots[slotIndex] == null)
        {
            Debug.LogError($"Invalid passive item slot index: {slotIndex}");
            return;
        }

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
        
        Debug.Log($"Upgraded passive item to level: {passiveItemLevels[slotIndex]}");

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp(); // End the level up screen if the player is choosing an upgrade
        }
    }

    void ApplyUpgradeOptions()
    {
        // First clear all UI elements and listeners
        foreach(var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }

        // Check if weapon slots are full
        bool weaponSlotsFull = AreWeaponSlotsFull();
        bool passiveItemSlotsFull = ArePassiveItemSlotsFull();

        // Get all currently available upgrades
        List<WeaponUpgrade> availableUpgrades = GetAvailableWeaponUpgrades(weaponSlotsFull);
        List<PassiveItemUpgrade> availablePassiveUpgrades = GetAvailablePassiveItemUpgrades(passiveItemSlotsFull);

        // Combine all available upgrades
        List<object> allAvailableUpgrades = new List<object>();
        allAvailableUpgrades.AddRange(availableUpgrades);
        allAvailableUpgrades.AddRange(availablePassiveUpgrades);

        // Shuffle the list for random selection
        ShuffleList(allAvailableUpgrades);

        // Determine how many upgrades to show (max 4, or as many as available)
        int upgradesToShow = Mathf.Min(upgradeUIOptions.Count, allAvailableUpgrades.Count);
        
        // Setup UI for each upgrade
        for(int i = 0; i < upgradesToShow; i++)
        {
            var upgradeOption = upgradeUIOptions[i];
            
            if(allAvailableUpgrades[i] is WeaponUpgrade weaponUpgrade)
            {
                SetupWeaponUpgradeUI(upgradeOption, weaponUpgrade, weaponSlotsFull);
            }
            else if(allAvailableUpgrades[i] is PassiveItemUpgrade passiveUpgrade)
            {
                SetupPassiveItemUpgradeUI(upgradeOption, passiveUpgrade, passiveItemSlotsFull);
            }
        }

        // Hide any remaining UI slots
        for(int i = upgradesToShow; i < upgradeUIOptions.Count; i++)
        {
            DisableUpgradeUI(upgradeUIOptions[i]);
        }
    }

    // Check if all weapon slots are occupied
    bool AreWeaponSlotsFull()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] == null)
                return false; // Found an empty slot
        }
        return true; // All slots are occupied
    }

    // Check if all passive item slots are occupied
    bool ArePassiveItemSlotsFull()
    {
        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] == null)
                return false; // Found an empty slot
        }
        return true; // All slots are occupied
    }

    // Get all available weapon upgrades (excluding duplicates and weapons player already has)
    List<WeaponUpgrade> GetAvailableWeaponUpgrades(bool slotsFull)
    {
        List<WeaponUpgrade> available = new List<WeaponUpgrade>();
        
        foreach(var weaponUpgrade in weaponUpgradeOptions)
        {
            // Check if player has this weapon or any version of it
            bool playerHasThisWeapon = false;
            int slotIndex = -1;
            
            for(int i = 0; i < weaponSlots.Count; i++)
            {
                if(weaponSlots[i] != null)
                {
                    // Check if this is the same weapon (by checking the upgrade chain)
                    if(IsSameWeapon(weaponSlots[i].weaponData, weaponUpgrade.weaponData))
                    {
                        playerHasThisWeapon = true;
                        slotIndex = i;
                        break;
                    }
                }
            }
            
            if(playerHasThisWeapon)
            {
                // Player has this weapon - check if it can be upgraded
                if(weaponSlots[slotIndex].weaponData.NextLevelPrefab != null)
                {
                    // Only add if not already in the list
                    if(!ContainsWeaponUpgrade(available, weaponUpgrade))
                    {
                        available.Add(weaponUpgrade);
                    }
                }
            }
            else if(!slotsFull) // Only allow new weapons if there are empty slots
            {
                // Player doesn't have this weapon AND there are empty slots
                // Make sure no other level of this weapon is already available
                bool otherLevelAvailable = false;
                foreach(var existingUpgrade in available)
                {
                    if(IsSameWeapon(existingUpgrade.weaponData, weaponUpgrade.weaponData))
                    {
                        otherLevelAvailable = true;
                        break;
                    }
                }
                
                if(!otherLevelAvailable)
                {
                    available.Add(weaponUpgrade);
                }
            }
            // If slots are full and player doesn't have this weapon, skip it
        }
        
        return available;
    }

    // Get all available passive item upgrades (excluding duplicates and items player already has)
    List<PassiveItemUpgrade> GetAvailablePassiveItemUpgrades(bool slotsFull)
    {
        List<PassiveItemUpgrade> available = new List<PassiveItemUpgrade>();
        
        foreach(var passiveUpgrade in passiveItemUpgradeOptions)
        {
            // Check if player has this passive item or any version of it
            bool playerHasThisItem = false;
            int slotIndex = -1;
            
            for(int i = 0; i < passiveItemSlots.Count; i++)
            {
                if(passiveItemSlots[i] != null)
                {
                    // Check if this is the same item (by checking the upgrade chain)
                    if(IsSamePassiveItem(passiveItemSlots[i].passiveItemData, passiveUpgrade.passiveItemData))
                    {
                        playerHasThisItem = true;
                        slotIndex = i;
                        break;
                    }
                }
            }
            
            if(playerHasThisItem)
            {
                // Player has this item - check if it can be upgraded
                if(passiveItemSlots[slotIndex].passiveItemData.NextLevelPrefab != null)
                {
                    // Only add if not already in the list
                    if(!ContainsPassiveItemUpgrade(available, passiveUpgrade))
                    {
                        available.Add(passiveUpgrade);
                    }
                }
            }
            else if(!slotsFull) // Only allow new items if there are empty slots
            {
                // Player doesn't have this item AND there are empty slots
                // Make sure no other level of this item is already available
                bool otherLevelAvailable = false;
                foreach(var existingUpgrade in available)
                {
                    if(IsSamePassiveItem(existingUpgrade.passiveItemData, passiveUpgrade.passiveItemData))
                    {
                        otherLevelAvailable = true;
                        break;
                    }
                }
                
                if(!otherLevelAvailable)
                {
                    available.Add(passiveUpgrade);
                }
            }
            // If slots are full and player doesn't have this item, skip it
        }
        
        return available;
    }

    // Check if two weapons are the same (using the NextLevelPrefab chain)
    bool IsSameWeapon(WeaponScriptableObject weapon1, WeaponScriptableObject weapon2)
    {
        // If they're the same object
        if(weapon1 == weapon2) return true;
        
        // Check if weapon1 is in weapon2's upgrade chain
        WeaponScriptableObject current = weapon1;
        while(current != null)
        {
            if(current.NextLevelPrefab != null)
            {
                var nextLevelController = current.NextLevelPrefab.GetComponent<WeaponController>();
                if(nextLevelController != null && nextLevelController.weaponData == weapon2)
                    return true;
            }
            
            // Check if weapon1 is a previous level of weapon2
            // We need to check all weapon upgrades to find previous levels
            foreach(var upgrade in weaponUpgradeOptions)
            {
                if(upgrade.weaponData.NextLevelPrefab != null)
                {
                    var nextController = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>();
                    if(nextController != null && nextController.weaponData == current)
                    {
                        // This means upgrade.weaponData is a previous level of current
                        if(upgrade.weaponData == weapon2)
                            return true;
                    }
                }
            }
            
            // Try to find the next level in our upgrade options
            bool foundNext = false;
            foreach(var upgrade in weaponUpgradeOptions)
            {
                if(upgrade.weaponData == current && upgrade.weaponData.NextLevelPrefab != null)
                {
                    var nextController = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>();
                    if(nextController != null)
                    {
                        current = nextController.weaponData;
                        foundNext = true;
                        break;
                    }
                }
            }
            
            if(!foundNext) break;
        }
        
        // Check if weapon2 is in weapon1's upgrade chain
        current = weapon2;
        while(current != null)
        {
            if(current.NextLevelPrefab != null)
            {
                var nextLevelController = current.NextLevelPrefab.GetComponent<WeaponController>();
                if(nextLevelController != null && nextLevelController.weaponData == weapon1)
                    return true;
            }
            
            // Check if weapon2 is a previous level of weapon1
            foreach(var upgrade in weaponUpgradeOptions)
            {
                if(upgrade.weaponData.NextLevelPrefab != null)
                {
                    var nextController = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>();
                    if(nextController != null && nextController.weaponData == current)
                    {
                        if(upgrade.weaponData == weapon1)
                            return true;
                    }
                }
            }
            
            // Try to find the next level
            bool foundNext = false;
            foreach(var upgrade in weaponUpgradeOptions)
            {
                if(upgrade.weaponData == current && upgrade.weaponData.NextLevelPrefab != null)
                {
                    var nextController = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>();
                    if(nextController != null)
                    {
                        current = nextController.weaponData;
                        foundNext = true;
                        break;
                    }
                }
            }
            
            if(!foundNext) break;
        }
        
        return false;
    }

    // Check if two passive items are the same (using the NextLevelPrefab chain)
    bool IsSamePassiveItem(PassiveItemScriptableObject item1, PassiveItemScriptableObject item2)
    {
        // If they're the same object
        if(item1 == item2) return true;
        
        // Check if item1 is in item2's upgrade chain
        PassiveItemScriptableObject current = item1;
        while(current != null)
        {
            if(current.NextLevelPrefab != null)
            {
                var nextLevelItem = current.NextLevelPrefab.GetComponent<PassiveItem>();
                if(nextLevelItem != null && nextLevelItem.passiveItemData == item2)
                    return true;
            }
            
            // Check if item1 is a previous level of item2
            foreach(var upgrade in passiveItemUpgradeOptions)
            {
                if(upgrade.passiveItemData.NextLevelPrefab != null)
                {
                    var nextItem = upgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>();
                    if(nextItem != null && nextItem.passiveItemData == current)
                    {
                        if(upgrade.passiveItemData == item2)
                            return true;
                    }
                }
            }
            
            // Try to find the next level
            bool foundNext = false;
            foreach(var upgrade in passiveItemUpgradeOptions)
            {
                if(upgrade.passiveItemData == current && upgrade.passiveItemData.NextLevelPrefab != null)
                {
                    var nextItem = upgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>();
                    if(nextItem != null)
                    {
                        current = nextItem.passiveItemData;
                        foundNext = true;
                        break;
                    }
                }
            }
            
            if(!foundNext) break;
        }
        
        // Check if item2 is in item1's upgrade chain
        current = item2;
        while(current != null)
        {
            if(current.NextLevelPrefab != null)
            {
                var nextLevelItem = current.NextLevelPrefab.GetComponent<PassiveItem>();
                if(nextLevelItem != null && nextLevelItem.passiveItemData == item1)
                    return true;
            }
            
            // Check if item2 is a previous level of item1
            foreach(var upgrade in passiveItemUpgradeOptions)
            {
                if(upgrade.passiveItemData.NextLevelPrefab != null)
                {
                    var nextItem = upgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>();
                    if(nextItem != null && nextItem.passiveItemData == current)
                    {
                        if(upgrade.passiveItemData == item1)
                            return true;
                    }
                }
            }
            
            // Try to find the next level
            bool foundNext = false;
            foreach(var upgrade in passiveItemUpgradeOptions)
            {
                if(upgrade.passiveItemData == current && upgrade.passiveItemData.NextLevelPrefab != null)
                {
                    var nextItem = upgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>();
                    if(nextItem != null)
                    {
                        current = nextItem.passiveItemData;
                        foundNext = true;
                        break;
                    }
                }
            }
            
            if(!foundNext) break;
        }
        
        return false;
    }

    // Check if list already contains this weapon upgrade (or a different level of same weapon)
    bool ContainsWeaponUpgrade(List<WeaponUpgrade> list, WeaponUpgrade upgrade)
    {
        foreach(var item in list)
        {
            if(IsSameWeapon(item.weaponData, upgrade.weaponData))
                return true;
        }
        return false;
    }

    // Check if list already contains this passive item upgrade (or a different level of same item)
    bool ContainsPassiveItemUpgrade(List<PassiveItemUpgrade> list, PassiveItemUpgrade upgrade)
    {
        foreach(var item in list)
        {
            if(IsSamePassiveItem(item.passiveItemData, upgrade.passiveItemData))
                return true;
        }
        return false;
    }

    // Helper method to shuffle a list
    void ShuffleList<T>(List<T> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Setup UI for weapon upgrade
    void SetupWeaponUpgradeUI(UpgradeUI ui, WeaponUpgrade weaponUpgrade, bool slotsFull)
    {
        // Check if player already has this weapon (or any level of it)
        bool playerHasWeapon = false;
        int weaponSlotIndex = -1;
        
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && IsSameWeapon(weaponSlots[i].weaponData, weaponUpgrade.weaponData))
            {
                playerHasWeapon = true;
                weaponSlotIndex = i;
                break;
            }
        }
        
        if(!playerHasWeapon && !slotsFull)
        {
            // This is a new weapon for the player AND there are empty slots
            ui.upgradeButton.onClick.AddListener(() => {
                // Use the InventoryManager to find an empty slot
                int emptySlot = GetFirstEmptyWeaponSlot();
                if (emptySlot != -1)
                {
                    // Create the weapon and add it to the correct slot
                    GameObject newWeapon = Instantiate(weaponUpgrade.initialWeapon, transform.position, Quaternion.identity);
                    newWeapon.transform.SetParent(transform);
                    AddWeapon(emptySlot, newWeapon.GetComponent<WeaponController>());
                }
                else
                {
                    Debug.LogError("No empty weapon slots found!");
                }
            });
            ui.upgradeNameDisplay.text = weaponUpgrade.weaponData.Name;
            ui.upgradeDescriptionDisplay.text = weaponUpgrade.weaponData.Description;
        }
        else if(playerHasWeapon)
        {
            // Player has this weapon - check if it can be upgraded
            if(weaponSlots[weaponSlotIndex].weaponData.NextLevelPrefab != null)
            {
                int slotIndex = weaponSlotIndex; // Capture variable for lambda
                ui.upgradeButton.onClick.AddListener(() => LevelUpWeapon(slotIndex, weaponUpgrade.weaponUpgradeIndex));
                
                var nextLevelData = weaponSlots[weaponSlotIndex].weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData;
                ui.upgradeNameDisplay.text = nextLevelData.Name;
                ui.upgradeDescriptionDisplay.text = nextLevelData.Description;
            }
            else
            {
                // Max level reached - this shouldn't appear in available upgrades, but just in case
                DisableUpgradeUI(ui);
                return;
            }
        }
        else
        {
            // Player doesn't have this weapon AND slots are full - this shouldn't happen
            DisableUpgradeUI(ui);
            return;
        }
        
        ui.upgradeIcon.sprite = weaponUpgrade.weaponData.Icon;
        EnableUpgradeUI(ui);
    }

    // Setup UI for passive item upgrade
    void SetupPassiveItemUpgradeUI(UpgradeUI ui, PassiveItemUpgrade passiveUpgrade, bool slotsFull)
    {
        // Check if player already has this item (or any level of it)
        bool playerHasItem = false;
        int itemSlotIndex = -1;
        
        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] != null && IsSamePassiveItem(passiveItemSlots[i].passiveItemData, passiveUpgrade.passiveItemData))
            {
                playerHasItem = true;
                itemSlotIndex = i;
                break;
            }
        }
        
        if(!playerHasItem && !slotsFull)
        {
            // This is a new item for the player AND there are empty slots
            ui.upgradeButton.onClick.AddListener(() => {
                // Use the InventoryManager to find an empty slot
                int emptySlot = GetFirstEmptyPassiveItemSlot();
                if (emptySlot != -1)
                {
                    // Create the item and add it to the correct slot
                    GameObject newItem = Instantiate(passiveUpgrade.initialPassiveItem, transform.position, Quaternion.identity);
                    newItem.transform.SetParent(transform);
                    AddPassiveItem(emptySlot, newItem.GetComponent<PassiveItem>());
                }
                else
                {
                    Debug.LogError("No empty passive item slots found!");
                }
            });
            ui.upgradeNameDisplay.text = passiveUpgrade.passiveItemData.Name;
            ui.upgradeDescriptionDisplay.text = passiveUpgrade.passiveItemData.Description;
        }
        else if(playerHasItem)
        {
            // Player has this item - check if it can be upgraded
            if(passiveItemSlots[itemSlotIndex].passiveItemData.NextLevelPrefab != null)
            {
                int slotIndex = itemSlotIndex; // Capture variable for lambda
                ui.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(slotIndex, passiveUpgrade.passiveItemUpgradeIndex));
                
                var nextLevelData = passiveItemSlots[itemSlotIndex].passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData;
                ui.upgradeNameDisplay.text = nextLevelData.Name;
                ui.upgradeDescriptionDisplay.text = nextLevelData.Description;
            }
            else
            {
                // Max level reached - this shouldn't appear in available upgrades, but just in case
                DisableUpgradeUI(ui);
                return;
            }
        }
        else
        {
            // Player doesn't have this item AND slots are full - this shouldn't happen
            DisableUpgradeUI(ui);
            return;
        }
        
        ui.upgradeIcon.sprite = passiveUpgrade.passiveItemData.Icon;
        EnableUpgradeUI(ui);
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

    // Public method to check if there are empty weapon slots (for other scripts)
    public bool HasEmptyWeaponSlot()
    {
        return !AreWeaponSlotsFull();
    }

    // Public method to check if there are empty passive item slots (for other scripts)
    public bool HasEmptyPassiveItemSlot()
    {
        return !ArePassiveItemSlotsFull();
    }

    // Public method to get first empty weapon slot index
    public int GetFirstEmptyWeaponSlot()
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] == null)
                return i;
        }
        return -1; // No empty slots
    }

    // Public method to get first empty passive item slot index
    public int GetFirstEmptyPassiveItemSlot()
    {
        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] == null)
                return i;
        }
        return -1; // No empty slots
    }

    // Debug method to check inventory state
    public void DebugInventoryState()
    {
        Debug.Log("=== INVENTORY STATE DEBUG ===");
        
        Debug.Log("Weapon Slots (0-5):");
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null)
            {
                Debug.Log($"  Slot {i}: {weaponSlots[i].weaponData.Name} (Level {weaponLevels[i]})");
            }
            else
            {
                Debug.Log($"  Slot {i}: EMPTY");
            }
        }
        
        Debug.Log("Passive Item Slots (0-5):");
        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] != null)
            {
                Debug.Log($"  Slot {i}: {passiveItemSlots[i].passiveItemData.Name} (Level {passiveItemLevels[i]})");
            }
            else
            {
                Debug.Log($"  Slot {i}: EMPTY");
            }
        }
        
        Debug.Log($"Empty weapon slots: {CountEmptyWeaponSlots()}/6");
        Debug.Log($"Empty passive slots: {CountEmptyPassiveItemSlots()}/6");
        Debug.Log($"First empty weapon slot: {GetFirstEmptyWeaponSlot()}");
        Debug.Log($"First empty passive slot: {GetFirstEmptyPassiveItemSlot()}");
    }

    // Count empty weapon slots
    public int CountEmptyWeaponSlots()
    {
        int count = 0;
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] == null)
                count++;
        }
        return count;
    }

    // Count empty passive item slots
    public int CountEmptyPassiveItemSlots()
    {
        int count = 0;
        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] == null)
                count++;
        }
        return count;
    }
}