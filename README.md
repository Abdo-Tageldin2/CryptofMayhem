# Crypt of mayhem

A fast-paced 1v1 card battle game built in Unity, inspired by Dungeon Mayhem. Play cards to deal damage, heal, shield up, and outsmart an AI opponent across three difficulty levels.

Built for the Mobile App Development course.

---

## Features

- **13 unique cards** with custom artwork — damage, heal, shield, draw, and play-again effects
- **Enemy AI** that adapts based on difficulty — scores cards by game state and plays smart
- **3 difficulty levels** — Easy (12 rounds, weak AI), Normal (10 rounds), Hard (8 rounds, aggressive AI)
- **Firebase Authentication** — sign up or log in with email/password
- **Cloud save** — wins and losses sync to Firebase Firestore
- **Guest mode** — play without an account
- **Profile stats** — track your wins, losses, and draws
- **Object pooling** — cards are recycled instead of created/destroyed each turn
- **Card animations** — smooth slide-to-play using coroutines

---

## How to Download and Run

### Requirements

- **Unity 6000.4.1f1** (or later Unity 6 LTS)
- macOS or Windows
- Internet connection (for Firebase — or use Guest mode offline)

### Setup

1. **Clone or download** this repository
2. Open **Unity Hub**
3. Click **Add** > **Add project from disk**
4. Navigate to the project folder and select it
5. Unity will import assets and rebuild the Library (takes a few minutes the first time)
6. Once loaded, check the **Console** tab for any errors — there should be none

### Running the Game

1. Open the **auth** scene: `Assets/auth.unity`
2. Press the **Play** button in Unity
3. The game starts on the login screen

> **Note:** If Firebase doesn't initialize (e.g. on some macOS setups), a 6-second timeout kicks in and you'll see "Login unavailable, use Guest". Guest mode works fully without Firebase.

---

## How to Play

### Game Flow

1. **Auth Screen** — Sign up, log in, or continue as guest
2. **Home Screen** — Choose difficulty, view your profile, or read How to Play
3. **Game Screen** — Battle the AI with your card deck

### Battle Rules

| Rule | Value |
|---|---|
| Starting HP | 10 |
| Starting hand | 3 cards |
| Draw per turn | 1 card |
| Actions per turn | 1 (unless a PlayAgain card gives you extra) |
| End turn | Press "End Turn" after playing at least 1 card |
| Win condition | Reduce enemy HP to 0, or have more HP when rounds run out |

### Card Effects

| Effect | What it does |
|---|---|
| Damage | Deal X damage to the opponent |
| Heal | Restore X HP (capped at max HP) |
| Shield | Block X incoming damage |
| Draw | Draw X extra cards from your deck |
| PlayAgain | Get 1 extra action this turn |

### Cards in the Deck

Both you and the AI use the same 13-card deck:

| Card | Effects |
|---|---|
| Rusty Axe | 1 Damage |
| Quick Shank | 1 Damage |
| Headbutt | 1 Damage, 1 Shield |
| Smite | 2 Damage |
| Double Damage | 2 Damage |
| Triple Damage | 3 Damage |
| Prayer | 1 Heal |
| Double Heal | 2 Heal |
| Double Shield | 2 Shield |
| Shield and Draw | 1 Shield, 1 Draw |
| Triple Draw | 3 Draw |
| Draw and Play Again | 1 Draw, 1 PlayAgain |
| Heal and Play Again | 1 Heal, 1 PlayAgain |

---

## Project Structure

```
Assets/
  Scripts/
    Battle/       Game loop, AI, shields, deck, effects, scoring
    Auth/         Firebase auth, login/signup, error mapping
    Home/         Home screen, profile display
    Data/         Card/deck ScriptableObjects, enums, constants
    UI/           Card display, HUD, animations, object pool
  ScriptableObjects/
    Cards/        13 card assets (CardData)
    Decks/        PlayerDeck + EnemyDeck (DeckData)
  Art/
    Cards/        Card artwork
    Icons/        Effect icons (attack, heal, shield, draw, play again)
    UI/           Screen mockups
  Scenes/
    auth          Login / signup / guest
    home          Main menu, profile, difficulty picker
    game          The card battle
```

---

## Tech Stack

- **Unity 6000.4.1f1** (Unity 6 LTS)
- **C#**
- **Firebase Auth** — email/password authentication
- **Firebase Firestore** — cloud storage for player stats
- **TextMeshPro** — UI text rendering
- **Universal Render Pipeline (URP)**

---

## Build Settings

Scenes are set in this order:

1. `auth` (index 0)
2. `home` (index 1)
3. `game` (index 2)

If the scene order gets lost, go to **File > Build Settings** and drag the scenes in this order.
