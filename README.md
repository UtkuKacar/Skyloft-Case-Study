# 🎮 Skyloft Studios - Unity Developer Case Study

> Survivor.io-style mobile game prototype developed in **Unity 6** with **URP**, targeting **Android**.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.14f1-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP-green.svg)](https://unity.com/srp/Universal-Render-Pipeline)
[![Platform](https://img.shields.io/badge/Platform-Android-orange.svg)](https://www.android.com/)

## 🛠️ Project

* **Unity:** 6000.3.14f1
* **Render Pipeline:** URP
* **Platform:** Android / Landscape
* **Input:** Unity New Input System
* **Packages:** Universal Render Pipeline, Input System
* **Scenes:** `MainMenu.unity`, `Game.unity`, `Result.unity`

## ✨ Implemented Features

* Virtual joystick and keyboard movement
* Player rotation and camera follow
* Rifle Idle, Walk and Death animations
* Player and enemy health systems
* Player HUD and enemy health bars
* Automatic nearest-enemy targeting and rifle fire
* Enemy pursuit, attack and death handling
* Animation-synchronized melee damage
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

## 🗺️ Arena

The game uses a lightweight **50 × 50** placeholder arena built with Unity primitive objects.

The layout includes an open central combat area, wide movement lanes and simple obstacles.

Players and enemies collide with the environment, while Player ↔ Enemy physical blocking remains disabled.

Enemies use lightweight obstacle avoidance to navigate around arena structures.

## 🎯 Difficulty

All difficulty levels use the same `Game.unity` scene and are configured through `DifficultyConfig`.

* **Easy:** 4s spawn interval / 2 enemies per wave / max 12 / enemy speed 4.5
* **Medium:** 2.5s spawn interval / 3 enemies per wave / max 24 / enemy speed 5
* **Hard:** 1.5s spawn interval / 4 enemies per wave / max 40 / enemy speed 5.5

Player movement speed is **5.5**.

## ⚔️ Combat

The player automatically targets and fires at the nearest living enemy within weapon range.

Enemies pursue the Player and use animation-synchronized melee attacks.

The attack behaviour is designed to remain threatening while preventing enemies from physically sticking to or trapping the Player.

## 🌊 Spawning

Enemies are spawned through `WaveSpawner` using the selected `DifficultyConfig`.

Spawn positions are validated before instantiation to prevent enemies from spawning inside arena structures, outside playable bounds or too close to the Player.

Current spawn settings include:

* minimum Player distance: `8`
* enemy spacing: `1`
* maximum candidate attempts: `12`

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
* **Replay** restarts the game with the selected difficulty.
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

MCP workflow:

`Read Unity state → Apply changes → Verify result`
