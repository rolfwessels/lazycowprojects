call .\_vsaddin\build_env.bat
set OutDir=%1
set ProjectName=%2
set IntDir=%3

if %vm_do_compile% == 0 echo ...Ignored. Using previous build files


if %vm_do_compile% == 1 %avr_bin_path%\avr-gcc  -Os -Wl,--gc-sections -mmcu=%arduino_build_mcu% -o %OutDir%\%ProjectName%.cpp.elf %OutDir%\include_sketch.cpp.o %OutDir%\core.a -L%OutDir% -lm
if errorlevel 1 goto fail1
if %vm_do_compile% == 1 %avr_bin_path%\avr-objcopy -O ihex -j .eeprom --set-section-flags=.eeprom=alloc,load --no-change-warnings --change-section-lma .eeprom=0 %OutDir%\%ProjectName%.cpp.elf %OutDir%\%ProjectName%.cpp.eep
if errorlevel 1 goto fail2
if %vm_do_compile% == 1 %avr_bin_path%\avr-objcopy -O ihex -R .eeprom %OutDir%\%ProjectName%.cpp.elf %OutDir%\%ProjectName%.cpp.hex
if errorlevel 1 goto fail3
if %vm_do_compile% == 1 echo Created sketch %ProjectName%.hex


if %vm_do_upload% == 1 echo Upload "%OutDir%\%ProjectName%.cpp.hex"
if %vm_do_upload% == 1 %avr_bin_path%\avrdude.exe -C%avr_etc_path%\avrdude.conf -p%arduino_build_mcu% -c%arduino_upload_protocol%v1 -P\\.\%arduino_upload_port% -b%arduino_upload_speed% -D -u -V -Uflash:w:%OutDir%\%ProjectName%.cpp.hex:i
if errorlevel 1 goto uploadFailed



goto :eof





:fail1
echo Failed to link %ProjectName%.cpp.elf
goto :eof

:fail2
echo Failed to create %ProjectName%.cpp.eep
goto :eof

:fail3
echo Failed to create %ProjectName%.hex
goto :eof

:uploadFailed
echo Failed to upload %ProjectName%.hex
pause
goto :eof

:eof