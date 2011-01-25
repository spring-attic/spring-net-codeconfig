REM other targets are:
REM 'build'
REM 'test'
REM 'test-integration'

@ECHO OFF
cls
@ECHO building...

tools\nant\bin\nant.exe %1 %2 %3 %4 %5 %6 %7 %8 %9 -f:Spring.Config.build > buildlog.txt

@ECHO displaying log file...
start "ignored but required placeholder window title argument" buildlog.txt
