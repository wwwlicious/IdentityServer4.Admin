properties {
	$base_directory = Resolve-Path . 
	$src_directory = "$base_directory\source"
	$output_directory = "$base_directory\build"
	$dist_directory = "$base_directory\distribution"
	$sln_file = "$src_directory\IdentityServer4.Admin.sln"
	$target_config = "Release"
	$framework_version = "v4.5.2"
	$xunit_path = "$src_directory\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
	$ilmerge_path = "$src_directory\packages\ilmerge.2.14.1208\tools\ILMerge.exe"
	$nuget_path = "$src_directory\.nuget\nuget.exe"
	
	$buildNumber = 0;
	$version = "1.0.0.0"
	$preRelease = $null
}

task default -depends Clean, CreateNuGetPackage
#task appVeyor -depends Clean, CreateNuGetPackage

task Clean {
	rmdir $output_directory -ea SilentlyContinue -recurse
	rmdir $dist_directory -ea SilentlyContinue -recurse
	# exec { msbuild /nologo /verbosity:quiet $sln_file /p:Configuration=$target_config /t:Clean }
}

task Compile -depends UpdateVersion {
		exec { msbuild /nologo /verbosity:q $sln_file /p:Configuration=$target_config /p:TargetFrameworkVersion=v4.5.2 }

	if ($LastExitCode -ne 0) {
        exit $LastExitCode
    }
}

task UpdateVersion {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$assemblyFileVersion =  "$major.$minor.$patch.$buildNumber"
	$assemblyVersion = "$major.$minor.0.0"
	$versionAssemblyInfoFile = "$src_directory/VersionAssemblyInfo.cs"
	"using System.Reflection;" > $versionAssemblyInfoFile
	"" >> $versionAssemblyInfoFile
	"[assembly: AssemblyVersion(""$assemblyVersion"")]" >> $versionAssemblyInfoFile
	"[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]" >> $versionAssemblyInfoFile
}

task ILMerge -depends Compile {
	$input_dlls = "$output_directory\IdentityAdmin.dll"

	Get-ChildItem -Path $output_directory -Filter *.dll |
		foreach-object {
			# Exclude IdentityAdmin.dll as that will be the primary assembly
			if ("$_" -ne "IdentityAdmin.dll" -and 
			    "$_" -ne "Owin.dll") {
				$input_dlls = "$input_dlls $output_directory\$_"
			}
	}

	New-Item $dist_directory\lib\net45 -Type Directory
	Invoke-Expression "$ilmerge_path /targetplatform:v4 /internalize /allowDup /target:library /out:$dist_directory\lib\net45\IdentityServer4.Admin.dll $input_dlls"
}

task CreateNuGetPackage -depends ILMerge {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$packageVersion =  "$major.$minor.$patch"
	if($preRelease){
		$packageVersion = "$packageVersion-$preRelease" 
	}

	if ($buildNumber -ne 0){
		$packageVersion = $packageVersion + "-build-" + $buildNumber.ToString().PadLeft(5,'0')
	}

	copy-item $src_directory\Core\IdentityAdmin.nuspec $dist_directory
	exec { . $nuget_path pack $dist_directory\IdentityAdmin.nuspec -BasePath $dist_directory -o $dist_directory -version $packageVersion }
}
