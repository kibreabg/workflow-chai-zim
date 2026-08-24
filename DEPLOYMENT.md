# Deployment Guide — WorkflowManagment to IIS via FTPS

This guide covers the **FTPS (FTP over SSL)** deployment path for the WorkflowManagment application.

## How it works

```
[GitHub push] → [CI Build workflow] → [Deploy to IIS (FTPS) workflow]
                                          ↓
                              GitHub-hosted Windows runner
                              builds + precompiles the site
                                          ↓
                              Uploads publish output over FTPS
                              to your IIS server's web root
                                          ↓
                              app_offline.htm dropped → files uploaded
                              → app_offline.htm removed (site back online)
```

- **No software installed on the IIS server** — it only needs the FTP/FTPS service (which you already have in IIS).
- **No firewall holes** — the GitHub runner connects *outbound* to your FTPS server.
- **Builds happen on GitHub's cloud** — your server never needs MSBuild or Visual Studio.

---

## Part 1 — One-time IIS server setup

### 1.1 Enable the FTP service (if not already installed)

1. Open **Server Manager** → **Add Roles and Features**.
2. Under **Web Server (IIS)** → **FTP Server**, check:
   - **FTP Service**
   - **FTP Extensibility**
3. Install.

### 1.2 Create an FTPS site (or bind FTP to your existing site)

1. Open **IIS Manager**.
2. Right-click **Sites** → **Add FTP Site** (or **Add FTP Publishing** on your existing site).
3. Set:
   - **Physical path**: your site's web root (e.g. `C:\inetpub\wwwroot\WorkflowManagment`)
   - **Binding**: IP address `All Unassigned`, port `21`, **Start FTP site automatically** ✓
   - **SSL**: **Require SSL** (this is what makes it FTPS)
4. **Authentication**: check **Basic**.
5. **Authorization**: allow the deploy user (created next) **Read + Write** permissions.

### 1.3 Create a dedicated deploy user

1. **Computer Management** → **Local Users and Groups** → **Users** → **New User**.
2. Create e.g. `svc-ftps-deploy` with a strong password.
3. In IIS Manager → your FTP site → **FTP Authorization Rules**:
   - Allow `svc-ftps-deploy` → **Read + Write**.
4. Grant the user NTFS permissions on the web root:
   ```powershell
   icacls "C:\inetpub\wwwroot\WorkflowManagment" /grant "svc-ftps-deploy:(OI)(CI)M" /T
   ```

### 1.4 Test FTPS manually

From any machine (e.g. your dev machine) with a tool like **FileZilla** or **WinSCP**:

- Host: `ftp.yourdomain.com` (or server IP)
- Port: `21`
- Protocol: **FTP over TLS/SSL (explicit)**
- User: `svc-ftps-deploy`
- Password: the one you set

You should be able to browse and upload files to the web root.

> **Note:** If your server is behind NAT, you'll need a port-forward for **TCP 21** (control) and the **passive data port range** (default 1024–65535, or a configured range like 50000–50100 in IIS FTP Firewall Support). GitHub runners connect outbound, so this is the only inbound rule needed.

---

## Part 2 — GitHub repository setup

### 2.1 Add GitHub Secrets

Go to **GitHub → your repo → Settings → Secrets and variables → Actions → New repository secret**:

| Secret | Value |
|---|---|
| `FTP_HOST` | Your FTPS host, e.g. `ftp.example.com` or `203.0.113.10` (no `ftp://` prefix) |
| `FTP_USERNAME` | `svc-ftps-deploy` |
| `FTP_PASSWORD` | The deploy user's password |
| `FTP_REMOTE_ROOT` | *(Optional)* The remote folder if the site root isn't the FTP user's home. Defaults to `/` |

### 2.2 What the workflow does

The workflow `.github/workflows/deploy-ftp.yml`:

