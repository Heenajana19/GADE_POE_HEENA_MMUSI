using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public class HealthPickupTile : PickupTile
    {
        public HealthPickupTile(Position position) : base(position)
        {
        }

        public override char Display
        {
            get { return '+'; }
        }

        public override void ApplyEffect(CharacterTile character)
        {
            character.Heal(10);
        }
    }
}
