using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public abstract class CharacterTile : Tile
    {
        private int hitPoints;
        private int maxHitPoints;
        private int attackPower;
        private int doubleDamageCount;

        // Vision is rebuilt after loading instead of being saved.
        [NonSerialized]
        private Tile[] vision;

        public int CurrentHealth
        {
            get { return hitPoints; }
        }

        public int MaxHealth
        {
            get { return maxHitPoints; }
        }

        public int AttackPower
        {
            get { return attackPower; }
        }

        public int DoubleDamageCount
        {
            get { return doubleDamageCount; }
        }

        public bool IsDead
        {
            get { return hitPoints <= 0; }
        }

        protected Tile[] Vision
        {
            get { return vision; }
        }

        protected CharacterTile(
            Position position,
            int hitPoints,
            int attackPower)
            : base(position)
        {
            this.hitPoints = hitPoints;
            maxHitPoints = hitPoints;
            this.attackPower = attackPower;

            vision = new Tile[4];
        }

        public void UpdateVision(Level level)
        {
            if (vision == null)
            {
                vision = new Tile[4];
            }

            vision[0] = level.GetTile(X, Y - 1);
            vision[1] = level.GetTile(X + 1, Y);
            vision[2] = level.GetTile(X, Y + 1);
            vision[3] = level.GetTile(X - 1, Y);
        }

        public Tile GetVisibleTile(Direction direction)
        {
            int index = (int)direction;

            if (vision == null || index < 0 || index >= 4)
            {
                return null;
            }

            return vision[index];
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
            {
                hitPoints = Math.Max(0, hitPoints - damage);
            }
        }

        public void Attack(CharacterTile target)
        {
            if (IsDead || target == null ||
                target.IsDead || target == this)
            {
                return;
            }

            int damage = attackPower;

            if (doubleDamageCount > 0)
            {
                damage *= 2;
                doubleDamageCount--;
            }

            target.TakeDamage(damage);
        }

        public void Heal(int amount)
        {
            if (!IsDead && amount > 0)
            {
                hitPoints = Math.Min(maxHitPoints, hitPoints + amount);
            }
        }

        public void SetDoubleDamage(int numberOfAttacks)
        {
            if (numberOfAttacks > 0)
            {
                doubleDamageCount += numberOfAttacks;
            }
        }
    }
}
