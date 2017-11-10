##ECS193: Senior Design @UCDavis Project Repository

1. File Hierarchy:
subsystem: contains source codes and relevent files for embedded systems
ARVA: Unity3D program workspace

2. Serial Port Connection Configuration in Unity3D:
Before compiling, make sure you go to Edit -> Project Settings -> Other Settings -> API Compatibility level: .NET 2.0. Please also change the COM port setting in CameraControl script to match your machine.

3. Unity3D setup for GitHub Version Control Collaboration:
Please go to Edit -> Project Settings -> Editor. From the “Version Control” drop down box, choose “Visible Meta Files”. From the “Asset Serialization” drop down, choose “Force Text”. Please refer to: http://www.studica.com/blog/how-to-setup-github-with-unity-step-by-step-instructions

4. Program Control: Please don't use touch pad mouse key for control. The interrupt is not handle in timely manner somehow. Please test the program with an external mouse.