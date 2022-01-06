# VR-3D-Annotation-Tool

> This is the project for Mixed Reality Lab @ ETH Zurich 2021, aiming to develop a 3D labelling tool supprted by VR device. 

Advances in technology have made possible the acquisition of 3D datasets of indoor
spaces for the general public. Nevertheless, these acquired data still needs to be labelled (e.g annotated) to be used
as training datasets. Therefore, the aim of our application is to facilitate this last step for the public through our
VR (Virtual Realiy) application. Two labeling modes are provided in our application:
1. User-Dyanmic mode: <br/>
  user can walk into the scene of interest immersively
3. User-Static mode:  <br/> 
  user can label objects while sitting comfortably on a chair.

# Demo
> insert video later

# Requirement 
Hardwares:
- HTC Vive Pro

Softwares: 
- Blender version 2.79b 
- Unity 2018.4(LTS)
- Steam 
- SteamVR

Unity plugins: 
- OpenVr, Window->Package Manager, select “OpenVR” in the package list and click install. Restart Unity

# Data
2D3Ds Dataset area 3.

Google Drive: 
- Dowload from this link the 3D model of the room: https://drive.google.com/drive/folders/17LrUZA8PF2EZjz1r8iOgAt0GAO_l_rrN?usp=sharing
- Put the folder **Meshes** and the file **Meshes.meta** in the folder VR-3D-Annotation-Tool\unity\Assets\Resources\Models
- Put the folders **RGBMeshes** and **EmptyMeshes** with the files **RGBMEshes.meta** and **EmptyMeshes.meta** into the folder VR-3D-Annotation-Tool\unity\Assets\Resources\Prefabs
- Open unity and the scene TestRoom2d3ds
- Click on the GameObject Meshe and update Meshe RGB and Meshe Empty accordingly with the Prefab in VR-3D-Annotation-Tool\unity\Assets\Resources\Prefabs\RGBMeshes and VR-3D-Annotation-Tool\unity\Assets\Resources\Prefabs\EmptyMeshes  

# 2 Labeling Modes

## User-Dynamic Labeling
-  Controller guidance

Left controller            |          Right controller   
:-------------------------:|:-------------------------:
<img src="https://github.com/SherryJYC/VR-3D-Annotation-Tool/blob/main/misc/controller_guide-left.png" alt="drawing" height="300" width="550"/>  | <img src="https://github.com/SherryJYC/VR-3D-Annotation-Tool/blob/main/misc/controller_guide-right.png" alt="drawing" height="300" width="550"/> 

## User-Static Labeling 
-  Controller guidance

Left controller            |          Right controller   
:-------------------------:|:-------------------------:
<img src="https://github.com/SherryJYC/VR-3D-Annotation-Tool/blob/main/misc/controller_guide-static_left.png" alt="drawing" height="300" width="550"/>  | <img src="https://github.com/SherryJYC/VR-3D-Annotation-Tool/blob/main/misc/controller_guide-static_right.png" alt="drawing" height="300" width="550"/> 

> for a user study to compare these two versions, please check out [link to user study report (NOT YET)]()


## Acknowledgement
This [git repository](https://github.com/pierlui92/Shooting-Labels) was used as the ground foundation of our project

## Authours
Cyprien Fol, Yuchang Jiang, Tianyu Wu, Yiming Zhao



