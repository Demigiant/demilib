:: Called by DeEditorTools (last in compilation order) post build

:: %StartupDir = $(SolutionDir)
set PATH=%PATH%;C:\Program Files\7-Zip\
set StartupDir=%~dp0
set DownloadsDir=%StartupDir%..\downloads
set DemigiantSubdir=Demigiant\DemiLib

:: Wait for other builds to complete
timeout /t 1

:: Complete DemiLib
set TargetDir=%DownloadsDir%\Complete\%DemigiantSubdir%
echo f | xcopy "%StartupDir%\bin" %TargetDir% /Y /I /S

:: De2D + Core
set TargetDir="%DownloadsDir%\De2D_complete\%DemigiantSubdir%"
echo f | xcopy %StartupDir%\bin\Core %TargetDir%\Core /Y /I /S
echo f | xcopy %StartupDir%\bin\De2D*.* %TargetDir% /Y /I /S

:: DeAudio + Core
set TargetDir="%DownloadsDir%\DeAudio_complete\%DemigiantSubdir%"
echo f | xcopy %StartupDir%\bin\Core %TargetDir%\Core /Y /I /S
echo f | xcopy %StartupDir%\bin\DeAudio*.* %TargetDir% /Y /I /S

:: DeEditorTools + Core
set TargetDir=%DownloadsDir%\DeEditorTools_complete\%DemigiantSubdir%
echo f | xcopy %StartupDir%\bin\Core %TargetDir%\Core /Y /I /S
echo f | xcopy %StartupDir%\bin\Editor\DeEditorTools.* %TargetDir%\Editor /Y /I /S

:: DeExtensions (no core)
set TargetDir=%DownloadsDir%\DeExtensions_complete\%DemigiantSubdir%
echo f | xcopy %StartupDir%\bin\DeExtensions.* %TargetDir% /Y /I /S

:: DeUtils (no core)
set TargetDir=%DownloadsDir%\DeUtils_complete\%DemigiantSubdir%
echo f | xcopy %StartupDir%\bin\DeUtils.* %TargetDir% /Y /I /S

:: Delete any leftover tmp or pdb files
start /wait cmd /c del /S %DownloadsDir%\*.tmp
start /wait cmd /c del /S %DownloadsDir%\*.pdb

:: ZIP and DELETE

start /wait cmd /c del %DownloadsDir%\Complete.zip
start /wait cmd /c 7z a %DownloadsDir%\Complete.zip -r %DownloadsDir%\Complete\Demigiant\
rmdir /S /Q %DownloadsDir%\Complete

start /wait cmd /c del %DownloadsDir%\De2D_complete.zip
start /wait cmd /c 7z a "%DownloadsDir%\De2D_complete.zip" -r %DownloadsDir%\De2D_complete\Demigiant\
rmdir /S /Q %DownloadsDir%\De2D_complete

start /wait cmd /c del %DownloadsDir%\DeAudio_complete.zip
start /wait cmd /c 7z a "%DownloadsDir%\DeAudio_complete (requires DOTween).zip" -r %DownloadsDir%\DeAudio_complete\Demigiant\
rmdir /S /Q %DownloadsDir%\DeAudio_complete

start /wait cmd /c del %DownloadsDir%\DeEditorTools_complete.zip
start /wait cmd /c 7z a %DownloadsDir%\DeEditorTools_complete.zip -r %DownloadsDir%\DeEditorTools_complete\Demigiant\
rmdir /S /Q %DownloadsDir%\DeEditorTools_complete

start /wait cmd /c del %DownloadsDir%\DeExtensions_complete.zip
start /wait cmd /c 7z a %DownloadsDir%\DeExtensions_complete.zip -r %DownloadsDir%\DeExtensions_complete\Demigiant\
rmdir /S /Q %DownloadsDir%\DeExtensions_complete

start /wait cmd /c del %DownloadsDir%\DeUtils_complete.zip
start /wait cmd /c 7z a %DownloadsDir%\DeUtils_complete.zip -r %DownloadsDir%\DeUtils_complete\Demigiant\
rmdir /S /Q %DownloadsDir%\DeUtils_complete

EXIT