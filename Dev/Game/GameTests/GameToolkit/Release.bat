CALL C:\Factory\SetEnv.bat
CALL Clean.bat
cx **
IF ERRORLEVEL 1 C:\app\MsgBox\MsgBox.exe E "BUILD ERROR"

acp Silvia20200001\Silvia20200001\bin\Release\Silvia20200001.exe out\Game.exe

COPY /B C:\Dat\DxLibDotNet\DxLibDotNet3_24b\DxLib.dll out
COPY /B C:\Dat\DxLibDotNet\DxLibDotNet3_24b\DxLibDotNet.dll out

C:\apps\MakeResourceCluster\MakeResourceCluster.exe Resource out\Resource.dat
xcp doc out

C:\Factory\SubTools\zip.exe /PE- /O out *P
