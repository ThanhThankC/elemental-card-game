# Elemental Card Game

A Yugioh-inspired turn-based card battle game built with **Unity 2022.3 (C#)**, targeting **Android**. Battle with elemental monsters, cast spells, and set traps across a strategic field system.

> **Work in progress** — approximately 20–30% complete.

---

## About the Game

Elemental Card Game is a digital card battle game where two players duel using decks of 50 cards. Each player manages monsters, spells, and traps across a structured field, trying to reduce the opponent's HP to zero.

The game features an **elemental system** where monsters can belong to one or more elements, and Special cards (SS) can buff specific monster types with matching elemental symbols.

---

## Current Features

- [x] Card creation & data system (Monster, Spell, Trap, Special cards)
- [x] Card dealing — draw cards into hand at game start
- [x] Hand layout & card sorting
- [x] Card selection from hand
- [x] Summon monsters to the field
- [x] Tribute system (sacrifice monsters to summon high-tier cards)
- [x] Send cards to Graveyard
- [ ] Spell card activation system *(in progress)*
- [ ] Trap card system
- [ ] Combat / battle phase
- [ ] HP damage system
- [ ] AI opponent
- [ ] Online multiplayer

---

## Card Types

| Type | Description |
|------|-------------|
| **Monster** | Has ATK, DEF, star tier (1–10), element(s), and a special ability |
| **Spell (P)** | Instant effect; Ps variant can be placed on field; Ps* has priority activation |
| **Trap (B)** | Activated in response to opponent actions; B* has priority activation |
| **Special (SS)** | Persistent buff cards that enhance monsters with matching elemental symbols |

---

## Field Layout

Each player's side of the field contains:

| Zone | Slots | Purpose |
|------|-------|---------|
| Deck Zone | 1 | Holds the player's 50-card deck |
| Hand Zone | 7 | Cards in hand (visible only to owner) |
| Monster Zone | 7 | Summon & battle monsters here |
| Trap Zone | 4 | Place face-down trap cards |
| Spell Zone | 3 | Activate spell or special cards |
| Graveyard | 1 | Destroyed/used cards |

---

## Tech Stack

- **Engine:** Unity 2022.3.32f1
- **Language:** C#
- **Platform:** Android
- **IDE:** Visual Studio

---

## Game Rules (Summary)

- Each player brings a deck of exactly **50 cards**
- Each player starts with **10,000 HP** — last one standing wins
- Game begins by drawing **5 cards** from the deck
- Turn order is decided randomly at the start
- Players take turns playing cards, summoning monsters, and attacking

---

## Getting Started

### Requirements

- Unity **2022.3.32f1**
- Android Build Support module installed

### Run the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/elemental-card-game.git
   ```
2. Open the project in **Unity Hub** → Add → select the cloned folder
3. Open the main scene in `Assets/Scenes/`
4. Press **Play** to run in the editor

---

## Project Structure
```
Assets/
├── Fonts/
└── Scripts/
    ├── Core/
    │   ├── Config/          # CardVisualConfig, GameRules
    │   ├── Data/            # CardData
    │   ├── Enums/           # BuffType, CardState, CardType, ElementType, MonsterTier, SpellEffectID
    │   ├── Manager/         # GamePhaseManager
    │   └── Utils/           # CardAnimator, IconDatabase, ObjectPool
    └── Gameplay/
        ├── Cards/           # Card, CardHighlighter
        ├── Deck/            # DeckManager, DiscardManager
        ├── Draw/            # DrawPhaseController, DrawPhaseUI
        ├── Field/           # FieldSlot, FieldZone, MonsterZone, SpellZone
        ├── Selection/       # CardSelectionManager, SelectionContext, SelectionValidator
        ├── Set/
        ├── Spells/
        │   ├── Effects/     # BuaHoMenh, BuffATKDEF, DestroyMonster, DrawCard effects
        │   ├── IContinuousSpellEffect, ISpellEffect
        │   └── SpellContext, SpellController, SpellEffectRegistry
        ├── Summon/          # SummonController
        └── UI/
            ├── HUD/         # FieldHighlightController, SpellTargetingUI
            ├── Interactions/ # CardClickHandler, CardDragHandler, CardTransformHelper
            ├── Layouts/     # HandLayoutManager
            └── Menus/       # CardActionMenu
```

---

## Roadmap

- [ ] Complete spell card system
- [ ] Implement trap card system
- [ ] Build combat / battle phase
- [ ] Add AI opponent
- [ ] Online multiplayer support
- [ ] Card collection & deck builder UI

---

## License

This project is for personal learning and portfolio purposes.
