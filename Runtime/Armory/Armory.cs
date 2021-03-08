using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Elysium.Utils.Attributes;
using UnityEngine.Events;
using Elysium.Core;
using Elysium.Utils;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Elysium.Equipment
{
    [CreateAssetMenu(fileName = "ArmorySO", menuName = "Scriptable Objects/Equipment/Armory")]
    public class Armory : ScriptableObject, ISavable
    {
        [RequireInterface(typeof(IEquipmentReceiver))]
        [SerializeField] protected ScriptableObject inventory;

        [Separator("Runtime Equipment", true)]
        [SerializeField] private EquipmentSlot[] equipmentSlots;

        [Separator("Default Equipment", true)]
        [SerializeField] private EquipmentSlot[] defaultEquipmentSlots;

        [Separator("All Equipment", true)]
        [SerializeField] private IEquipableDatabase equipmentDatabase;

        public IEquipmentReceiver Inventory => inventory as IEquipmentReceiver;
        public IReadOnlyList<EquipmentSlot> EquipmentSlots => equipmentSlots;

        public event UnityAction<IEquipable> OnEquipped;
        public event UnityAction<IEquipable> OnUnequipped;
        public event UnityAction OnValueChanged;

        public IEquipable Peek(EquipmentSlotType _slotType)
        {
            EquipmentSlot slot = GetSlotByType(_slotType);
            return slot.Equipment;
        }

        public bool IsEmptySlot(EquipmentSlotType _slotType)
        {
            EquipmentSlot slot = GetSlotByType(_slotType);
            return slot.Equipment == null;
        }

        public bool HasEquipped(IEquipable _equipment)
        {
            EquipmentSlot slot = GetSlotByType(_equipment.Slot);
            return slot.Equipment == _equipment;
        }

        public bool Equip(IEquipable _equipment)
        {
            if (_equipment == null) { throw new SystemException("trying to equip null equipment"); }

            EquipmentSlot slot = GetSlotByType(_equipment.Slot);
            Unequip(slot);
            slot.Equipment = _equipment;
            Debug.Log($"Equipped {_equipment.EquipmentName}");
            OnEquipped?.Invoke(_equipment);
            return true;
        }

        public bool Unequip(EquipmentSlotType _slotType)
        {
            EquipmentSlot slot = GetSlotByType(_slotType);
            return Unequip(slot);
        }

        public bool Unequip(EquipmentSlot _slot)
        {
            IEquipable previousEquipment = _slot.Equipment;
            _slot.Equipment = null;
            if (previousEquipment != null)
            {
                Debug.Log($"Unequipped {previousEquipment.EquipmentName}");
                OnUnequipped?.Invoke(previousEquipment);
            }

            return true;
        }

        [ContextMenu("Refresh Runtime Equipment Slots")]
        private void BuildEmptyRuntimeEquipmentSlots() => BuildEmptyEquipmentSlots(ref equipmentSlots);

        [ContextMenu("Refresh Default Equipment Slots")]
        private void BuildEmptyDefaultEquipmentSlots() => BuildEmptyEquipmentSlots(ref defaultEquipmentSlots);        

        private void OnEnable()
        {
            if (inventory == null)
            {
                Debug.LogError("inventory reference not set in armory");
                return;
            }

            OnUnequipped += Inventory.Add;
        }

        private void OnDisable()
        {
            OnUnequipped -= Inventory.Add;
        }

        private EquipmentSlot GetSlotByType(EquipmentSlotType slotType) => equipmentSlots.Single(x => x.Slot == slotType);        

        private void BuildEmptyEquipmentSlots(ref EquipmentSlot[] _array)
        {
            EquipmentSlotType[] slots = EnumTools.GetAllEnums<EquipmentSlotType>();
            int numOfSlots = slots.Length;
            _array = new EquipmentSlot[numOfSlots];

            for (int i = 0; i < numOfSlots; i++)
            {
                _array[i] = new EquipmentSlot(slots[i]);
            }
        }

        // ---------------------------------- ISavable ---------------------------------- //

        public ushort Size
        {
            get
            {
                string[] items = equipmentSlots.Where(x => x.EquipmentSO != null).Select(x => x.EquipmentSO.name).ToArray();
                Stream stream = new MemoryStream();
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, items);
                ushort length = (ushort)stream.Length;
                stream.Dispose();

                return length;
            }
        }

        public void Load(BinaryReader reader)
        {
            BuildEmptyEquipmentSlots(ref equipmentSlots);
            int size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                string name = reader.ReadString();

                IEquipable equip = equipmentDatabase.GetElementByName(name);
                equipmentSlots[i] = new EquipmentSlot(equip.Slot);
                equipmentSlots[i].Equipment = equip;
            }
        }

        public void LoadDefault()
        {
            Debug.Log($"OnLoad: loading default values for inventory {name}");

            if (defaultEquipmentSlots == null) 
            {
                BuildEmptyEquipmentSlots(ref defaultEquipmentSlots);
            }

            int numOfSlots = EnumTools.GetAllEnums<EquipmentSlotType>().Length;
            if (defaultEquipmentSlots.Length != numOfSlots) { throw new System.Exception("default array isn't null, but doesn't contain all slot types"); }

            Array.Copy(defaultEquipmentSlots, equipmentSlots, numOfSlots);
            OnValueChanged?.Invoke();
        }

        public void Save(BinaryWriter writer)
        {
            string[] items = equipmentSlots.Where(x => x.EquipmentSO != null).Select(x => x.EquipmentSO.name).ToArray();
            writer.Write(items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                writer.Write(items[i]);
            }
        }

        private void OnValidate()
        {
            equipmentDatabase.Refresh();
#if UNITY_EDITOR
            EditorPlayStateNotifier.RegisterOnExitPlayMode(this, () =>
            {
                // Debug.Log($"OnPlayModeExit: resetting inventory {name}");
                BuildEmptyEquipmentSlots(ref equipmentSlots);
            });
#endif
        }
    }
}