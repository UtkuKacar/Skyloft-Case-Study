# 🎮 Skyloft Studios - Unity Developer Case Study

> Survivor.io-style mobile game prototype developed in **Unity 6** with **URP**, targeting **Android**.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.14f1-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP-green.svg)](https://unity.com/srp/Universal-Render-Pipeline)
[![Platform](https://img.shields.io/badge/Platform-Android-orange.svg)](https://www.android.com/)

## 🛠️ Project

* **Unity:** 6000.3.14f1
* **Render Pipeline:** Universal Render Pipeline (URP)
* **Platform:** Android / Landscape
* **Input:** Unity New Input System
* **Scripting Backend:** IL2CPP
* **Android Architecture:** ARM64
* **Graphics API:** Vulkan
* **Scenes:** `MainMenu.unity`, `Game.unity`, `Result.unity`

Main packages and tools used in the project:

* Universal Render Pipeline
* Input System
* UnityMeshSimplifier
* Unity Coplay MCP

## ✨ Implemented Features

* Virtual joystick and keyboard movement
* Player rotation and camera follow
* Rifle Idle, Walk and Death animations
* Player and enemy health systems
* Player HUD and enemy health bars
* Automatic nearest-enemy targeting
* Automatic rifle fire
* Enemy pursuit and melee attacks
* Animation-synchronized enemy damage
* Lightweight enemy obstacle avoidance
* Wave-based enemy spawning
* Safe spawn-position validation
* Easy / Medium / Hard difficulty selection
* Difficulty-based spawn pressure and enemy movement speed
* 3-minute survival timer
* Session kill counter
* Persistent total kill count
* Win / Lose game flow
* Result screen with kill statistics
* Replay and Main Menu navigation
* Fade transitions between scenes
* Lightweight placeholder arena built with Unity primitives
* Mobile-focused enemy rendering optimization
* Physical Android profiling and baseline-vs-optimized comparison

## 🗺️ Arena

The game uses a lightweight **50 × 50** placeholder arena built with Unity primitive objects.

The layout contains an open central combat area, wide movement lanes and simple obstacles designed to support high enemy density without requiring a complex navigation system.

Players and enemies collide with the environment, while direct **Player ↔ Enemy physical blocking is disabled** to prevent the Player from becoming trapped inside enemy crowds.

Enemies use lightweight direct pursuit with obstacle avoidance around arena structures.

## 🎯 Difficulty

All difficulty levels use the same `Game.unity` scene and are configured through `DifficultyConfig` ScriptableObjects.

| Difficulty | Spawn Interval | Enemies / Wave | Max Enemies | Enemy Speed |
|---|---:|---:|---:|---:|
| Easy | 4.0 s | 2 | 12 | 4.5 |
| Medium | 2.5 s | 3 | 24 | 5.0 |
| Hard | 1.5 s | 4 | 40 | 5.5 |

Player movement speed is **5.5**.

This keeps the arena and gameplay systems identical while changing the amount of pressure applied to the Player.

## ⚔️ Combat

The Player automatically targets the nearest living enemy within weapon range and fires without requiring a separate attack input.

Enemies continuously pursue the Player and switch into a melee attack state when they enter attack range.

Enemy damage is synchronized with the attack animation so damage is applied at the intended impact point rather than immediately when an enemy enters range.

The combat setup is designed to keep enemies threatening while avoiding direct physical blocking between the Player and the crowd.

## 🌊 Spawning

Enemies are spawned through `WaveSpawner` using the selected `DifficultyConfig`.

Spawn positions are validated before instantiation to avoid:

* spawning inside arena obstacles
* spawning outside playable bounds
* spawning too close to the Player
* excessive overlap between newly spawned enemies

Current validation settings include:

* **Minimum Player distance:** `8`
* **Enemy spacing:** `1`
* **Maximum candidate attempts:** `12`

Active enemy tracking is handled through `EnemyRegistry`.

## 🎮 Controls

### 📱 Mobile

Use the on-screen virtual joystick to move.

The Player automatically targets and fires at the nearest living enemy within weapon range.

### ⌨️ Unity Editor

Use:

* **WASD**
* **Arrow Keys**

to move the Player.

Attacking is automatic in both Editor and Android builds.

## 🖥️ Game Flow

```text
Main Menu
    ↓
Difficulty Selection
    ↓
Game
    ↓
Win / Lose
    ↓
Result
    ├── Replay
    └── Main Menu
```

* Survive for **3 minutes** to win.
* Reaching **0 HP** results in a loss.
* The Result screen displays the current session kill count.
* **Replay** restarts the game with the currently selected difficulty.
* **Main Menu** returns to difficulty selection.
* Total kills are persisted between sessions.

## 📊 Gameplay HUD

* **Top Left:** Player Health
* **Top Center:** Survival Timer
* **Top Right:** Session Kill Count
* **Enemies:** Individual health bars

## ⚙️ Architecture

The project uses a lightweight, component-based architecture focused on keeping systems separated without introducing unnecessary framework complexity.

Main responsibilities are separated between systems such as:

* Player input and movement
* Player combat and health
* Enemy movement
* Enemy attack
* Enemy health and lifecycle
* Enemy registry
* Wave spawning
* Difficulty configuration
* Game flow
* UI
* Enemy rendering optimization

`DifficultyConfig` ScriptableObjects separate difficulty data from spawning logic.

`EnemyRegistry` provides a lightweight active-enemy source for targeting and spawning systems without repeated scene-wide searches.

Runtime references such as the Player are injected into spawned enemies rather than repeatedly searched for during gameplay.

## 🚀 Mobile Rendering Optimization

Physical Android profiling identified **enemy body rendering** as the main performance bottleneck, especially when many `SkinnedMeshRenderer` instances were active simultaneously.

A state-first rendering approach was implemented to reduce the cost of the surrounding enemy crowd.

### Chasing

Normal chasing enemies use:

* `MeshRenderer`
* pre-baked running poses
* **16 evenly sampled run poses**
* shared meshes
* original Lit material appearance
* approximately **3.1K vertices / 3.3K triangles per pose**

The pose meshes are swapped at runtime without generating meshes or performing runtime mesh baking.

### Attacking

Enemies that are actively attacking switch back to the original:

* `SkinnedMeshRenderer`
* Animator
* full-detail supplied enemy model
* original attack animation

After the attack transition completes, the enemy returns to the lightweight baked chasing representation.

This keeps full skeletal animation where it is visually important while reducing the rendering cost of the surrounding crowd.

## 🎨 Asset Optimization

All supplied source assets are preserved separately and remain unchanged.

The optimized enemy meshes are derivative assets created specifically for the crowd-rendering path.

The original supplied enemy model is approximately:

* **19.9K vertices**
* **36.9K triangles**

The final **Mobile3K** baked crowd representation uses 16 running poses and is approximately:

* **3.1K vertices**
* **3.3K triangles per pose**

The original full-detail model remains available and is still used during important animated interactions such as enemy attacks.

This allows aggressive mobile optimization without destructively modifying the provided source assets.

## 📈 Android Performance

Performance was measured on a physical **Xiaomi Redmi Note 12** in Hard mode with approximately **40 active enemies**.

| Metric | Baseline | Final Optimized |
|---|---:|---:|
| GPU Frame Time | 61.17 ms | **34.63 ms** |
| CPU Frame Time | 61.23 ms | **34.64 ms** |
| Derived FPS | 16.33 | **28.87** |

The final version reduced GPU frame time by approximately **43%** and increased the measured frame rate by approximately **77%** compared with the original Android baseline.

The 30 FPS target was not fully reached in the profiled Development Build, but the final implementation produced a substantial and repeatable improvement while preserving the intended visual quality and gameplay behaviour.

Performance decisions were based on physical-device profiling rather than Unity Editor frame rate.

## 📱 Android Profiling Environment

Final performance verification used:

* **Device:** Xiaomi Redmi Note 12
* **Android:** 15
* **Resolution:** 2400 × 1080
* **Graphics API:** Vulkan
* **Architecture:** ARM64
* **Scripting Backend:** IL2CPP
* **Quality:** Mobile
* **VSync:** Off
* **Deep Profiling:** Off

The profiling process, controlled A/B experiments and optimization decisions are summarized in `AI_WORK_LOG.md`; the detailed development report remains private.

## 🚀 Setup

1. Clone or download the repository.
2. Open the project using **Unity 6000.3.14f1**.
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press **Play**.
5. Select **Easy**, **Medium** or **Hard**.
6. Play using the virtual joystick or Editor keyboard controls.

For Android builds, use Landscape orientation with ARM64 / IL2CPP.

## 🤖 AI & MCP Tools

AI-assisted tools were used as part of the development, debugging, profiling and verification workflow.

* **ChatGPT** — architecture, debugging, profiling analysis and technical review
* **Unity Coplay MCP** — direct Unity Editor inspection, modification and verification
* **Antigravity & Codex** — AI-assisted code implementation, refactoring and codebase analysis

The MCP workflow followed a verification-oriented cycle:

```text
Read Unity state
        ↓
Apply targeted change
        ↓
Verify in Editor
        ↓
Build / Profile where required
        ↓
Review measured result
```
