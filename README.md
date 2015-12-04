# demilib
Various utility libraries for Unity (alpha).  
Developed by Daniele Giardini - http://www.demigiant.com

You can get it all, or only single configurations, from the downloads folder.  
**NOTE:** If you have *DOTween* in your project, and you're importing a configuration that requires DemiLib core, first delete (not simply overwrite) the DemiLib folder that DOTween already put in your project, since it's an old version with a different directory layout.

##Core libraries (Unity 4.6 or later)
####DemiEditor
A library with various UnityEditor utils, plus GUI methods to draw nicer Editor Inspectors/Panels
####DemiLib
Integrates with DemiEditor to allow customization of colors. Uses custom Skin and Color structs that return the correct value depending on Unity's skin (normal or dark)

##Extra libraries
Extra Libraries are independent from each other, but may require the Core libraries (DemiLib and DemiEditor).
###DeAudio/DeAudioEditor
![DeAudioManager](screenshots/DeAudioManager_inspector.png "DeAudioManager")  
*Unity 5 or later - requires Core libraries and DOTween*  
Audio manager.
###Debugging
*Unity 4.6 or later - requires Core libraries*  
A barebone debug library.
###DeEditorTools
*Unity 5 or later - requires Core libraries*  
Miscellaneous editor panels.
#####DeEditorTools > Scenes Panel
![Scenes Panel](screenshots/DeEditorTools_scenespanel.png "Scenes Panel")  
*Open from Tools > Demigiant > Scenes Panel*  
A dockable panel with all scenes in the build. You can click them directly to switch to the given scene, or use a toggle to enable/disable each.
###DeExtensions
*Unity 5 or later*  
Miscellaneous runtime extensions.
###DeUtils
*Unity 5 or later*  
Miscellaneous runtime utilities.