# ⚛️ Unity SDF 3D renderer | C#

This project contains the Unity-based 3D molecular renderer used in the ChemSolve mobile application.

The renderer receives molecular data in SDF (Structure Data File) format from the Android client and reconstructs the molecular structure in real time using dynamically generated geometry.

This repository focuses exclusively on:
* Molecular visualization
* Runtime structure generation
* SDF parsing and interpretation
* Atom and bond rendering

The Android application and backend inference pipeline are located in the main ChemSolve repository.

## 🎬 Demo

The renderer dynamically updates the molecular structure whenever a new SDF file is received from the mobile application.

![2026-02-1617-43-55-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/5e3e7f08-7a87-4296-8436-de510b43db74)

## ✨ Features

Runtime parsing of V3000 SDF files.

Dynamic atom and bond generation.

Support for:
  * Single bonds
  * Double bonds
  * Triple bonds

Automatic molecule clearing and rebuilding

Embedded Unity runtime inside Android application

Lightweight rendering optimized for mobile devices

## 🔑 Core Components

### generateMolecule.cs

Responsible for:
  * Reading SDF structure data
  * Instantiating atom prefab
  * Creating bond connections
  * Positioning atoms using coordinates from the SDF file

The molecule is rebuilt each time a new structure is received.

### rotation.cs

Provides automatic rotation of the molecule to improve spatial understanding during visualization.
*  Continuous rotation applied to the molecule root object
*  Adjustable rotation speed
*  Lightweight update loop to maintain performance on mobile devices

## 💻 Technologies Used

* Unity 2021 LTS
* C#
* Unity Android Player
* OpenGL ES 3
* Android UnityPlayer integration

## ⚙️ Set-up

For this to work, once you clone the repo you need to unzip the Library.zip folder into the Unity-3D-SDF folder and then open the project Unity 2021 LTS version. It may work in other versions but I have not tested this.

For use in Unity you need to click on the main camera and change the field of view to 60.

The dw is a GameObject made for the mobile application, this has the moleculeGeneration and rotate script attached to it to generate molecules from SDFs sent to mobiles.

The dw(1) is a GameObject made for testing in Unity, it generates molecules from sdf ouput in Assest/sdf, so if you want view a different molecule you need to replace the sdf file in that folder. Otherwise if you intend to use this set-up for the mobile app delete dw(1) before exporting the project.

The repo already has the settings saved to be able to export the build into a android build so nothing needs to be changed unless you need something more specific. To export the project into a android build you want to go to File > Build Settings > Export. Then once exported copy the Unity Launcher into your android app folder.

You also need to modify this line in the gradle file in the Unity Launcher:

```Bash
    aaptOptions {
        // Unity streaming assets must not be compressed
        noCompress = ['.unity3d', '.ress', '.resource', '.obb', '.bundle', '.bytes', '.dat', '.mp4', '.wav', '.ogg']
    }
```




