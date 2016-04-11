CALL .\_vsaddin\build_env.bat

if %vm_do_compile% == 0 goto :eof
	
set IntDir=%1
set InputFileName=%2
set InputName=%3
set InputPath=%4
set InputDir=%5
set ProjectName=%6
set ProjectDir=%arduino_project_full_path%

%avr_bin_path%\avr-gcc -c -g -Os -w -ffunction-sections -fdata-sections  -mmcu=%arduino_build_mcu% -DF_CPU=%arduino_build_f_cpu% -I%InputDir% -I"%ProjectDir%" %arduino_compiler_include_paths% -o"%IntDir%\%InputFileName%.o" "%InputPath%" 
if errorlevel 1 goto fail1


%avr_bin_path%\avr-ar rcs %IntDir%\core.a %IntDir%\%InputFileName%.o
if errorlevel 1 goto fail2

goto :eof


:fail1
echo Failed to create %InputFileName%.o (avr-gcc)
goto :eof

:fail2
echo Failed to append %InputFileName%.o to core.a (avr-ar)
goto :eof
