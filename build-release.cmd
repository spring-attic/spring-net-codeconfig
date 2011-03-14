@ECHO OFF
cls

.\tools\nant\bin\nant.exe -f:Spring.Config.build package-zip -D:project.sign=true -D:project.releasetype=release -D:buildconfiguration=Release  > buildlog.txt
@rem .\tools\nant\bin\nant.exe -f:Spring.Config.build package-zip -D:project.sign=true -D:project.releasetype=release > buildlog.txt

start "ignored but required placeholder window title argument" buildlog.txt
