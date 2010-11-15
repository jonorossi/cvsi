@echo off

set WIXDIR=..\..\tools\WiX\bin
set OUTPUT=Output

if exist "%OUTPUT%" rmdir /S /Q "%OUTPUT%"
mkdir "%OUTPUT%"

::%WIXDIR%\candle.exe -out %OUTPUT%\ -ext %WIXDIR%\WixUIExtension.dll Product.wxs
%WIXDIR%\candle.exe -out %OUTPUT%\ -pedantic -wx -sw1080 Product.wxs Integration.wxs

%WIXDIR%\light.exe -out %OUTPUT%\CVSI-0.3.0.msi -ext %WIXDIR%\WixUIExtension.dll -cultures:en-US %OUTPUT%\Integration.wixobj %OUTPUT%\Product.wixobj
