Install both dependancy throought pacakage manager onsole (PMC) 
  Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 3.1.1
  Install-Package Microsoft.EntityFrameworkCore.Tools -Version 3.1.1

Apply migration
  Add-Migration name_of_migrationfile
  Update-Database


Now yo able to run project.
