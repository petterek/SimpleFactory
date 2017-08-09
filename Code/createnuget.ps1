Import-Module ItasBuildNuget -Verbose -Force

$Feed = "Default"
$IncludeReferencedProjects = $false;
$Output = "nuget";

ItasBuildNuget -Root $PSScriptRoot -Output $Output -Configuration "Release" -Nuspec  -Feed $Feed
Read-Host 'Script finished. Press Enter to exit' | Out-Null
