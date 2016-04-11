call .\_vsaddin\build_env.bat

if %vm_do_compile% == 0 goto noDel

set OutDir=%1
set ProjectName=%2
set IntDir=%3

del %OutDir%\*.o
del %OutDir%\*.a

del %OutDir%\*.eep
del %OutDir%\*.elf
del %OutDir%\*.hex

exit 0


:noDel
exit 0
