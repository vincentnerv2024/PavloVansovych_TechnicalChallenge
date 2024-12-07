# **XR Hybrid Gameplay Prototype**

## **Overview**
This project is a hybrid prototype or simulation of XR gameplay. Due to the lack of VR hardware like Oculus and the time-consuming nature of the XR Device Simulator, this project simulates XR-style interactions and mechanics using a traditional FPS setup.

The goal of the prototype is to showcase core gameplay elements such as object interaction, enemy combat, and UI interaction, resembling what might be found in VR games.

---

## **Features**

### **Core Mechanics**
#### **Shield Interaction**
- **Pick Up:** Press **RMB** to pick up the shield.
- **Block:** Hold **RMB** to block enemy attacks (when the shield is equipped).
- **Drop:** Press **T** to drop the shield.

#### **Force Sphere Interaction**
- **Pick Up:** Use **LMB** to pick up the Force Sphere.
- **Throw:** Release **LMB** to throw the sphere at enemies.

#### **World Canvas UI Interaction**
- Interact with UI elements (e.g., buttons, scanners) using **LMB**, simulating XR-style interaction.

---

### **Enemies**
The game features three types of enemies with unique abilities and behaviors:
1. **Enforcer:** Fires lasers from a blaster.
2. **Overseer:** Shoots a long, continuous laser from its eye.
3. **Drone:** A conceptual enemy that serves as a placeholder.

Each enemy has varying speed, reaction time, and behavior to challenge the player.

---

### **Environment Interaction**
- **Hand Scanner:** Players can interact with a door scanner using the UI interaction mechanic, simulating XR-style interaction.

---

### **Gameplay Notes**
- **Game Over:** If the player's health reaches zero, the scene reloads automatically.
- **Win Condition:** Unfortunately, the win condition didn't trigger correctly in this version, but the gameplay remains functional.

---

## **Tools and Plugins Used**
- **FEEL:** For adding immersive feedback and visual effects.
- **DOTween:** For smooth animations, object movement, and UI transitions.
- **Synty Assets:** For 3D models, animations, and level design concepts.
- **Custom Scripts:** All core scripts, including enemy AI, interaction mechanics, and UI functionality, are custom-made for this prototype.

---

## **Known Bugs and Limitations**
1. **Game Over Mechanic:** Works correctly but lacks proper win condition handling.
2. **Polish:** Due to time constraints, some mechanics lack full refinement or optimization.
3. **XR Device Simulator:** Initially planned for development, but issues with functionality led to creating a simulation of XR gameplay instead.

---

## **Development Notes**
This project was created as part of a **Technical Interview Challenge** for **NEX Level Gaming** under a tight deadline. While some features are incomplete or buggy, the prototype demonstrates core concepts of XR-style gameplay using traditional FPS controls.

### **Key Notes**
- The level design and script implementation were done entirely by me.
- I relied on a mix of third-party assets and custom code to meet the tight timeline.
- While not entirely **SOLID**, the scripts are structured enough to showcase the intended gameplay mechanics.

---

## **How to Play**

### **Run the Game**
1. Open the game and start playing directly.

### **Controls**
- **WASD:** Move
- **Shift:** Sprint
- **Space:** Jump
- **RMB:** Pick up/drop shield and block attacks
- **LMB:** Pick up/throw the Force Sphere and interact with UI
- **T:** Drop the shield

### **Gameplay Loop**
- Survive enemy attacks using the shield.
- Use the Force Sphere to eliminate enemies.
- Interact with the environment to progress.

---

## **Conclusion**
Thank you for this challenging and interesting task! While it’s not perfect, I hope this prototype provides a glimpse of my ability to adapt quickly, develop custom mechanics, and simulate gameplay under tight deadlines.

I look forward to discussing this further during the interview.
