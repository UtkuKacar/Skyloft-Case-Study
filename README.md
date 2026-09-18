# Skyloft Studios - Unity Developer Case Study

> Survivor.io-style mobile game prototype developed in **Unity 6** using the **Universal Render Pipeline (URP)** targeting **Android**.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.14f1-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP%20Mobile-green.svg)](https://unity.com/srp/Universal-Render-Pipeline)
[![Target Platform](https://img.shields.io/badge/Platform-Android-orange.svg)](https://www.android.com/)

---

## 🛠️ Project Specifications

- **Unity Version:** `Unity 6 (6000.3.14f1)`
- **Render Pipeline:** Universal Render Pipeline (URP - Mobile)
- **Target Platform:** Android
- **Input System:** Unity New Input System

---

## 📁 Project Structure

```text
Assets/
├── Animations/             # Animation clips and controllers
├── Art/
│   ├── Original/           # Preserved source models and textures
│   └── Optimized/          # Working directory for optimized assets
├── Materials/              # URP materials and shaders
├── Prefabs/                # Domain-separated prefabs (Player, Enemy, Weapons)
├── Scenes/
│   └── Game.unity          # Main gameplay scene
├── ScriptableObjects/      # Configuration assets
├── Scripts/                # Source code modules (Core, Player, Enemy, Combat, Spawning, UI, Data)
├── Settings/               # URP mobile configuration assets
└── Tests/                  # Unit and integration tests
```

---

## 🎮 Baseline Scene: `Game.unity`

The main scene maintains an organized, decoupled hierarchy:

```text
Game
├── Main Camera             # Top-down isometric camera framing the arena
├── Environment
│   ├── Arena               # 50x50 placeholder bounds plane with collider
│   ├── Directional Light   # Scene directional light
│   └── Global Volume       # URP post-processing and volume settings
├── Gameplay                # Gameplay actors container
├── Systems                 # Scene-level orchestrators
└── UI                      # User interface container
```

---

## 🚀 Getting Started

1. Open the project in **Unity 6 (6000.3.14f1)**.
2. Load the scene: `Assets/Scenes/Game.unity`.
3. Press **Play** in the Editor to inspect the baseline setup.