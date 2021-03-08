using Elysium.Equipment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryTest : MonoBehaviour
{
    public Armory armory;
    public IEquipableDatabase equipment;

    private void OnValidate()
    {
        equipment.Refresh();
    }

    [ContextMenu("Equip1")]
    private void Equip1()
    {
        armory.Equip(equipment.Elements[0] as IEquipable);
    }

    [ContextMenu("Equip2")]
    private void Equip2()
    {
        armory.Equip(equipment.Elements[1] as IEquipable);
    }

    [ContextMenu("Equip3")]
    private void Equip3()
    {
        armory.Equip(equipment.Elements[2] as IEquipable);
    }

    [ContextMenu("Unequip1")]
    private void Unequip1()
    {
        armory.Unequip(armory.EquipmentSlots[0]);
    }

    [ContextMenu("Unequip2")]
    private void Unequip2()
    {
        armory.Unequip(armory.EquipmentSlots[1]);
    }

    [ContextMenu("Unequip3")]
    private void Unequip3()
    {
        armory.Unequip(armory.EquipmentSlots[2]);
    }
}
