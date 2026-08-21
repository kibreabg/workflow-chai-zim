# Cline Rules: Architecture Orientation for WorkflowManagment2.0

This is an N-tier ASP.NET Web Forms (.NET Framework 4.x) application using Entity Framework 6.5.1. Full architecture details live in `structure.md` at the repo root. Before making changes, always check this file plus `structure.md`.

## Layer Responsibilities

- **CoreDomain**: Domain entities, business rules, and core interfaces (IEntity, IWorkspace, IReadOnlyWorkspace). Contains 30+ enums (WorkflowType, AccessLevel, DocumentType, etc.) and entity classes for Users, Settings, Requests, TravelLogs, etc. Repository pattern with Entity Framework.

- **Shared**: Utilities, configuration constants, and common services. Contains AppConstants.cs (application-wide constants), TechnicalConfig.cs (technical settings), UserConfig.cs (user configuration), Encryption.cs, ExceptionUtility.cs, AppMessage.cs, SingleSMS.cs, and sub-folders for Events, FusionUtil, MailSender, Navigation, and Settings.

- **Services**: Business logic entry point. Main class is WorkflowManagmentServices.cs, with ControllerBase.cs as base class for controllers. Exposes business operations to the presentation layer via module initializers (ServicesModuleInitializer.cs).

- **DataAccess**: Repository pattern implementation. BaseDao.cs provides base data operations; individual DAOs (UserDao, RoleDao) handle specific entity operations. DatabaseHelper.cs contains centralized database utilities. All data access goes through Entity Framework 6.5.1.

- **DBConnection**: Connection management. ConnectionManager.cs and DatabaseConn.cs handle database connections. DBConnectionModuleInitializer.cs sets up connection initialization.

- **Enums**: Centralized enumeration definitions (30+ enums) used across the solution for consistent state management (WorkflowType, AccessLevel, DocumentType, MonthName, Level, etc.).

- **ServerControls**: Custom ASP.NET server controls. Currently includes Calendar.cs for calendar functionality.

- **Modules/* (Admin, Approval, Report, Request, Setting, Shell, Library)**: Feature-specific module libraries organized by feature folder pattern. Each module is a separate class library project composed in the Shell module (Shell/ provides the composition root and bootstrapping). Modules enable loose coupling and plug-in architecture.

## Request Flow

1. **Web Forms page/code-behind** (View) in `WebSites/WorkflowManagment/WorkflowManagment/` loads (.aspx/.ascx).
2. **Code-behind** activates presenter/controller logic, calling `WorkflowManagmentServices`.
3. **Service Layer** (`WorkflowManagmentServices.cs`) orchestrates business operations.
4. **DataAccess** (`BaseDao` + DAOs) fetches/persists data through **Entity Framework 6.5.1** against **CoreDomain** entities.
5. Results return to the View for rendering.

Config flows bottom-up: CoreDomain/App.config, DataAccess/App.Config, Services/AppConfig, DBConnection/ConnectionManager.cs, then web.config (WebSites/WorkflowManagment/WorkflowManagment/) for ASP.NET settings (connection strings, session state, auth). Shared project provides TechnicalConfig.cs, UserConfig.cs, and AppConstants.cs for cross-project configuration.

## Instructions for Cline

- Always check `architecture.md` + `structure.md` before making changes.
- Place new code in the correct layer/module rather than putting logic directly in code-behind.
- Follow the N-tier separation: entities in CoreDomain, logic in Services, data in DataAccess, config where it belongs.
- Use module initializers for dependency wiring; do not bypass the composition root.
- When adding features, prefer creating new module projects or extending existing ones rather than monolithic changes to CoreDomain or Services.