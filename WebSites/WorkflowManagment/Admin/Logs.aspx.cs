using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ObjectBuilder;
using Chai.WorkflowManagment.Shared;
using Chai.WorkflowManagment.CoreDomain.Users;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chai.WorkflowManagment.Modules.Admin.Views
{
    public partial class Logs : Microsoft.Practices.CompositeWeb.Web.UI.Page, ILogsView
    {
        private LogsPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this._presenter.OnViewInitialized();
                UploadFile();
            }
            this._presenter.OnViewLoaded();
        }

        [CreateNew]
        public LogsPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._presenter = value;
                this._presenter.View = this;
            }
        }

        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }

        protected void ClearFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
                UploadFile();
            }
        }

        protected void ViewFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            if (File.Exists(filePath))
            {
                if (Path.GetFileName(filePath).Equals("ErrorExceptions.log", StringComparison.OrdinalIgnoreCase))
                {
                    pnlLogDetails.Visible = false;
                    pnlExceptionManager.Visible = true;
                    LoadExceptions(filePath);
                }
                else
                {
                    pnlExceptionManager.Visible = false;
                    litLogDetails.Text = File.ReadAllText(filePath);
                    pnlLogDetails.Visible = true;
                }
            }
        }

        private void LoadExceptions(string filePath)
        {
            List<ExceptionEntry> exceptions = ParseExceptions(filePath);
            Session["LogExceptions"] = exceptions;
            lstExceptionTitles.DataSource = exceptions;
            lstExceptionTitles.DataTextField = "Title";
            lstExceptionTitles.DataValueField = "Id";
            lstExceptionTitles.DataBind();
        }

        private List<ExceptionEntry> ParseExceptions(string filePath)
        {
            // Each call to ExceptionUtility.LogException writes one block:
            //   [Inner Exception Type / Inner Exception / Inner Source / Inner Stack Trace]  (optional)
            //   Exception Type / Exception / Source / Stack Trace                            (always)
            // A new block therefore starts at "Inner Exception Type:" or at a second
            // "Exception Type:" line (an exception logged without an inner exception).
            List<ExceptionEntry> entries = new List<ExceptionEntry>();
            string[] lines = File.ReadAllLines(filePath);
            StringBuilder currentEntry = new StringBuilder();
            string currentTitle = null;
            string currentTimestamp = string.Empty;
            bool topLevelTypeSeen = false;
            int id = 0;

            foreach (string line in lines)
            {
                bool isInnerTypeLine = line.Contains("Inner Exception Type:");
                bool isTopTypeLine = !isInnerTypeLine && line.Contains("Exception Type:");

                if (isInnerTypeLine || isTopTypeLine)
                {
                    bool isNewBlock = isInnerTypeLine || topLevelTypeSeen;

                    if (isNewBlock && currentEntry.Length > 0)
                    {
                        entries.Add(CreateEntry(id++, currentTitle, currentTimestamp, currentEntry.ToString()));
                        currentEntry.Clear();
                        currentTitle = null;
                        topLevelTypeSeen = false;
                    }

                    currentTimestamp = ExtractTimestamp(line);
                }

                if (isTopTypeLine)
                {
                    // The top-level exception type identifies the whole entry
                    currentTitle = ExtractValue(line, "Exception Type:");
                    topLevelTypeSeen = true;
                }
                else if (!topLevelTypeSeen && isInnerTypeLine && currentTitle == null)
                {
                    // Fall back to the inner exception type until the top-level type appears
                    currentTitle = ExtractValue(line, "Inner Exception Type:");
                }

                currentEntry.AppendLine(line);
            }

            if (currentEntry.Length > 0)
            {
                entries.Add(CreateEntry(id++, currentTitle, currentTimestamp, currentEntry.ToString()));
            }

            entries.Reverse(); // Show latest first
            return entries;
        }

        private static ExceptionEntry CreateEntry(int id, string title, string timestamp, string details)
        {
            string displayTitle = string.IsNullOrEmpty(title) ? "Unknown Exception" : title;
            if (!string.IsNullOrEmpty(timestamp))
            {
                displayTitle = string.Format("{0}  ({1})", displayTitle, timestamp);
            }
            return new ExceptionEntry { Id = id, Title = displayTitle, Details = details };
        }

        private static string ExtractTimestamp(string line)
        {
            // log4net pattern: 2026-08-21 19:48:16,496 [thread] LEVEL logger - message
            int firstSpace = line.IndexOf(' ');
            int secondSpace = firstSpace > 0 ? line.IndexOf(' ', firstSpace + 1) : -1;
            return secondSpace > 0 ? line.Substring(0, secondSpace) : string.Empty;
        }

        private static string ExtractValue(string line, string marker)
        {
            int index = line.IndexOf(marker);
            return index >= 0 ? line.Substring(index + marker.Length).Trim() : string.Empty;
        }

        protected void lstExceptionTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<ExceptionEntry> exceptions = Session["LogExceptions"] as List<ExceptionEntry>;
            if (exceptions != null && lstExceptionTitles.SelectedIndex != -1)
            {
                int id = int.Parse(lstExceptionTitles.SelectedValue);
                ExceptionEntry entry = exceptions.Find(x => x.Id == id);
                if (entry != null)
                {
                    litExceptionDetails.Text = entry.Details;
                }
            }
        }

        protected void btnCloseExceptionManager_Click(object sender, EventArgs e)
        {
            pnlExceptionManager.Visible = false;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            pnlLogDetails.Visible = false;
        }

        private void UploadFile()
        {
            try
            {
                string AuditLog = Server.MapPath("~/AuditTrail.Log");
                string Errorlog = Server.MapPath("~/ErrorExceptions.Log");
                string FailedLoginslog = Server.MapPath("~/FailedLogins.Log");

                IList<Log> attachments = new List<Log>();
                Log l = new Log();
                l.FilePath = AuditLog;
                l.fileName = "Audit Trail";
                attachments.Add(l);
                Log l2 = new Log();
                l2.FilePath = Errorlog;
                l2.fileName = "Exceptions";
                attachments.Add(l2);
                Log l3 = new Log();
                l3.FilePath = FailedLoginslog;
                l3.fileName = "Failed Login Attempts";
                attachments.Add(l3);

                grvAttachments.DataSource = attachments;
                grvAttachments.DataBind();

            }
            catch (Exception ex)
            { 
              
            }
            
        }

        [Serializable]
        public class ExceptionEntry
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
        }

        public class Log
        {
            private string _fileName;
            private string _filePath;
            public  string fileName
            {
                get { return _fileName; }
                set { _fileName = value; }
            }
            public string FilePath
            {
                get { return _filePath; }
                set { _filePath = value; }
            }
        }
     
  
}
}
