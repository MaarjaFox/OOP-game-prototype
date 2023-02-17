using BestGame.Units;
using UnityEngine;


namespace BestGame.Effects
{

    public class Modifier : MonoBehaviour
    {
        public enum ModifierType
        {
            ATTACK,
            DEALT_DAMAGE,
            DEALING_DAMAGE,
            //POISON,
            HEALTH,
            INITIATIVE,
            DEFENSE
        }
        public enum ModifierOperation
        {
            ADD,
            SUBTRACT,
            MULTIPLY,
            DIVIDE
        }

        [SerializeField]
        private ModifierOperation op;
        [SerializeField]
        private int amount;

        public bool IsBeneficial = true;

        [SerializeField]
        public ModifierType type;

        public bool IsEternal = true;

        private volatile int cooldown = 3;

        [SerializeField] private new string name;

        public int Apply(int baseAmount)
        {
            if (op == ModifierOperation.ADD)
            {
                return baseAmount + amount;
                //Debug.Log("HEALINGTEST");
            }

            return baseAmount;
        }

        public virtual bool IsApplicable(BaseUnit unit)
        {
            return true;
        }

        public void OnTurnUpdated()
        {
            if (!IsEternal)
            {
                cooldown--;
                Debug.Log("Decreasing cooldown: " + cooldown.ToString());
                if (cooldown == 0)
                {
                    Debug.Log("Deleting");
                    DestroyImmediate(this.gameObject, true);
                }
            }

        }
        
    }
}