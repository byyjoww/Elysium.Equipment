using Elysium.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReceiverSO", menuName = "Scriptable Objects/Equipment/Receiver")]
public class EquipmentReceiverTestSO : ScriptableObject, IEquipmentReceiver
{
    public void Add(IEquipable _equipment)
    {
        Debug.Log($"Received equipment {_equipment.EquipmentName}");
    }
}
