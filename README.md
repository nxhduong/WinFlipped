![App icon](./Resources/Images/AppIcon.png)
# WinFlipped
![](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/nxhduong/WinFlipped)
![GitHub contributors](https://img.shields.io/github/contributors/nxhduong/WinFlipped)
![GitHub Release](https://img.shields.io/github/v/release/nxhduong/WinFlipped)

WinFlipped is an attempt to mimic the functionality of Flip3D (available on Windows Vista/7) on Windows 10/11.

It works by screen-grabbing all available windows (meaning that there are no live window thumbnails, unfortunately). For security and privacy reasons, screenshots are not saved anywhere. 

This is not a perfect Flip3D replacement, but as far as I know, there is no better way, since Windows does not provide a good API for this (If you know one, please tell me).

This project is currently a WIP.
# What is Flip3D?
According to [www.pcmag.com](https://www.pcmag.com/encyclopedia/term/flip-3d):

Flip3D is *"a feature of the earlier Aero interface in Windows that displayed the desktop and open apps in 3D. 
Pressing Windows key + Tab key invoked Flip 3D, and continually pressing Windows-Tab rotated the windows from back to front."*

![Flip3D](./Resources/Images/Flip3D.jpg)
# Requirements
- Windows 10/11
# Usage
- Precompiled binaries, if available, can be found on the `Releases` tab
- You can assign a key combination to launch this program, using tools like *AutoHotKey*.
- Press `TAB` to cycle through windows, and `ENTER` to switch to the selected one.
# Notes
- This program is NOT TESTED on computers with multiple monitors and may not work properly
- This program is also NOT TESTED with multiple desktops
# To-dos:
- Finish animation
- Improve performance if able
- Find new ways of getting window thumbnails, as some programs use GPU rendering, meaning that they will show up blank.
# Contributing to this project
Suggestions are welcomed.
# License
[MIT](./LICENSE)
