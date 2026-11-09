using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class AttackBuffPickupTile : PickupTile
    {
        public AttackBuffPickupTile(Position position) : base(position)
        {
        }

        public override char Display
        {
            get { return '*'; }
        }

        public override void ApplyEffect(CharacterTile character)
        {
            character.SetDoubleDamage(3);
        }
    }
}
