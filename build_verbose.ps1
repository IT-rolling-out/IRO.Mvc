$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent;

function ReadBool($hint)
{
  $hint=$hint+" y/n (y) ";
  if($Silent -eq 1)
  {
    Write-Host $hint;
    return 1;
  }
  $answer = Read-Host $hint
  if($answer -eq 'n'){
    return 0;
  }
  return 1;
}

function AskConfiguration()
{
  $IsRelease = ReadBool "Please enter 'y' if you wan't to build in release mode"; 
  $IsRelease=$IsRelease;  
  if($IsRelease)
  {
    $Configuration="Release";
  }
  else
  {
    $Configuration="Debug";
  }
}

$NugetsOutputDir="$PSScriptRoot\output\nuget";
$WantClearOutputNuget = ReadBool "Want clear '$NugetsOutputDir' before build? "
if($WantClearOutputNuget){
  if(Test-Path -Path $NugetsOutputDir){
    rd $NugetsOutputDir -recurse;  	
  }  
  Write-Host "Removed."	  
}

$Configuration=""
$IsRelease="";

AskConfiguration;
& "$PSScriptRoot\build" -NotSilent 1 -IsRelease $IsRelease -NugetsOutputDir $NugetsOutputDir