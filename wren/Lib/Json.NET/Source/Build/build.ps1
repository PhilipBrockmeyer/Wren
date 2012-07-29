﻿properties { 
  $zipFileName = "Json35r7.zip"
  $signAssemblies = $false
  $signKeyPath = "D:\Development\Releases\newtonsoft.snk"
  $buildDocumentation = $false
  
  $baseDir  = resolve-path ..
  $buildDir = "$baseDir\Build"
  $sourceDir = "$baseDir\Src"
  $toolsDir = "$baseDir\Tools"
  $docDir = "$baseDir\Doc"
  $releaseDir = "$baseDir\Release"
  $workingDir = "$baseDir\Working"
  $builds = @(
    @{Name = "Newtonsoft.Json.Silverlight"; TestsName = "Newtonsoft.Json.Tests.Silverlight"; Constants="SILVERLIGHT"; FinalDir="Silverlight"},
    @{Name = "Newtonsoft.Json.Compact"; TestsName = "Newtonsoft.Json.Tests.Compact"; Constants="PocketPC"; FinalDir="Compact"},
    @{Name = "Newtonsoft.Json.Net20"; TestsName = "Newtonsoft.Json.Tests.Net20"; Constants="NET20"; FinalDir="DotNet20"},
    @{Name = "Newtonsoft.Json"; TestsName = "Newtonsoft.Json.Tests"; Constants=""; FinalDir="DotNet"}
  )
} 

task default -depends Test

# Ensure a clean working directory
task Clean {
  Set-Location $baseDir
  
  if (Test-Path -path $workingDir)
  {
    Write-Output "Deleting Working Directory"
    
    del $workingDir -Recurse -Force
  }
  
  New-Item -Path $workingDir -ItemType Directory
}

# Build each solution, optionally signed
task Build -depends Clean { 
  
  foreach ($build in $builds)
  {
    $name = $build.Name
    $finalDir = $build.FinalDir

    Write-Host -ForegroundColor Green "Building " $name
    Write-Host
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release /p:OutputPath=bin\Release\$finalDir\ /p:AssemblyOriginatorKeyFile=$signKeyPath "/p:SignAssembly=$signAssemblies" (GetConstants $build.Constants $signAssemblies) ".\Src\$name.sln" } "Error building $name"
  }
}

# Merge LinqBridge into .NET 2.0 build
task Merge -depends Build {
  $binaryDir = "$sourceDir\Newtonsoft.Json\bin\Release\DotNet20"
  MergeAssembly "$binaryDir\Newtonsoft.Json.Net20.dll" $signKeyPath "$binaryDir\LinqBridge.dll"
  del $binaryDir\LinqBridge.dll

  $binaryDir = "$sourceDir\Newtonsoft.Json.Tests\bin\Release\DotNet20"
  MergeAssembly "$binaryDir\Newtonsoft.Json.Tests.Net20.dll" $signKeyPath "$binaryDir\LinqBridge.dll"
  MergeAssembly "$binaryDir\Newtonsoft.Json.Net20.dll" $signKeyPath "$binaryDir\LinqBridge.dll"
  del $binaryDir\LinqBridge.dll
}

# Optional build documentation, add files to final zip
task Package -depends Merge {
  foreach ($build in $builds)
  {
    $name = $build.TestsName
    $finalDir = $build.FinalDir
    
    Copy-Item -Path "$sourceDir\Newtonsoft.Json\bin\Release\$finalDir" -Destination $workingDir\Package\Bin\$finalDir -recurse
  }
  
  if ($buildDocumentation)
  {
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:DocumentationSourcePath=$workingDir\Package\Bin\DotNet" $docDir\doc.shfbproj } "Error building documentation. Check that you have Sandcastle, Sandcastle Help File Builder and HTML Help Workshop installed."
    
    move -Path $workingDir\Documentation\Documentation.chm -Destination $workingDir\Package\Documentation.chm
    move -Path $workingDir\Documentation\LastBuild.log -Destination $workingDir\Documentation.log
  }
  
  Copy-Item -Path $docDir\readme.txt -Destination $workingDir\Package\
  Copy-Item -Path $docDir\versions.txt -Destination $workingDir\Package\Bin\

  robocopy $sourceDir $workingDir\Package\Source\Src /MIR /NP /XD .svn bin obj /XF *.suo *.user
  robocopy $buildDir $workingDir\Package\Source\Build /MIR /NP /XD .svn
  robocopy $docDir $workingDir\Package\Source\Doc /MIR /NP /XD .svn
  robocopy $toolsDir $workingDir\Package\Source\Tools /MIR /NP /XD .svn
  
  exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileName $workingDir\Package\* } "Error zipping"
}

# Unzip package to a location
task Deploy -depends Package {
  exec { .\Tools\7-zip\7za.exe x -y "-o$workingDir\Deployed" $workingDir\$zipFileName } "Error unzipping"
}

# Run tests on deployed files
task Test -depends Deploy {
  foreach ($build in $builds)
  {
    $name = $build.TestsName
    $finalDir = $build.FinalDir
    
    Write-Host -ForegroundColor Green "Copying test assembly $name to deployed directory"
    Write-Host
    robocopy ".\Src\Newtonsoft.Json.Tests\bin\Release\$finalDir" $workingDir\Deployed\Bin\$finalDir /NP /XO /XF LinqBridge.dll
    
    Copy-Item -Path ".\Src\Newtonsoft.Json.Tests\bin\Release\$finalDir\$name.dll" -Destination $workingDir\Deployed\Bin\$finalDir\

    Write-Host -ForegroundColor Green "Running tests " $name
    Write-Host
    exec { .\Tools\NUnit\nunit-console.exe "$workingDir\Deployed\Bin\$finalDir\$name.dll" /xml:$workingDir\$name.xml } "Error running $name tests"
  }
}

function MergeAssembly($dllPrimaryAssembly, $signKey, [string[]]$mergedAssemlies)
{
  $mergeAssemblyPaths = [String]::Join(" ", $mergedAssemlies)
  
  $primary = Get-Item $dllPrimaryAssembly
  $mergedAssemblyName = $primary.Name
  $temporaryDir = $primary.DirectoryName + "\" + [Guid]::NewGuid().ToString()
  New-Item $temporaryDir -ItemType Directory
  
  $ilMergeKeyFile = switch($signAssemblies) { $true { "/keyfile:$signKeyPath" } default { "" } }
  
  try
  {
    exec { .\Tools\ILMerge\ilmerge.exe "/internalize" "/closed" "/log:$workingDir\$mergedAssemblyName.MergeLog.txt" $ilMergeKeyFile "/out:$temporaryDir\$mergedAssemblyName" $dllPrimaryAssembly $mergeAssemblyPaths } "Error executing ILMerge"
    Copy-Item -Path $temporaryDir\$mergedAssemblyName -Destination $dllPrimaryAssembly -Force
  }
  finally
  {
    Remove-Item $temporaryDir -Recurse -Force
  }
}

function GetConstants($constants, $includeSigned)
{
  $signed = switch($includeSigned) { $true { ";SIGNED" } default { "" } }

  return "/p:DefineConstants=`"TRACE;$constants$signed`""
}