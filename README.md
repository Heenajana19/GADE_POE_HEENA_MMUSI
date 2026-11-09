# Game Development Final POE

C# Windows Forms action-adventure game for GADE6122.

## Contributors

- Heena Jana
- Mmusi Phoko

## Running the project

1. Install Visual Studio with the .NET desktop development workload
   and .NET 8 SDK.
2. Open GADE_POE_HEENA_JANA.csproj.
3. Build the project.
4. Press F5 to run.

## Controls

| Action | Key |
|---|---|
| Move up | W or Up Arrow |
| Move right | D or Right Arrow |
| Move down | S or Down Arrow |
| Move left | A or Left Arrow |
| Attack up | I |
| Attack right | L |
| Attack down | K |
| Attack left | J |

## Game rules

- Complete 10 levels.
- The hero starts with 40 health and 5 attack power.
- Each level has one more enemy than the previous level.
- Enemies move after every two successful hero movements.
- A successful hero attack triggers enemy attacks.
- Defeat every enemy to unlock the level exit.
- Enter the unlocked exit to advance.
- Complete the final level and enter its exit to win.
- The game ends if the hero's health reaches zero.
- Defeated enemies remain visible as x.

## Enemies

| Enemy | Health | Attack | Behaviour |
|---|---|---|---|
| Grunt | 10 | 1 | Moves randomly; attacks an adjacent hero |
| Warlock | 10 | 5 | Stationary; attacks all eight surrounding positions |
| Tyrant | 15 | 5 | Moves towards the hero; attacks its row and column |

Warlocks and tyrants can damage other enemies.

Enemy generation:
- Grunt: 50%
- Warlock: 30%
- Tyrant: 20%

## Pickups

One pickup is generated per level.

- Health pickup: restores 10 HP, capped at maximum health.
- Attack buff: doubles damage for the next three successful attacks.
- Attack buffs stack.

Pickup generation:
- Health: two-thirds probability.
- Attack buff: one-third probability.

## Saving and loading

Use the Save Game and Load Game buttons.

The binary save preserves the current level, characters, health,
pickups, exit state, remaining attack buffs, game state and movement
counter.

Save location:
%LOCALAPPDATA%\GADE_POE_HEENA_JANA\savegame.bin
