using UnityEngine;
using System.Collections.Generic;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int _itemCount = 10;
        [SerializeField] private int _maxID = 1000;
        [SerializeField] private int _maxValue = 500;

        [Header("Search Test Inputs")]
        [SerializeField] private string _searchName = "Potion";
        [Tooltip("Leave at 0 to search for a random ID that exists in the inventory.")]
        [SerializeField] private int _searchID = 0;

        [Space, SerializeField]
        private List<InventoryItem> _inventory = new List<InventoryItem>();

        private readonly string[] _itemNames =
        {
            "Sword", "Shield", "Potion", "Bow", "Arrow",
            "Helmet", "Boots", "Ring", "Amulet", "Scroll"
        };

        private void Start()
        {
            InitializeInventory();
            RunAllTests();
        }


        // SETUP

        // Populate the inventory with randomized IDs and values
        [ContextMenu("Initialize Inventory")]
        public void InitializeInventory()
        {
            _inventory.Clear();

            // Can't have more unique IDs than the ID range allows
            int count = Mathf.Min(_itemCount, _maxID);
            HashSet<int> usedIDs = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                // Keeps rolling until an ID that hasn't been used yet is found
                int id;
                do
                {
                    id = Random.Range(1, _maxID + 1);
                }
                while (!usedIDs.Add(id));

                string itemName = _itemNames[i % _itemNames.Length];
                int value = Random.Range(1, _maxValue + 1);

                _inventory.Add(new InventoryItem(id, itemName, value));
            }

            PrintInventory("Initial Inventory (unsorted)");
        }


        // TASK 1: LINEAR SEARCH BY NAME

        // Checks each item one by one until a match is found
        public InventoryItem LinearSearchByName(string itemName)
        {
            for (int i = 0; i < _inventory.Count; i++)
            {
                if (string.Equals(_inventory[i].Name, itemName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return _inventory[i];
                }
            }

            return null;
        }


        // TASK 2: BINARY SEARCH BY ITEM ID

        // Requires the list to be sorted by ID, so we sort first Iterative version (no recursion, no stack overflow risk)
        public InventoryItem BinarySearchByID(int targetID)
        {
            QuickSortByID();

            int low = 0;
            int high = _inventory.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2; // Avoids int overflow vs (low + high) / 2
                int midID = _inventory[mid].ID;

                if (midID == targetID)
                {
                    return _inventory[mid];
                }

                if (midID < targetID)
                {
                    low = mid + 1; // Target is in the right half
                }
                else
                {
                    high = mid - 1; // Target is in the left half
                }
            }

            return null;
        }


        // TASK 3: QUICK SORT BY VALUE

        public void QuickSortByValue()
        {
            QuickSort(0, _inventory.Count - 1, (a, b) => a.Value.CompareTo(b.Value));
        }

        // Same Quick Sort reused for Task 2's "sort by ID first" step
        public void QuickSortByID()
        {
            QuickSort(0, _inventory.Count - 1, (a, b) => a.ID.CompareTo(b.ID));
        }

        private void QuickSort(int low, int high, System.Comparison<InventoryItem> compare)
        {
            if (low < high)
            {
                int pivotIndex = Partition(low, high, compare);

                QuickSort(low, pivotIndex - 1, compare); // Sort left of pivot
                QuickSort(pivotIndex + 1, high, compare); // Sort right of pivot
            }
        }

        // Uses the last element as the pivot
		// Everything <= pivot moves left of it, everything > pivot moves right
		// Returns the pivot's final index
        private int Partition(int low, int high, System.Comparison<InventoryItem> compare)
        {
            InventoryItem pivot = _inventory[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (compare(_inventory[j], pivot) <= 0)
                {
                    i++;
                    Swap(i, j);
                }
            }

            Swap(i + 1, high);
            return i + 1;
        }

        private void Swap(int a, int b)
        {
            InventoryItem temp = _inventory[a];
            _inventory[a] = _inventory[b];
            _inventory[b] = temp;
        }


        // TESTS

        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            TestLinearSearch();
            TestBinarySearch();
            TestQuickSort();
        }

        [ContextMenu("Test Linear Search")]
        public void TestLinearSearch()
        {
            InventoryItem found = LinearSearchByName(_searchName);
            Debug.Log(found != null
                ? $"[Linear Search] Found \"{_searchName}\": {found}"
                : $"[Linear Search] \"{_searchName}\" not found.");
        }

        [ContextMenu("Test Binary Search")]
        public void TestBinarySearch()
        {
            if (_inventory.Count == 0)
            {
                Debug.LogWarning("[Binary Search] Inventory is empty. Initialize it first.");
                return;
            }

            int targetID = _searchID > 0 ? _searchID : _inventory[Random.Range(0, _inventory.Count)].ID;

            InventoryItem found = BinarySearchByID(targetID);
            PrintInventory("Inventory Sorted by ID");

            Debug.Log(found != null
                ? $"[Binary Search] Found ID {targetID}: {found}"
                : $"[Binary Search] ID {targetID} not found.");
        }

        [ContextMenu("Test Quick Sort")]
        public void TestQuickSort()
        {
            QuickSortByValue();
            PrintInventory("Inventory Sorted by Value (Quick Sort)");
        }

        private void PrintInventory(string title)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"--- {title} | {_inventory.Count} items ---");

            foreach (InventoryItem item in _inventory)
            {
                sb.AppendLine(item.ToString());
            }

            Debug.Log(sb.ToString());
        }
    }
}