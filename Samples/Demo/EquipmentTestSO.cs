using Elysium.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTestSO", menuName = "Scriptable Objects/Equipment/Equipment")]
public class EquipmentTestSO : ScriptableObject, IEquipable
{
    public EquipmentSlotType slot;
    public EquipmentSlotType Slot => slot;

    public string EquipmentName => name;
}
