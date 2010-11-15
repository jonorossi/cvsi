+===================+
| How to build CVSI |
+===================+
Step 1 - Install VS SDK
-----------------------
- To develop extensions for Visual Studio you will need to install the Visual Studio 2005 SDK Version 4.0:
  
  http://www.microsoft.com/downloads/details.aspx?FamilyID=51A5C65B-C020-4E08-8AC0-3EB9C06996F4&displaylang=en

Step 2 - Build Castle.VisualStudio.NVelocityLanguageService
-----------------------------------------------------------
- Ensure you have checked out \Castle.NVelocity\ from the castle contrib repository as well. Both Castle.NVelocity
  and CVSI folders need to be located the same as they are on the repository. The easiest option is to checkout
  the entire contrib repository.
- Open \CVSI\trunk\src\Castle.VisualStudio.NVelocityLanguageService\Castle.VisualStudio.NVelocityLanguageService.sln
  in Visual Studio.
- Go into the project properties, under Debug set these values (without the quotes):
  - Set "Start external program" to "C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\devenv.exe"
  - Set "Command line arguments" to "/rootsuffix Exp"
- Run the project.

Step 3 - Use Castle Visual Studio Integration
---------------------------------------------
- You will now be able to use the experimental hive of Visual Studio to open *.vm and *.njs files. When you want to
  open an NVelocity file you do not need start Visual Studio via the CVSI project, you can just run Visual Studio
  under the experimental hive as mentioned below because Visual Studio will be using the binaries you registered
  the last time you built CVSI.

+=================+
|      Notes      |
+=================+
- You can start the Visual Studio experimental hive copy at anytime via the
  "Start Visual Studio 2005 under Experimental hive" menu item in the start menu.
- If you installed the SDK into the 64-bit Program Files directory or changed the default directory then you
  will need to update the path to the targets file in the Castle.VisualStudio.NVelocityLanguageService.csproj
  file.


+=================+
|  Known Issues   |
+=================+
- Issues are logged in the Castle Project's JIRA (http://support.castleproject.org) under the Contrib project
  with the Castle Visual Studio Integration component.
