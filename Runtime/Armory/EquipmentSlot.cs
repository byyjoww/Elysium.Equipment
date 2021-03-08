using UnityEngine;
using Elysium.Utils.Attributes;
using Elysium.Equipment;

namespace Elysium.Equipment
{
    [System.Serializable]
    public class EquipmentSlot
    {
        [RequireInterface(typeof(IEquipable))]
        [SerializeField, ReadOnly] private ScriptableObject equipmentSO = default;

        [SerializeField] private EquipmentSlotType slotType = default;

        public EquipmentSlot(EquipmentSlotType _slotType)
        {
            this.slotType = _slotType;
            equipmentSO = default;
        }

        public ScriptableObject EquipmentSO => equipmentSO;
        public EquipmentSlotType Slot => slotType;

        public IEquipable Equipment
        {
            get => equipmentSO as IEquipable;
            set => equipmentSO = (ScriptableObject)value;
        }
    }

    public enum EquipmentSlotType
    {
        DEFAULT,
        SLOT_TYPE_01,
        SLOT_TYPE_02,
    }
}