:: %1-2-3-4 = $(SolutionDir) $(TargetDir) $(TargetFileName) $(TargetName)

set BinDir=bin.Global\DemiLib
set DestinationDir=%1..\..\%BinDir%
set BinDirNoMeta=bin.Global_no_meta\DemiLib
set DestinationDirNoMeta=%1..\..\%BinDirNoMeta%
set BinDirUnityTests=DemiLib.UnityTests.Unity5\Assets\Demigiant\DemiLib
set DestinationDirUnityTests=%1..\%BinDirUnityTests%
set BinDirUnityTests2018=DemiLib.UnityTests.Unity2018\Assets\Demigiant\DemiLib
set DestinationDirUnityTests2018=%1..\%BinDirUnityTests2018%

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
echo f | xcopy "%1\bin" %DestinationDir% /Y /I /E

echo Exporting Assembly to %DestinationDirNoMeta%
echo f | xcopy "%1\bin" %DestinationDirNoMeta% /Y /I /E

echo Exporting Assembly to %DestinationDirUnityTests%
echo f | xcopy "%1\bin" %DestinationDirUnityTests% /Y /I /E

echo Exporting Assembly to %DestinationDirUnityTests2018%
echo f | xcopy "%1\bin" %DestinationDirUnityTests2018% /Y /I /E