# VR_Gunjack

A VR first-person shooter prototype built in Unity to explore VR interaction, weapon handling, enemy AI, and wave-based combat.

## Overview

VR_Gunjack is a Unity-based VR demo game where the player uses XR interactions to grab weapons, fire bullets, throw grenades, manage ammo, survive enemy waves, and reach a win condition by earning enough score. The project appears to be built around Unity XR Interaction Toolkit, NavMesh-based enemy navigation, and world-space VR UI.

## Features

- **VR weapon handling**
  - Grab and use firearms with XR Interaction Toolkit
  - Fire projectiles from a weapon-mounted spawn point
  - Play muzzle flash and gunshot audio on fire
  - Display ammo count in VR UI

- **Grenade gameplay**
  - Throw grenades using XR grab interactions
  - Fuse timer after release
  - Explosion effects, sound, physics impulse, and area damage

- **Enemy AI**
  - NavMeshAgent-based enemy movement
  - Detection, walking, running, and attack ranges
  - Enemy damage and death handling
  - Auto-detection of the XR player rig

- **Wave-based combat**
  - Periodic enemy waves
  - Increasing difficulty over time
  - Spawn points defined as child objects of the wave manager

- **Player health and game over**
  - Player health bar and health text
  - Hurt sounds
  - Game over screen when health reaches zero
  - Player interaction restricted to UI after death

- **Scoring and win state**
  - Score increases when enemies are killed
  - Win condition triggered at a target score
  - Win screen displayed in front of the player
  - World lighting fades during the win transition

- **VR menus and UI**
  - In-game menu that appears in front of the player
  - World-space UI positioning for readability in VR

## Project Structure

The main gameplay scripts are located in:

```text
VR Room/Assets/Scripts/
