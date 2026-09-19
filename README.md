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
* **Main Scene:** `Assets/Scenes/Game.unity`

## ✨ Implemented Features

* Virtual joystick and keyboard movement
* Player rotation and camera follow
* Rifle Idle, Walk and Death animations
* Player and rifle models/materials
* Enemy movement and animations
* Player and enemy health systems
* Player HUD and enemy health bars
* Enemy attack and damage
* Automatic nearest-enemy targeting
* Automatic rifle fire
* Player and enemy death handling

## 🎮 Controls

### 📱 Mobile

Use the on-screen joystick to move.

The player automatically aims and fires when an enemy enters weapon range.

### ⌨️ Unity Editor

Use **WASD** or **Arrow Keys** to move.

## 🚀 Setup

1. Open the project with **Unity 6000.3.14f1**.
2. Open `Assets/Scenes/Game.unity`.
3. Press **Play**.

## 🤖 AI & MCP Tools

* **ChatGPT** — architecture, debugging and technical review
* **Unity Coplay MCP** — direct Unity Editor inspection, modification and verification

MCP workflows are performed using:

`Read Unity state → Apply changes → Verify result`

## 📌 Remaining Work

* Wave spawning
* Three difficulty levels
* 3-minute survival timer
* Win / Lose screens
* Kill tracking and persistent total kills
* Replay flow
* Object pooling
* Performance profiling
* Asset and runtime optimization
* Android build and device testing

## ⚠️ Known Limitations

* Rifle attacks currently use direct damage instead of physical projectiles.
* Final profiling and optimization have not been completed yet.
* Android device validation has not been completed yet.
