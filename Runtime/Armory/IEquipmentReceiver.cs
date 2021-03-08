using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysium.Equipment
{
    public interface IEquipmentReceiver
    {
        void Add(IEquipable _equipment);
    }
}