using UnityEngine;

namespace OffLine
{
    public enum status
    {
        Strength = 1 << 0,
        Agility = 1 << 1,
        Weakness = 1 << 2,
        Thorn = 1 << 3
    }
    
    public class Unit : MonoBehaviour
    {
        public int maxHp;
        public int curHp;
        public int armor;
    }
}