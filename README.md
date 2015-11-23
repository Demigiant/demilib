# demilib
Various utility libraries for Unity (alpha).
Developed by Daniele Giardini - http://www.demigiant.com

As of now it includes these libraries:

##Core libraries (Unity 4.6 or later)
- **DemiEditor**: a library with various UnityEditor utils, plus GUI methods to draw nicer Editor Inspectors/Panels
- **DemiLib**: integrates with DemiEditor to allow customization of colors. Uses custom Skin and Color structs that return the correct value depending on Unity's skin (normal or dark)

##Extra libraries
Extra Libraries are independent from each other, but require the Core libraries (DemiLib and DemiEditor).
- **Debugging** (Unity 4.6 or later): a barebone debug library
- **DeAudio/DeAudioEditor** (Unity 5 or later - requires DOTween): audio manager