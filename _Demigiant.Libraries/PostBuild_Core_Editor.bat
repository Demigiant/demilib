:: %1-2-3-4 = $(SolutionDir) $(TargetDir) $(TargetFileName) $(TargetName)

set BinDir=bin.Global\DemiLib\Core\Editor
set DestinationDir=%1..\..\%BinDir%
set BinDirNoMeta=bin.Global_no_meta\DemiLib\Core\Editor
set DestinationDirNoMeta=%1..\..\%BinDirNoMeta%
set BinDirUnityTests=DemiLib.UnityTests.Unity5\Assets\Demigiant\DemiLib\Core\Editor
set DestinationDirUnityTests=%1..\%BinDirUnityTests%

echo %DestinationDir%
echo %2

echo Deleting TMPs...
start /wait cmd /c DEL %2\*.tmp

CD %2
echo Converting PDB to MDB and deleting PDB...
"c:\Program Files\pdb2mdb\pdb2mdb.exe" %3
start /wait cmd /c DEL %4.pdb

echo Exporting Assembly to %DestinationDir%
echo Copying files to Destination...
echo f | xcopy "%1\bin\Core\Editor" %DestinationDir% /Y /I /E

echo Exporting Assembly to %DestinationDirNoMeta%
echo f | xcopy "%1\bin\Core\Editor" %DestinationDirNoMeta% /Y /I /E

echo Exporting Assembly to %DestinationDirUnityTests%
echo f | xcopy "%1\bin\Core\Editor" %DestinationDirUnityTests% /Y /I /E