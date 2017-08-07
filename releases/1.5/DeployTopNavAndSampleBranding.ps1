
Add-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue

#$webApp = "http://intranet.dev.local/"
$literalPath = "C:\Source\Codeplex\sptopnav\Main\TopNavProvider\WSP\"
$featureName = 'SampleBranding_SampleMasterPageGallery'

function WaitForJobToFinish([string]$SolutionFileName)
{ 
	$JobName = "*solution-deployment*$SolutionFileName*"
	$job = Get-SPTimerJob | ?{ $_.Name -like $JobName }
	if ($job -eq $null) 
	{
		Write-Host 'Timer job not found'
	}
	else
	{
		$JobFullName = $job.Name
		Write-Host -NoNewLine "Waiting to finish job $JobFullName"
		
		while ((Get-SPTimerJob $JobFullName) -ne $null) 
		{
			Write-Host -NoNewLine .
			Start-Sleep -Seconds 5
		}
		Write-Host
		Write-Host  "Finished waiting for job.."
	}
}

Write-Host "===================================================================================================="
Write-Host "De-activating features in all site collections"
#$webApps = Get-SPWebApplication
$webApps = "http://your.dns","http://your.other.dns"
foreach ($webApp in $webApps)
{
    $siteCollections = Get-SPWebApplication $webApp | Get-SPSite
    foreach ($site in $siteCollections)
    {
    	$url = $site.Url
    	Write-Host "De-activating the $featureName feature in the $url site collection"
    	disable-spfeature -Identity $featureName -url $url -confirm:$false
    }
}

Write-Host "De-activation completed."
Write-Host "===================================================================================================="

$solution = "Sample Branding Package.wsp"
Write-Host 'Uninstall Solution: $solution'
Uninstall-SPSolution -Identity $solution -Confirm:$false
	
Write-Host 'Waiting for job to finish'
WaitForJobToFinish 

Write-Host 'Remove solution $solution'
Remove-SPSolution –Identity $solution -confirm:$false

$solution = "Core.wsp"
Write-Host 'Uninstall Solution: $solution'
Uninstall-SPSolution -Identity $solution -Confirm:$false
	
Write-Host 'Waiting for job to finish'
WaitForJobToFinish 

Write-Host 'Remove solution $solution'
Remove-SPSolution –Identity $solution -confirm:$false

$solution = "TopNavProvider.wsp"
Write-Host 'Uninstall Solution: $solution'
Uninstall-SPSolution -Identity $solution -Confirm:$false -AllWebApplications
	
Write-Host 'Waiting for job to finish'
WaitForJobToFinish 

Write-Host 'Remove solution $solution'
Remove-SPSolution –Identity $solution -confirm:$false


#Add the new one back
$solutions = "Core.wsp","PacAl - Branding Package.wsp"
foreach ($solution in $solutions)
{
	$solPath = $literalPath + $solution
	Add-SPSolution $solPath
}

$solPath = $literalPath + "TopNavProvider.wsp"
Add-SPSolution $solPath

foreach ($solution in $solutions)
{
	Write-Host "Installing the $solution Package in the $site site collection"
	Install-SPSolution -Identity $solution -GACDeployment -Force
	Write-Host 'Waiting for job to finish' 
	WaitForJobToFinish 
}

Install-SPSolution -Identity "TopNavProvider.wsp" -GACDeployment -AllWebApplications
Write-Host 'Waiting for job to finish' 
WaitForJobToFinish 


Write-Host "===================================================================================================="
Write-Host "Activating features in all site collections"

$webApps = "http://your.dns","http://your.other.dns"
foreach ($webApp in $webApps)
{
    $siteCollections = Get-SPWebApplication $webApp | Get-SPSite
    foreach ($site in $siteCollections)
    {
    	$url = $site.Url
    	Write-Host "Activating the $featureName feature in the $url site collection"
    	enable-spfeature -Identity $featureName -url $url
    }
}

Write-Host "Deploy Completed. Check for errors."
Write-Host "===================================================================================================="