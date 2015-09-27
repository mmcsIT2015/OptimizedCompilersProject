
set target=%~1

if not defined target (
   set target=..\..\..\..\Common
)

if exist %target%\tests.txt (
	del /q /s %target%\tests.txt
)

dir /b /s "test-*" >> %target%\tests.txt

