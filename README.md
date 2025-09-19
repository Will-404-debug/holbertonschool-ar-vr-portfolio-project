# 🌐 UnityMolX VR Portfolio Project

## 📖 Project Overview
This is my Holberton School AR/VR Portfolio Project, built using **Unity**, the **XR Interaction Toolkit**, and custom C# scripts.  
The project extends the **UnityMol** molecular visualization software with immersive VR interactions through **OpenXR** integration, focusing on **atom selection, UI integration, and two-hand manipulation**.

⚡ The particularity of **UnityMolX** is that it does **not** use the regular Unity collider-based raycast system.  
Instead, it relies on a **custom molecular raycasting algorithm** that directly queries atomic data for more accurate and efficient interaction.

---

## 🛠️ Technologies Used
- Unity 2021.3.45f1
- XR Interaction Toolkit (XRI) 3.2.0  
- OpenXR  
- Unity Input System  
- C#  
- UnityMol API (APIPython integration)  

---

## 📋 Requirements
- A **VR headset connected to your PC** (tested with Meta Quest 3 + OpenXR).  
- A PC capable of running Unity VR builds → **minimum**: GPU such as an NVIDIA GTX 1060 or AMD Radeon RX 480 or better, a CPU like an Intel i5-4590 or AMD Ryzen 5 1500X or greater, at least 8GB of RAM, a compatible HDMI 1.3 video output, and at least one USB 3.0 port.
- Please read the **`Docs/`** folder for usage instructions, since UnityMolX is **scientifically centered**.

---

## 📂 Repository Structure
```plaintext
holbertonschool-ar-vr-portfolio-project/
│
├── Builds/                   # Unity build (Windows (for now))
│
├── VR_XRI_OpenXR_minimal/    # Folder to request view for the code
│
├── Docs/                     # Optional docs and images
│
└── README.md
```

---

## ✨ Custom Scripts

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

### `ControllerHelpToggleXR.cs`
- Manages **Controller Help UI** in VR.
- Allows toggling an in-VR guide that shows controller button bindings.
- Uses input actions to display contextual help panels.

### `QuitButton.cs`
- Provides a safe **quit button** inside VR.
- Lets the user exit UnityMolX directly from the in-game UI without removing the headset.

### `StartupHint.cs`
- Displays a **popup hint** when the application starts.
- Reminds the user to press the menu button on the left controller to toggle controller help.
- Always follows the main camera for visibility.

### `StickyHintFollowHead.cs`
- Makes UI elements (like Controller Help panels) **follow the player’s head movement**.
- Ensures hints stay visible in the VR field of view.

---

## ▶️ How to Run the Build
1. Clone the repository:  
   ```bash
   git clone https://github.com/Will-404-debug/holbertonschool-ar-vr-portfolio-project.git
   ```
2. Navigate to the `Builds/` folder.  
3. Click on UnityMolX-*Your Operating System*.txt, then click on the link download the .zip, unzip the folder and run the executable.  

---

## 📓 Devlogs
I documented my development progress in LinkedIn posts:  
👉 [Read my Devlog #0](https://www.linkedin.com/pulse/unitymolx-vr-devlog-0-first-steps-scientific-william-guilon-dronnier-rh7ue/?trackingId=NOumVZNI%2BGdLEe6Zh5lsfw%3D%3D)

👉 [Read my Devlog #1](https://www.linkedin.com/pulse/unitymolx-vr-devlog-1-william-guilon-dronnier-gxgle/)
PS: more to come (be patient)

---

## 👤 Author
**William K. Guilon Dronnier**
Holberton School – AR/VR Specialization

