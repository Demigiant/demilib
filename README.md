# demilib
A library of various Editor Utilities for Unity (alpha).
Developed by Daniele Giardini - http://www.demigiant.com

As of now it includes:
#CORE
- DemiEditor: a library with various UnityEditor utils, plus GUI methods to draw nicer Editor Inspectors/Panels
- DemiLib: integrates with DemiEditor to allow customization of colors. Uses custom Skin and Color structs that return the correct value depending on Unity's skin (normal or dark)

#EXTRA
Extra Libraries are independent from each other, but may require the core libs (DemiLib and DemiEditor).
- Debugging: a barebone debug library
- Audio/AudioEditor (Core required): an audio system