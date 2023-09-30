CALL C:\Factory\SetEnv.bat
CALL Clean.bat
cx **
IF ERRORLEVEL 1 C:\app\MsgBox\MsgBox.exe E "BUILD ERROR"

acp Gattonero20200001\Gattonero20200001\bin\Release\Gattonero20200001.exe out\*P.exe
xcp doc out
xcp Resource out\Resource

COPY /B C:\Dat\DxLibDotNet\DxLibDotNet3_24b\DxLib.dll out
COPY /B C:\Dat\DxLibDotNet\DxLibDotNet3_24b\DxLibDotNet.dll out

C:\Factory\SubTools\zip.exe /PE- /O out *P
