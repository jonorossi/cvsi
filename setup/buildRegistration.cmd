@echo off

set REGPKG_2005_DIR=..\..\tools\RegPkg\VS2005
set REGPKG_2008_DIR=..\..\tools\RegPkg\VS2008
set BIN_2005_DIR=..\..\build\vs2005
set BIN_2008_DIR=..\..\build\vs2008

@echo on

::%REGPKG_2005_DIR%\regpkg.exe /codebase /root:Software\Microsoft\VisualStudio\8.0 /wixfile:LanguageServiceRegistration.VS2005.wxi %BIN_2005_DIR%\Castle.VisualStudio.NVelocityLanguageService.dll

::%REGPKG_2008_DIR%\regpkg.exe /codebase /root:Software\Microsoft\VisualStudio\9.0 /wixfile:LanguageServiceRegistration.VS2008.wxi %BIN_2008_DIR%\Castle.VisualStudio.NVelocityLanguageService.dll
