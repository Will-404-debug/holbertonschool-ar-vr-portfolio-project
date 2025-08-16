# 📦 Protein Data Bank (PDB) in UnityMolX

## 🌐 What is the PDB?
The **Protein Data Bank (PDB)** is a **global repository** for 3D structural data of biological macromolecules such as proteins, DNA, and RNA.  
It is maintained by the **Worldwide Protein Data Bank (wwPDB)** consortium, which ensures that structural biology data is freely available to scientists worldwide.  

👉 Website: [https://www.rcsb.org/](https://www.rcsb.org/)

---

## 🧩 How UnityMolX Uses the PDB
UnityMolX is built on top of **UnityMol**, which supports importing structures directly from the PDB.  
This allows researchers to **visualize, explore, and manipulate molecular structures** in VR without needing to create their own datasets.  

- Molecules are loaded via their **PDB ID** (e.g., `1BTA`, `1CRN`, `2HHB`).  
- UnityMol automatically fetches the corresponding 3D data from the PDB servers.  
- Structures can then be represented in VR as atoms, bonds, ribbons, or surfaces.  
- With UnityMolX, you can **interact with these molecules in VR** using OpenXR — selecting atoms, scaling, rotating, and exploring them intuitively.  

---

## 🔬 Why the PDB Matters
- It provides **validated, peer-reviewed structural data** used worldwide in biology, chemistry, and medicine.  
- Ensures that UnityMolX can always access **up-to-date molecular structures**.  
- Makes VR molecular visualization not only educational but also **scientifically relevant** for research.  

---

## ⚙️ Example Usage in UnityMolX 
1. Open UnityMolX VR.  
2. Load a structure by entering its **PDB ID** and then click **Fetch**. (Not functional in the UnityMolX-Windows first version, need to do some tweaks in the APIPython.)
3. Explore the molecule in VR:  
   - Grab with one hand to move it.  
   - Use two hands to scale and rotate.  
   - Select atoms via the custom molecular raycast system.  

---

## 📚 References
- [RCSB Protein Data Bank](https://www.rcsb.org/)
- [Worldwide Protein Data Bank](https://www.wwpdb.org/)

---

✍️ *This project integrates the global PDB database into VR workflows, bringing molecular research closer to immersive technologies.*

**© William K. Guilon Dronnier**
 