# Deploy-FTPS.ps1
#
# Uploads a local publish folder to an IIS site over FTPS (FTP over SSL)
# using the .NET FtpWebRequest API (no third-party dependencies).
#
# Handles the app_offline.htm dance so IIS releases file locks before
# the upload, then brings the site back online.
#
# Usage:
#   .\deploy-ftps.ps1 -LocalPath "C:\publish" `
#                     -FtpHost "ftp.example.com" `
#                     -FtpUser "deployuser" `
#                     -FtpPass "secret" `
#                     -RemoteRoot "/" `
#                     -ExcludeFolders @("SVUploads","BAUploads","CSUploads") `
#                     -ExcludeFiles @("web.config")
#
# NOTE: This script is invoked by the GitHub Actions workflow
#       (.github/workflows/deploy-ftp.yml). It can also be run manually
#       from a machine that can reach the FTPS server.

param(
    [Parameter(Mandatory = $true)]
    [string]$LocalPath,

    [Parameter(Mandatory = $true)]
    [string]$FtpHost,

    [Parameter(Mandatory = $true)]
    [string]$FtpUser,

    [Parameter(Mandatory = $true)]
    [string]$FtpPass,

    [Parameter(Mandatory = $false)]
    [string]$RemoteRoot = "/",

    [Parameter(Mandatory = $false)]
    [string[]]$ExcludeFolders = @(),

    [Parameter(Mandatory = $false)]
    [string[]]$ExcludeFiles = @()
)

$ErrorActionPreference = "Stop"

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

function New-FtpRequest {
    param(
        [string]$Uri,
        [string]$Method
    )
    $req = [System.Net.FtpWebRequest]::Create($Uri)
    $req.Method = $Method
    $req.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
    $req.EnableSsl = $true          # FTPS (explicit TLS)
    $req.UsePassive = $true
    $req.KeepAlive = $false
    $req.UseBinary = $true
    return $req
}

function Test-FtpPath {
    param([string]$Uri)
    try {
        $req = New-FtpRequest -Uri $Uri -Method ([System.Net.WebRequestMethods+Ftp]::ListDirectory)
        $resp = $req.GetResponse()
        $resp.Close()
        return $true
    }
    catch {
        return $false
    }
}

function New-FtpDirectory {
    param([string]$Uri)
    if (-not (Test-FtpPath -Uri $Uri)) {
        try {
            $req = New-FtpRequest -Uri $Uri -Method ([System.Net.WebRequestMethods+Ftp]::MakeDirectory)
            $resp = $req.GetResponse()
            $resp.Close()
            Write-Host "  [DIR]  $Uri"
        }
        catch {
            # Directory may already exist (race) - ignore
        }
    }
}

function Send-FtpFile {
    param(
        [string]$LocalFile,
        [string]$RemoteFile
    )
    $req = New-FtpRequest -Uri $RemoteFile -Method ([System.Net.WebRequestMethods+Ftp]::UploadFile)
    $content = [System.IO.File]::ReadAllBytes($LocalFile)
    $req.ContentLength = $content.Length
    $stream = $req.GetRequestStream()
    $stream.Write($content, 0, $content.Length)
    $stream.Close()
    $resp = $req.GetResponse()
    $resp.Close()
    Write-Host "  [PUT]  $RemoteFile"
}

function Remove-FtpFile {
    param([string]$RemoteFile)
    try {
        $req = New-FtpRequest -Uri $RemoteFile -Method ([System.Net.WebRequestMethods+Ftp]::DeleteFile)
        $resp = $req.GetResponse()
        $resp.Close()
        Write-Host "  [DEL]  $RemoteFile"
    }
    catch {
        # File may not exist - ignore
    }
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

$baseUri = "ftp://$FtpHost$RemoteRoot"
$baseUri = $baseUri.TrimEnd('/') + "/"

Write-Host "=============================================="
Write-Host " FTPS Deploy"
Write-Host "  Host      : $FtpHost"
Write-Host "  Local     : $LocalPath"
Write-Host "  Remote    : $baseUri"
Write-Host "  Excl dirs : $($ExcludeFolders -join ', ')"
Write-Host "  Excl files: $($ExcludeFiles -join ', ')"
Write-Host "=============================================="

if (-not (Test-Path $LocalPath)) {
    throw "Local path not found: $LocalPath"
}

# 1. Upload app_offline.htm to take the site offline
$offlineContent = "<html><head><title>Site Under Maintenance</title></head><body><h1>Site is being updated. Please try again shortly.</h1></body></html>"
$offlineLocal = Join-Path $env:TEMP "app_offline.htm"
Set-Content -Path $offlineLocal -Value $offlineContent -Encoding ASCII
Write-Host "`n[1/4] Taking site offline (app_offline.htm)..."
Send-FtpFile -LocalFile $offlineLocal -RemoteFile ($baseUri + "app_offline.htm")

try {
    # 2. Upload all files (excluding configured folders/files)
    Write-Host "`n[2/4] Uploading files..."
    $fileCount = 0
    Get-ChildItem -Path $LocalPath -Recurse -File | ForEach-Object {
        $rel = $_.FullName.Substring($LocalPath.TrimEnd('\').Length + 1).Replace('\', '/')

        # Skip excluded files
        if ($ExcludeFiles -contains $_.Name) {
            Write-Host "  [SKIP] $rel (excluded file)"
            return
        }

        # Skip files inside excluded folders
        foreach ($ex in $ExcludeFolders) {
            if ($rel -like "$ex/*" -or $rel -eq $ex) {
                Write-Host "  [SKIP] $rel (excluded folder)"
                return
            }
        }

        # Ensure remote directory exists
        $dir = [System.IO.Path]::GetDirectoryName($rel)
        if ($dir) {
            $dirUri = $baseUri + $dir.Replace('\', '/') + "/"
            New-FtpDirectory -Uri $dirUri
        }

        Send-FtpFile -LocalFile $_.FullName -RemoteFile ($baseUri + $rel)
        $fileCount++
    }
    Write-Host "  Uploaded $fileCount files."

    # 3. Remove app_offline.htm to bring the site back online
    Write-Host "`n[3/4] Bringing site back online..."
    Remove-FtpFile -RemoteFile ($baseUri + "app_offline.htm")

    Write-Host "`n[4/4] Deployment complete."
}
catch {
    Write-Host "`n[ERROR] Deployment failed. Attempting to bring site back online..."
    try { Remove-FtpFile -RemoteFile ($baseUri + "app_offline.htm") } catch {}
    throw
}