# Skyloft Studios - Unity Developer Case Study

> Survivor.io-style mobile game prototype developed in **Unity 6** using the **Universal Render Pipeline (URP)** targeting **Android**.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.14f1-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP%20Mobile-green.svg)](https://unity.com/srp/Universal-Render-Pipeline)
[![Target Platform](https://img.shields.io/badge/Platform-Android-orange.svg)](https://www.android.com/)
[![Input System](https://img.shields.io/badge/Input%20System-New%20Input%20System-brightgreen.svg)](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)

---

## 🛠️ Project Specifications

- **Unity Version:** Unity 6 (`6000.3.14f1`)
- **Render Pipeline:** Universal Render Pipeline (URP - Mobile)
- **Target Platform:** Android
- **Orientation:** Portrait
- **Input System:** Unity New Input System

---

## 📁 Project Structure

```text
Assets/
├── Animations/          # Animation clips and controllers
├── Art/
│   ├── Original/        # Preserved source models and textures
│   └── Optimized/       # Working directory for optimized assets
├── Materials/           # URP materials
├── Prefabs/             # Player, Enemy and Weapon prefabs
├── Scenes/
│   └── Game.unity       # Main gameplay scene
├── ScriptableObjects/   # Gameplay configuration assets
├── Scripts/             # Core gameplay modules
├── Settings/            # URP and project configuration
└── Tests/               # Tests and validation
```

---

## 🎮 Main Scene: `Game.unity`

The project currently contains a single gameplay scene with a simple mobile-oriented arena.

```text
Game
├── Main Camera
├── Environment
│   ├── Arena
│   ├── Directional Light
│   └── Global Volume
├── Gameplay
│   └── Player
├── Systems
└── UI
    ├── Canvas
    └── EventSystem
```

---

## ✨ Implemented Features

### Player Movement
- Virtual joystick movement for mobile
- Keyboard controls for Editor testing
- CharacterController based movement
- Smooth rotation toward movement direction
- Camera follow system
- Configurable movement parameters

### Player Animation
- Humanoid animation setup
- Rifle Idle animation
- Walk With Rifle animation
- Idle / Move transitions synchronized with movement input
- Script-driven movement with root motion disabled

### Player & Weapon
- Supplied player model and textures
- URP player materials
- Supplied rifle model and textures
- Reusable Rifle prefab
- Rifle attached to the player's right-hand bone
- Weapon automatically follows character animations

### Mobile UI
- Portrait-oriented Canvas
- Responsive virtual joystick
- Touch and mouse input
- Input dead-zone handling
- Automatic joystick recentering

---

## 🤖 AI & Unity MCP

AI-assisted development is used throughout the project for implementation, debugging and technical review.

Unity MCP is integrated into the workflow for Editor-level development and verification.

A complete MCP-assisted workflow has been used for the player animation setup:

```text
Read Unity state ➔ Apply changes ➔ Verify in Unity
```



## 🎮 Controls

### Mobile
- Use the on-screen virtual joystick to move the player.

### Unity Editor
- Use **W**, **A**, **S**, **D** or the **arrow keys**.

---

## 🚀 Getting Started

1. Open the project in **Unity 6 (6000.3.14f1)**.
2. Load `Assets/Scenes/Game.unity`.
3. Press **Play**.
4. Use the virtual joystick or keyboard controls to move the player.

---

## 📌 Current Status

### Completed ✅
- [x] Project and Android setup
- [x] URP configuration
- [x] Base arena
- [x] Player prefab
- [x] Virtual joystick
- [x] Player movement
- [x] Player rotation
- [x] Camera follow
- [x] Player materials and textures
- [x] Rifle integration
- [x] Humanoid animation setup
- [x] Rifle Idle and Walk locomotion
- [x] Unity MCP integration

### In Progress / Remaining ⏳
- [ ] Player health
- [ ] Enemy system
- [ ] Automatic attack system
- [ ] Wave spawning
- [ ] Difficulty levels
- [ ] Game timer
- [ ] Win / Lose states
- [ ] Kill tracking
- [ ] Persistent total kills
- [ ] Object pooling
- [ ] Performance profiling
- [ ] Asset optimization
- [ ] Runtime optimization
- [ ] Android build and device testing
