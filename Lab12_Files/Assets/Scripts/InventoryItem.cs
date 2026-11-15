using UnityEngine;

namespace Inventory
{
    // Plain data class representing one item in the inventory
    // [System.Serializable] lets Unity display a List<InventoryItem> in the Inspector
    [System.Serializable]
    public class InventoryItem
    {
        // Private backing fields shown in the Inspector via [SerializeField]
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private int _value;

        // Public read-only properties
        public int ID => _id;
        public string Name => _name;
        public int Value => _value;

        public InventoryItem(int id, string name, int value)
        {
            _id = id;
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return $"[ID: {_id}] {_name} (Value: {_value})";
        }
    }
}