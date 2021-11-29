#requires -version 5
<#
Based on versioning script by Anton Smolkov from https://stackoverflow.com/questions/54456640/how-to-configure-gitversion-for-release-flow
#>

Function IsMain($branch)
{
	if ($branch -match "refs/heads/main$")
	{
		return $true;
	}
	if ($branch -match "refs/heads/master$")
	{
		return $true;
	}
	if ($branch -match "refs/remotes/origin/main$")
	{
		return $true;
	}
	if ($branch -match "refs/remotes/origin/master$")
	{
		return $true;
	}
	if ($branch -match "main$")
	{
		return $true;
	}
	if ($branch -match "master$")
	{
		return $true;
	}

	return $false;
}

Function IsRelease($branch)
{
	if ($branch -match "refs/heads/release/")
	{
		return $true;
	}
	if ($branch -match "refs/remotes/origin/release/")
	{
		return $true;
	}

	return $false;
}

$env:LC_ALL='C.UTF-8'
[Console]::OutputEncoding = [System.Text.Encoding]::GetEncoding("utf-8")

$CurrentBranchName = (git branch | where {$_.trim().startswith('*')}).trimstart('*').trim()
$CurrentBranchFullName = $CurrentBranchName
$CurrentCommit = git rev-parse HEAD
$BranchHeads = @()
$BranchHeads += git show-ref

if ($CurrentBranchName -match 'detached at')
{
    Write-Host "In detached HEAD mode, looking for a branch with the commit $CurrentCommit at it's head"

    foreach ($BranchHead in $BranchHeads) 
    {
        if (($BranchHead -match "^$($CurrentCommit)" -And (IsRelease($BranchHead))))
        {
            $CurrentBranchName = $BranchHead.substring($CurrentCommit.length).trim()            
            $CurrentBranchFullName = $CurrentBranchName
            $CurrentBranchName = $CurrentBranchName.substring("refs/remotes/origin/".length)
            Write-Host "Found '$CurrentBranchName'"
            break
        }
    }

    if ($CurrentBranchName -match 'detached at')
    {
        foreach ($BranchHead in $BranchHeads) 
        {
            if (($BranchHead -match "^$($CurrentCommit)" -And (IsMain($BranchHead))))
            {
                $CurrentBranchName = $BranchHead.substring($CurrentCommit.length).trim()            
                $CurrentBranchFullName = $CurrentBranchName
                $CurrentBranchName = $CurrentBranchName.substring("refs/remotes/origin/".length)
                Write-Host "Found '$CurrentBranchName'"
                break
            }
        }
    }
    
    if ($CurrentBranchName -match 'detached at')
    {
        foreach ($BranchHead in $BranchHeads) 
        {
            if ($BranchHead -match "^$($CurrentCommit)")
            {
                $CurrentBranchName = $BranchHead.substring($CurrentCommit.length).trim()
                $CurrentBranchFullName = $CurrentBranchName
                Write-Host "Found '$CurrentBranchName'"
                break
            }
        }
    }
}

$mainBranchName = ""
foreach ($BranchHead in $BranchHeads) 
{
    if ($BranchHead -match " refs/remotes/origin/main$")
    {
        $mainBranchName = "refs/remotes/origin/main"
        break
    }
    if ($BranchHead -match " refs/remotes/origin/master$")
    {
        $mainBranchName = "refs/remotes/origin/master"
        break
    }
}

if ($mainBranchName -match "^$")
{
    foreach ($BranchHead in $BranchHeads) 
    {
        if ($BranchHead -match " refs/heads/main$")
        {
            $mainBranchName = "refs/heads/main"
            break
        }
        if ($BranchHead -match " refs/heads/master$")
        {
            $mainBranchName = "refs/heads/master"
            break
        }
    }
}

if ($CurrentBranchName -match 'detached at')
{
    Write-Host "Did not find a current branch, using the shortened commit hash in place of a branch identifier" -ForegroundColor Yellow
    $CurrentBranchName = "Commit-" + $CurrentCommit.substring(0, 8)
    $CurrentBranchFullName = $CurrentCommit
}

Write-Host "Current branch: $CurrentBranchName"
Write-Host "Current commit: $CurrentCommit"

$BranchTag = ""
$ActualCommitCounterTarget = ""

