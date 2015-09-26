cls

echo off
set current=%CD%
set target=..\..\..\..\Common

echo f | xcopy a.txt %target%\a.txt /R /Y /I
echo f | xcopy gplex.exe %target%\gplex.exe /R /Y /I
echo f | xcopy gppg.exe %target%\gppg.exe /R /Y /I
echo f | xcopy SimpleLex.lex %target%\SimpleLex.lex /R /Y /I
echo f | xcopy SimpleYacc.y %target%\SimpleYacc.y /R /Y /I
echo on

cd %target%\

gplex.exe /unicode SimpleLex.lex
gppg.exe /no-lines /gplex SimpleYacc.y

echo off
del /q SimpleLex.lex
del /q SimpleYacc.y
del /q gplex.exe
del /q gppg.exe

cd %current%
echo on
