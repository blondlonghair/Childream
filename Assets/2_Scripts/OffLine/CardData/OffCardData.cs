using UnityEngine;

namespace OffLine
{
    public class OffCardData : MonoBehaviour
    {
        public int id;
        public string name;
        public int power;

        public virtual void SkillEffect(OffPlayer caster, Monster target)
        {
            
        }
    }
    
    
}