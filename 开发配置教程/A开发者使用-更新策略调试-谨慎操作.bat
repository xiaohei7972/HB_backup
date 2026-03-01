
if exist Debug (rd /s /q Debug)
md Debug
cd Debug
if exist .git (echo ok) else (goto init)
git fetch --all  
git reset origin/master --hard
git config --global core.quotepath false
git config --global gui.encoding utf-8
git config --global i18n.commit.encoding utf-8
git config --global i18n.logoutputencoding utf-8
set LESSCHARSET=utf-8
git log --pretty=format:"%%an %%x09 %%ad %%x09 %%s %%x09" -5 --date=format:"%%y-%%m-%%d %%H:%%M:%%S"
echo Done
pause
exit
:init 
echo init.......
git init .
ping 127.0.0.1 -n 3 >nul
if exist .git (echo Git intalled) else (goto exception2)
if exist Hearthstone-myRoutines (rd /s /q Hearthstone-myRoutines)
git clone https://gitee.com/UniverseString/Hearthstone-myRoutines.git
cd ..
if exist ..\Bots (rd /s /q ..\Bots)
if exist ..\Plugins (rd /s /q ..\Plugins)
if exist ..\Routines (rd /s /q ..\Routines)
if exist ..\CompiledAssemblies (rd /s /q ..\CompiledAssemblies)
xcopy .\Debug\Hearthstone-myRoutines\Bots ..\Bots /E /I /C
xcopy .\Debug\Hearthstone-myRoutines\Plugins ..\Plugins /E /I /C
xcopy .\Debug\Hearthstone-myRoutines\Routines ..\Routines /E /I /C
if exist Debug (rd /s /q Debug)
if exist ..\Routines (echo finished) else (echo Install Git First!)
pause
exit
:exception1
echo ERROR
pause
exit
:exception2
echo  Install Git First!
pause
exit