# Copilot Instructions For workflowmanagment2.0

## Project Snapshot
- Solution type: legacy ASP.NET Web Site + class library modules on .NET Framework 4.8.
- Main solution: `WorkflowManagment.sln`.
- Web app entry: `WebSites/WorkflowManagment` (IIS Express website project in solution).
- Architecture style: Composite Web Application Block (CAB) / Prism-era modular Web Forms.
- ORM/data access: Entity Framework 6.5.1 in core libraries, with custom workspace abstraction.
- Dependency style: `packages.config` + repository-local `packages/` and `Library/` binary references.

## Solution Structure
- `WebSites/WorkflowManagment`: ASP.NET Web Forms website (pages, masters, Web.config, static assets).
- `Chai.WorkflowManagment.CoreDomain`: entities, EF context, repository/workspace abstractions.
- `Chai.WorkflowManagment.Services`: service locator-like static access and shared controller base.
- `Chai.WorkflowManagment.Shared`: shared utilities, settings/config handlers, navigation services.
- `Chai.WorkflowManagment.Enums`: domain enums used across modules.
- `Chai.WorkflowManagment.ServerControls`: custom controls.
- `Modules/*`: functional modules (Admin, Request, Approval, Report, Setting, Shell).

## Runtime Composition And Module Wiring
- CompositeWeb modules are registered in `WebSites/WorkflowManagment/Web.config` under `<compositeWeb><modules>`.
- `Shell` module provides key cross-cutting services (for example navigation service registration).
- Each module has a `*ModuleInitializer` class that extends `ModuleInitializer`.
- Web app uses Forms authentication and custom `AuthenticationModule` HTTP module.
- Global application class inherits `Microsoft.Practices.CompositeWeb.WebClientApplication`.

## Web Stack Conventions
- UI pattern is classic Web Forms + Presenter interfaces:
  - Pages in website folders such as `Admin/`, `Request/`, `Approval/`, `Report/`, `Setting/`.
  - Presenter/view contracts are in each module's `Views/` folder.
- Avoid introducing ASP.NET Core patterns, DI containers, or middleware assumptions.
- Keep code-behind and presenter responsibilities aligned with existing patterns.

## Data Access Conventions
- Use `WorkspaceFactory.Create()` for writable operations and `WorkspaceFactory.CreateReadOnly()` for queries.
- Use `IWorkspace` and `IReadOnlyWorkspace` abstractions instead of direct context usage unless existing code already does otherwise.
- Core EF context class: `WorkflowManagmentDbContext`.
- Base EF behavior is implemented in `BaseDbContext`, `EFWorkspace`, and `ReadOnlyEFWorkspace`.
- Existing code contains both LINQ-based queries and raw SQL string queries in controllers. Prefer safer LINQ/parameterized patterns for new changes, but keep changes minimal and compatible with current behavior.

## Important Legacy Constraints
- Keep naming as-is, including historical spelling `Managment` in namespaces/projects/paths.
- Do not migrate to SDK-style project format unless explicitly requested.
- Do not replace `packages.config` with `PackageReference` unless explicitly requested.
- Many references come from `Library/*.dll`; preserve those paths.
- Preserve CompositeWeb/Enterprise Library integration points.

## Build And Run Guidance
- Typical restore/build flow:
  1. Restore NuGet packages for `packages.config` projects.
  2. Build `WorkflowManagment.sln` in Visual Studio 2022 (solution format indicates VS 17).
- Web project is a website mapped to `http://localhost:61090` in solution metadata.
- There are no dedicated test projects currently in solution; validate via focused manual checks for changed pages/features.

## Security And Secrets Handling (Critical)
- Repository currently contains sensitive values (database and SMTP credentials) in config/code.
- Never add new secrets to source files.
- When editing related files, prefer environment-specific or transformed settings patterns if requested, and avoid expanding secret exposure.
- Treat these files as high-risk:
  - `WebSites/WorkflowManagment/Web.config`
  - `WebSites/WorkflowManagment/UserLogin.aspx.cs`

## Known Risk Areas For Refactoring
- Static singleton-like service holder in `WorkflowManagmentServices` can cause shared-state side effects.
- Authentication/session flow depends on FormsAuth + `ChaiPrincipal` + HTTP module wiring.
- Some module initializers and namespaces appear to contain legacy or inconsistent references; do not "clean up" broadly without a targeted request.
- `Chai.WorkflowManagment.DataAccess` project appears legacy and is not included in the active solution; verify before modifying it.

## Change Strategy For Agents
- Prefer narrow, surgical edits that match existing style and framework usage.
- When adding a feature, usually touch layers in this order:
  1. Domain entity/enums (if needed).
  2. Controller/service methods in the relevant module.
  3. Presenter interface and presenter implementation.
  4. Web Forms page/code-behind and markup.
  5. Module registration/config only if required.
- Validate null handling carefully around `GetCurrentUser()`, `EmployeePosition`, and session-backed fields.
- For data mutations, ensure `CommitChanges()` is called in the writable workspace path.

## File And Folder Hygiene
- Do not edit `bin/`, `obj/`, `packages/`, `.vs/`, generated logs, or output artifacts unless explicitly asked.
- Keep edits out of historical/unused artifacts unless task explicitly targets them.

## PR/Change Checklist For This Repo
- Change compiles in relevant projects.
- Composite module wiring remains valid.
- Authentication/login path still works for affected changes.
- No new secrets introduced.
- No broad rename/reformat across legacy codebase.
- Manual smoke check documented for touched pages/flows.
