# 🎮 Skyloft Studios - Unity Developer Case Study

> Survivor.io-style mobile game prototype developed in **Unity 6** with **URP**, targeting **Android**.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.14f1-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP-green.svg)](https://unity.com/srp/Universal-Render-Pipeline)
[![Platform](https://img.shields.io/badge/Platform-Android-orange.svg)](https://www.android.com/)

## 🛠️ Project

* **Unity:** 6000.3.14f1
* **Render Pipeline:** URP
* **Platform:** Android / Portrait
* **Input:** Unity New Input System
* **Scenes:** `MainMenu.unity`, `Game.unity`, `Result.unity`

## ✨ Implemented Features

* Virtual joystick and keyboard movement
* Player rotation and camera follow
* Rifle Idle, Walk and Death animations
* Player and enemy health systems
* Player HUD and enemy health bars
* Automatic nearest-enemy targeting and rifle fire
* Enemy pursuit, attack and death handling
* Wave-based enemy spawning
* Easy / Medium / Hard difficulty selection
* Configurable enemy count and spawn frequency
* 3-minute survival timer
* Session kill counter
* Persistent total kill count
* Win / Lose game flow
* Result screen with kill statistics
* Replay and Main Menu options
* Fade transitions between scenes
* Player and enemy death handling

## 🎯 Difficulty

The game uses the same gameplay scene for all difficulty levels.

Difficulty is configured through `DifficultyConfig` assets and applied by the `WaveSpawner`.

* **Easy:** 4s spawn interval / 2 enemies per wave / max 12
* **Medium:** 2.5s spawn interval / 3 enemies per wave / max 24
* **Hard:** 1.5s spawn interval / 4 enemies per wave / max 40

## 🎮 Controls

### 📱 Mobile

Use the on-screen joystick to move.

The player automatically targets and fires at the nearest living enemy within weapon range.

### ⌨️ Unity Editor

Use **WASD** or **Arrow Keys** to move.

## 🖥️ Game Flow

`Main Menu → Difficulty Selection → Game → Win / Lose → Result`

* Survive for **3 minutes** to win.
* Reaching **0 HP** results in a loss.
* **Replay** restarts with the selected difficulty.
* **Main Menu** allows selecting a different difficulty.

## 📊 Gameplay HUD

* **Top Left:** Player Health
* **Top Center:** Survival Timer
* **Top Right:** Session Kill Count

## 🚀 Setup

1. Open the project with **Unity 6000.3.14f1**.
2. Open `Assets/Scenes/MainMenu.unity`.
3. Press **Play**.
4. Select **Easy**, **Medium** or **Hard**.

## 🤖 AI & MCP Tools

* **ChatGPT** — architecture, debugging and technical review
* **Unity Coplay MCP** — direct Unity Editor inspection, modification and verification

MCP workflows follow:

`Read Unity state → Apply changes → Verify result`

AI-assisted engineering decisions are documented separately in `AI_WORK_LOG.md`.

## 📌 Remaining Work

* Baseline performance profiling
* Object pooling
* Asset optimization
* Runtime optimization
* Baseline vs optimized performance comparison
* Android build and device testing
* Final APK
* Final demo video

## ⚠️ Known Limitations

* Rifle attacks currently use direct damage instead of physical projectiles.
* Final profiling and optimization have not been completed yet.
* Android device validation has not been completed yet.