$RevList = @()
if ($CurrentBranchName -match '(^release|/release)/(?<Major>[0-9]+)$')
{
    Write-Host "Working in release branch mode"
    Write-Host "Versioning base will be the merge base between main and $CurrentBranchName"

    $ActualCommitCounterTarget = "Patch"
    $BaseVersion=[System.Version]("$([int]$Matches.Major).0.0")
    $AncestorCommitHash = git merge-base $mainBranchName $CurrentBranchFullName
}
else
{
    $BaseVersion = [version]'0.0.0'
    $CommitsCounter = '0'
    $MinorBump = '0'
	
    if (IsMain($CurrentBranchName))
    {
        $ActualCommitCounterTarget = "Minor"
        Write-Host "Working in main branch mode"
    }
    else
    {
		$ActualCommitCounterTarget = "TagSuffix"
        $BranchTag = "-alpha"
        Write-Host "Working in feature/PR branch mode - evaluating base version for $CurrentBranchName"
		
		# This effectively says 'what would main be on?' and then adds 1. So a feature branch from a main that would be on 3.1.0 would yield 3.2.0-alpha.x
		$VersionSources = @()
		$VersionSources += git branch --list -a | % {$_.trimstart('*').trim()} | ? {$_ -match '(^release|/release)/(?<Major>[0-9]+)$'} | `
			Select-Object @{n = 'VersionBranchName'; e = {$_}}, @{n='BaseVersion'; e={[System.Version]("$([int]$Matches.Major).0.0")}} | `
			Sort-Object BaseVersion -Descending

		foreach ($VersionSource in $VersionSources) 
		{
			$AncestorCommitHash = git merge-base $mainBranchName $VersionSource.VersionBranchName
			
			if ( $(git merge-base --is-ancestor $AncestorCommitHash $CurrentCommit ; $LASTEXITCODE) -eq 0) 
			{
				$BaseVersion = $VersionSource.BaseVersion

				$RevList += git rev-list "$mainBranchName" "^$AncestorCommitHash"
				$MinorBump = $RevList.count + 1
				break
			}
		}
    }
    
    Write-Host "Versioning base will be the merge base between the most recent release branch and $CurrentBranchName"

    $VersionSources = @()
    $VersionSources += git branch --list -a | % {$_.trimstart('*').trim()} | ? {$_ -match '(^release|/release)/(?<Major>[0-9]+)$'} | `
        Select-Object @{n = 'VersionBranchName'; e = {$_}}, @{n='BaseVersion'; e={[System.Version]("$([int]$Matches.Major).0.0")}} | `
        Sort-Object BaseVersion -Descending

    foreach ($VersionSource in $VersionSources) 
    {
        $AncestorCommitHash = git merge-base $mainBranchName $VersionSource.VersionBranchName
        
        if ( $(git merge-base --is-ancestor $AncestorCommitHash $CurrentCommit ; $LASTEXITCODE) -eq 0) 
        {
            $BaseVersion = $VersionSource.BaseVersion
			$($BaseVersion.GetType().GetField('_Minor', 'static,nonpublic,instance')).setvalue($BaseVersion, [int]$BaseVersion.Minor + [int]$MinorBump)   
            Write-Host "Using $($VersionSource.VersionBranchName) as the versioning base"
            break
        }
        else
        {
            Write-Host "Considered $($VersionSource.VersionBranchName) but could not find a merge base" -ForegroundColor DarkGray
        }
    }
}

$RevList = @()
if ($AncestorCommitHash -match '.+')
{
    Write-Host "Versioning base commit is $AncestorCommitHash"
    $RevList += git rev-list "$CurrentCommit" "^$AncestorCommitHash"
}
else
{
    Write-Host "There is no versioning base commit, using entire history"
    $RevList += git rev-list "$CurrentCommit"
}

$CommitsCounter = $RevList.count

Write-Host "Commits between versioning base and current commit: $CommitsCounter"

if ($ActualCommitCounterTarget -match "TagSuffix")
{
	Write-Host "Adding commit count to tag suffix"
	$BranchTag += ".$CommitsCounter"
	Write-Host "Tag is now $($BranchTag)"
}
else
{
	if ($ActualCommitCounterTarget -match "Patch")
	{
		Write-Host "Adding commit count to patch"
		$($BaseVersion.GetType().GetField('_Build', 'static,nonpublic,instance')).setvalue($BaseVersion, [int]$BaseVersion.Build + [int]$CommitsCounter)
	}
	elseif ($ActualCommitCounterTarget -match "Minor")
	{
		Write-Host "Adding commit count to minor"
		$($BaseVersion.GetType().GetField('_Minor', 'static,nonpublic,instance')).setvalue($BaseVersion, [int]$BaseVersion.Minor + [int]$CommitsCounter)           
	}
	Write-Host "Version is now $($BaseVersion)"
}

$CalculatedNugetVersion = "$($BaseVersion.Major).$($BaseVersion.Minor).$($BaseVersion.Build)"

$CalculatedAssemblyVersion = "$($BaseVersion.Major).$($BaseVersion.Minor).$($BaseVersion.Build).0"

$CalculatedAssemblyInformationalVersion = "$CalculatedNugetVersion$BranchTag"

Write-Host "Calculated output:"
Write-Host "                 AutoVersion_Major: $($BaseVersion.Major)"
Write-Host "                 AutoVersion_Minor: $($BaseVersion.Minor)"
Write-Host "                 AutoVersion_Patch: $($BaseVersion.Build)"
Write-Host "       AutoVersion_MajorMinorPatch: $($BaseVersion.Major).$($BaseVersion.Minor).$($BaseVersion.Build)"
Write-Host "        AutoVersion_AssemblySemVer: $($CalculatedAssemblyVersion)"
Write-Host "  AutoVersion_InformationalVersion: $($CalculatedAssemblyInformationalVersion)"
Write-Host "                AutoVersion_SemVer: $($CalculatedAssemblyInformationalVersion)"

if ($Env:GITHUB_ENV -match ".+")
{
	echo "AutoVersion_Major=$($BaseVersion.Major)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_Minor=$($BaseVersion.Minor)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_Patch=$($BaseVersion.Build)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_MajorMinorPatch=$($BaseVersion.Major).$($BaseVersion.Minor).$($BaseVersion.Build)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_AssemblySemVer=$($CalculatedAssemblyVersion)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_InformationalVersion=$($CalculatedAssemblyInformationalVersion)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
	echo "AutoVersion_SemVer=$($CalculatedAssemblyInformationalVersion)" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
}	
