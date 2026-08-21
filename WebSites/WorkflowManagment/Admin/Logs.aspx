<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Logs.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Admin.Views.Logs"
    Title="Users"  MasterPageFile="~/Shared/AdminMaster.master" %>
<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>

<asp:Content ID="content" ContentPlaceHolderID="DefaultContent" runat="Server">
     <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
                     <header>
					        <span class="widget-icon"> <i class="fa fa-edit"></i> </span>
					        <h2>Logs</h2>				
				    </header>
   <div role="content">

                        <!-- widget edit box -->
                        <div class="jarviswidget-editbox">
                            <!-- This area used as dropdown edit box -->

                        </div>
                        <!-- end widget edit box -->

                        <!-- widget content -->
                        <div class="widget-body">
                            <div class="tab-content">
                                <div class="tab-pane" id="hr1">
                                    <div class="tabbable tabs-below">
                                        <div class="tab-content padding-10">
                                            <div class="tab-pane" id="AA">
                                            </div>
                                        </div>
                                        <ul class="nav nav-tabs">
                                            <li class="active">
                                                <a data-toggle="tab" href="#AA">Tab 1</a>
                                            </li>
                                        </ul>
                                    </div>

                                </div>
                                <div class="tab-pane active" id="hr2">

                                    <ul class="nav nav-tabs">
                                        <li class="active">
                                            <a href="#iss1" data-toggle="tab">Logs</a>
                                        </li>
                                        
                                    </ul>
                                    <div class="tab-content padding-10">
                                        <div class="tab-pane active" id="iss1">
                                            <asp:GridView ID="grvAttachments"
                                                runat="server" AutoGenerateColumns="False"
                                                CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active">
                                                <RowStyle CssClass="rowstyle" />
                                                <Columns>
                                                    <asp:BoundField DataField="fileName" HeaderText="File Name" SortExpression="fileName" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkView" Text="View" CommandArgument='<%# Eval("filePath") %>' runat="server" OnClick="ViewFile" CssClass="btn btn-xs btn-primary"></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("filePath") %>' runat="server" OnClick="DownloadFile" CssClass="btn btn-xs btn-success"></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkClear" Text="Clear" CommandArgument='<%# Eval("filePath") %>' runat="server" OnClick="ClearFile" OnClientClick="return confirm('Are you sure you want to clear this log file?');" CssClass="btn btn-xs btn-danger"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                                <FooterStyle CssClass="FooterStyle" />
                                                <HeaderStyle CssClass="headerstyle" />
                                                <PagerStyle CssClass="PagerStyle" />
                                                <RowStyle CssClass="rowstyle" />
                                            </asp:GridView>
                                        </div>
                                  
                                    </div>
                                    <asp:Panel ID="pnlLogDetails" runat="server" Visible="false" Style="margin-top: 20px;">
                                        <div class="well">
                                            <header>
                                                <h3>Log Details</h3>
                                            </header>
                                            <div style="width: 100%; height: 400px; overflow-y: scroll; border: 1px solid #ccc; background-color: #fff; padding: 8px; white-space: pre-wrap; font-family: Consolas, 'Courier New', monospace; font-size: 12px;">
                                                <asp:Literal ID="litLogDetails" runat="server" Mode="Encode"></asp:Literal>
                                            </div>
                                            <div class="form-actions">
                                                <asp:Button ID="btnClose" runat="server" Text="Close" OnClick="btnClose_Click" CssClass="btn btn-default" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlExceptionManager" runat="server" Visible="false" Style="margin-top: 20px;">
                                        <div class="well">
                                            <header>
                                                <h3>Exception Log Manager</h3>
                                            </header>
                                            <div class="row">
                                                <div class="col-md-4">
                                                    <label>Exceptions (Latest First)</label>
                                                    <asp:ListBox ID="lstExceptionTitles" runat="server" Width="100%" Height="400px" CssClass="form-control"
                                                        AutoPostBack="true" OnSelectedIndexChanged="lstExceptionTitles_SelectedIndexChanged"></asp:ListBox>
                                                </div>
                                                <div class="col-md-8">
                                                    <label>Exception Details</label>
                                                    <div style="width: 100%; height: 400px; overflow-y: scroll; border: 1px solid #ccc; background-color: #fff; padding: 8px; white-space: pre-wrap; font-family: Consolas, 'Courier New', monospace; font-size: 12px;">
                                                        <asp:Literal ID="litExceptionDetails" runat="server" Mode="Encode"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-actions" style="margin-top: 10px;">
                                                <asp:Button ID="btnCloseExceptionManager" runat="server" Text="Close" OnClick="btnCloseExceptionManager_Click" CssClass="btn btn-default" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>

                        </div>
                        <!-- end widget content -->

                    </div>
   </div>
        
  
</asp:Content>
