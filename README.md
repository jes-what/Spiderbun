# Spiderbun

A predator-prey ecosystem simulation built in Unity, extending a prior FSM-based simulation with new agents, a custom 3D environment, a dual resource system, and expanded agent behaviors.

## Overview

The simulation populates a custom 3D environment with two types of reactive agents, rabbits (prey) and black widow spiders (predators), that move through the world, manage survival needs, interact with each other, and respond dynamically to their environment without any direct player input.

## Features

### Agents
- **Prey (Rabbits)**: wander, flee from predators, seek food and water when resources run low
- **Predators (Spiders)**: wander, hunt prey when hungry, seek water when thirsty
- Both agents have animated idle, movement, and death states driven by Unity's Animator

### FSM States
- **WanderState**: random movement with wall avoidance
- **ChaseState**: predator steers toward detected prey using cone-based vision and raycasting
- **FleeState**: prey steers away from detected predators with wall-sliding behavior
- **HungerState** (prey only): navigates to nearest active food source via SimulationManager
- **ThirstState** (both): navigates to nearest water source via SimulationManager

### Resource System
- Energy and hydration drain over time at agent-specific rates
- Critical and low thresholds trigger state transitions
- Agents die if either resource reaches zero, triggering a death animation and delayed respawn

### Environment
- Custom 3D scene with tree, rock, and terrain prefabs from the Flat Cube Environment asset
- 14 depletable fruit bush food sources (FoodPlant) that visually swap to an empty state after 3 feedings and respawn after 10 seconds
- Two lake groups built from primitive cylinders serve as water sources (WaterSource)
- Epic BlueSunset skybox

### SimulationManager
- Handles agent instantiation, death, and respawn with randomized delay and position offset
- Provides GetNearestFood and GetNearestWater to Hunger and Thirst states

### UI
- World-space AgentStatusBar on each agent displaying energy (green) and hydration (blue) bars
- Bars always face the active camera regardless of camera switching or rotation

### Camera System
- 8 cameras placed throughout the scene
- Switch cameras with left/right arrow keys or D-pad
- Rotate with mouse or right stick, zoom with scroll wheel or left stick
- Managed by CameraSwitcher, CameraHandler, and InputManager

## Controls
| Input | Action |
|---|---|
| Left/Right Arrow or D-pad | Switch camera |
| Mouse move or Right stick | Rotate camera |
| Scroll wheel or Left stick up/down | Zoom in/out |

## Notes
- Third-party assets (rabbit, spider prefabs, environment kit, skybox) are included but not original work
- ProjectSettings are not exported with Unity packages; a LayerSetup editor script handles layer initialization on load
