<p align="center">
  <img width="300" src="Assets/BuildProfileAssets/defaultIcon1024.png">
</p>

# Jump VS 

## Purpose 
This game started as a side project, but the goals have evolved to be: 
- Add sprites with compelling designs and colors 
- Implement an online multiplayer system 
- Create a matchmaking system 

## Getting Started 
Run this command to clone the repository:

```shell
git clone https://github.com/SunveerIV/Jump-VS.git
```

Choose an IDE: 
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [JetBrains Rider](https://www.jetbrains.com/rider/) is recommended 
- [Visual Studio Code](https://code.visualstudio.com/download) works as a lightweight option 

Download Unity Tools: 
- Install [Unity Hub](https://unity.com/download) 
- Editor version [6000.0.58f2](https://unity.com/releases/editor/whats-new/6000.0.58f2) 

## Platforms 
- iOS is the primary target platform. 
- Android support is planned for the future.

## Security 
The following information has been excluded from the repository for security reasons: 
- Unity Cloud Project ID 
- Unity Organization ID 

## Building
Follow these steps in the Unity Editor to generate a functional build for your desired platform: 
- Go to **Edit -> Project Settings -> Services**, and assign an Organization and cloud project. 
- Go to **Assets/Prefabs/Managers**, select the **NetworkManager** prefab, and set the **Protocol Type** to **Relay Unity Transport** in the **Unity Transport** Component. 
- Go to **File -> Build Profiles** and build the project for your desired platform.
