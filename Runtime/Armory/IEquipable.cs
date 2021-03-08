using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysium.Equipment
{
    public interface IEquipable
    {
        string EquipmentName { get; }
        EquipmentSlotType Slot { get; }
    }

    [System.Serializable]
    public class IEquipableDatabase : IDatabase<IEquipable> { }
}