1. **Triggers**:
   - Automatically after a successful **CI Build** on `master`.
   - Manually via **Actions → "Deploy to IIS (FTPS)" → Run workflow**.
2. **Builds** the site exactly like CI (precompiled, Release).
3. **Uploads** the publish output over FTPS using `scripts/deploy-ftps.ps1`, which:
   - Uploads `app_offline.htm` first → IIS stops the app and releases file locks.
   - Uploads all files (creating directories as needed).
   - **Skips** the upload folders (`SVUploads`, `BAUploads`, `CSUploads`) — these are runtime data.
   - **Skips** `web.config` — your server's connection strings stay untouched.
   - Removes `app_offline.htm` → site comes back online.

---

## Part 3 — First deployment

1. Push the workflow files to `master`:
   ```
   .github/workflows/deploy-ftp.yml
   scripts/deploy-ftps.ps1
   ```
2. Go to **Actions** → **Deploy to IIS (FTPS)** → **Run workflow**.
3. Watch the logs. You should see:
   ```
   [1/4] Taking site offline (app_offline.htm)...
   [2/4] Uploading files...
     [PUT]  ftp://.../Default.aspx
     ...
   [3/4] Bringing site back online...
   [4/4] Deployment complete.
   ```
4. Browse to your site and confirm it loads.

---

## Part 4 — Rollback

The workflow does **not** delete files on the server (it only uploads/overwrites), so a rollback is a manual restore:

1. On the server, back up the current site:
   ```powershell
   Copy-Item "C:\inetpub\wwwroot\WorkflowManagment" "C:\backups\wm-$(Get-Date -Format 'yyyyMMdd-HHmmss')" -Recurse
   ```
2. To roll back to a previous version, restore from a backup:
   ```powershell
   # Take site offline
   New-Item -ItemType File -Path "C:\inetpub\wwwroot\WorkflowManagment\app_offline.htm" -Force
   # Restore
   Remove-Item "C:\inetpub\wwwroot\WorkflowManagment\*" -Recurse -Force
   Copy-Item "C:\backups\wm-20240824-101500\*" "C:\inetpub\wwwroot\WorkflowManagment\" -Recurse
   # Bring site back online
   Remove-Item "C:\inetpub\wwwroot\WorkflowManagment\app_offline.htm" -Force
   ```

---

## Part 5 — Security notes

| Concern | Mitigation |
|---|---|
| **Credentials in transit** | The workflow uses **FTPS (explicit TLS)** — never plain FTP. |
| **Credentials at rest** | Stored as **GitHub Secrets** — never in the repo. |
| **web.config overwrite** | The deploy script **excludes `web.config`**, so production connection strings are never overwritten by the repo's dev config. |
| **Upload folders** | `SVUploads`, `BAUploads`, `CSUploads` are excluded — runtime data is never touched. |
| **Deploy user permissions** | Use a dedicated low-privilege user (`svc-ftps-deploy`) with write access **only** to the web root. |
| **Passive port range** | If behind NAT, forward TCP 21 + the configured passive range (IIS → FTP Firewall Support). |

---

## Troubleshooting

| Symptom | Likely cause / fix |
|---|---|
| `530 User cannot log in` | Wrong credentials, or the user isn't in the FTP Authorization Rules. |
| `534 Policy requires SSL` | The FTP site requires SSL but the client connected without it. The script uses `EnableSsl = $true`, so this should only happen if you test with a plain FTP client. |
| `550 Permission denied` | NTFS permissions on the web root missing for the deploy user. Re-run the `icacls` command. |
| `File in use` errors during upload | The `app_offline.htm` step should prevent this. If it still happens, check that the site's app pool actually stops (it should when `app_offline.htm` is present). |
| `Connection timed out` | Passive port range not forwarded, or firewall blocking outbound 21. |
| `The remote server returned an error: (550)` on directory creation | The FTP user lacks **Write** permission at the root, or `FTP_REMOTE_ROOT` points to a non-existent folder. |