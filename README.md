# 🌐 UnityMolX VR Portfolio Project

## 📖 Project Overview
This is my Holberton School AR/VR Portfolio Project, built using **Unity**, the **XR Interaction Toolkit**, and custom C# scripts.  
The project extends the **UnityMol** molecular visualization software with immersive VR interactions through **OpenXR** integration, focusing on **atom selection, UI integration, and two-hand manipulation**.

⚡ The particularity of **UnityMolX** is that it does **not** use the regular Unity collider-based raycast system.  
Instead, it relies on a **custom molecular raycasting algorithm** that directly queries atomic data for more accurate and efficient interaction.

---

## 🛠️ Technologies Used
- Unity  
- XR Interaction Toolkit (XRI) 3.2.0  
- OpenXR  
- Unity Input System  
- C#  
- UnityMol API (APIPython integration)  

---

## 📋 Requirements
- A **VR headset connected to your PC** (tested with Meta Quest 3 + OpenXR).  
- A PC capable of running Unity VR builds.  
- Please read the **`Docs/`** folder for usage instructions, since UnityMolX is **scientifically centered**.

---

## 📂 Repository Structure
```plaintext
holbertonschool-ar-vr-portfolio-project/
│
├── Builds/                # Unity build (Windows (for now))
│
├── Scripts/               # Custom scripts I created to integrate OpenXR
│   ├── AtomLineVisualOverride.cs
│   ├── AtomRayInteractor.cs
│   ├── TwoHandGrabScaler.cs
│   ├── UIRayInteractorToggle.cs
│
├── Docs/                  # Optional docs and images
│
└── README.md
```

---

## ✨ Custom Scripts

### `AtomLineVisualOverride.cs`
- Customizes the XR line visual.  
- Dynamically adjusts **line length** and **color gradient** depending on selection state.  
- Provides visual feedback when targeting atoms.

### `AtomRayInteractor.cs`
- Extended `XRRayInteractor` that implements **custom atom raycasting** logic.  
- Does **not** rely on Unity’s default **collider-based raycast**.  
- Instead, it integrates with **UnityMol’s custom raycast system**, which calculates hits directly on molecular data.  
- Tracks hit positions (`CurrentHitPosition`, `SelectionHitPosition`).  
- Ensures only valid molecule atoms can be hovered/selected.

### `TwoHandGrabScaler.cs`
- Implements **two-hand grab interactions** for molecular models.  
- Supports **rotation** when no triggers are pressed, and **scaling** when triggers are pressed.  
- Integrates with **APIPython** for global molecule scaling.  
- Falls back to direct local scaling if APIPython is disabled.  
- Handles input via Unity’s **Input System** with fallback to XR device triggers.

### `UIRayInteractorToggle.cs`
- Toggles between **UI ray interaction** and **Atom ray interaction**.  
- Uses `GraphicRaycaster` to detect if the ray is hovering over UI elements.  
- Prevents conflicts between UI input and molecular ray interaction.  

---

## ▶️ How to Run the Build
1. Clone the repository:  
   ```bash
   git clone https://github.com/Will-404-debug/holbertonschool-ar-vr-portfolio-project.git
   ```
2. Navigate to the `Builds/` folder.  
3. Unzip the folder and  run the executable for your platform.  

---

## 📓 Devlogs
I documented my development progress in a LinkedIn post:  
👉 [Read my Devlog #0](https://www.linkedin.com/pulse/unitymolx-vr-devlog-0-first-steps-scientific-william-guilon-dronnier-rh7ue)
PS: more to come (be patient)

---

## 👤 Author
**William K. Guilon Dronnier** 
Holberton School – AR/VR Specialization
