# 🎮 UnityMolX – Basic Controls

This document provides an overview of the **basic controls** for interacting with molecular structures in **UnityMolX VR**.  
UnityMolX uses **OpenXR** with custom scripts to enable both scientific accuracy and intuitive VR interaction.  

---

## 🖐️ Hand Interactions

### 👉 One-Hand Grab
- **Action:** Grab the molecule with one controller.  
- **Result:** Move/translate the molecule in space.  
- **Use Case:** Reposition the molecule for better viewing.  

---

### ✋ Two-Hand Grab (Rotation)
- **Action:** Grab the molecule with both controllers **without pressing triggers**.  
- **Result:** Rotate the molecule around its center.  
- **Use Case:** Orient the molecule to inspect specific chains or residues.  

---

### ✋🔫 Two-Hand Grab (Scaling)
- **Action:** Grab the molecule with both controllers **and press triggers**.  
- **Result:** Scale the molecule up or down.  
- **Use Case:** Zoom into atomic details or zoom out to see the global structure.  

---

## 🎯 Atom Selection
- **Action:** Point with your controller ray (custom **AtomRayInteractor**).  
- **Result:** Select an atom directly using **molecular raycasting** (not colliders).  
- **Visual Feedback:**  
  - Line length adjusts dynamically.  
  - Line color changes when an atom is targeted.  

---

## 🖥️ UI Interaction
- **Action:** Point at menus or UI panels.  
- **System:** The **UIRayInteractorToggle** automatically switches your ray to UI mode.  
- **Result:** Prevents conflicts between UI interaction and molecular interaction.  

---

## 🧪 Special Notes
- **Global Scale:** Scaling uses the **APIPython API** to ensure all molecular structures are updated consistently.  
- **Scientific Precision:** Atom selection relies on **UnityMol’s custom raycasting** (not physics colliders).  
- **Recommended Setup:** VR headset connected to PC (tested on Meta Quest 3 + OpenXR).  

---

## 📚 Quick Reference
| Action                          | Controllers                | Effect                  |
|---------------------------------|----------------------------|-------------------------|
| One-hand grab                   | Grab with one controller   | Move molecule           |
| Two-hand grab (no triggers)     | Both controllers           | Rotate molecule         |
| Two-hand grab (with triggers)   | Both controllers + triggers| Scale molecule          |
| Atom selection                  | Point ray at molecule      | Highlight/select atom   |
| UI interaction                  | Point ray at UI            | Switches to UI mode     |

---

✍️ *These controls form the foundation of UnityMolX VR interaction.*

**© William K. Guilon Dronnier**
