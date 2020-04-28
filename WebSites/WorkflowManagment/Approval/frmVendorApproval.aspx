<%@ Page Title="Vendor Request Approval Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmVendorApproval.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Approval.Views.frmVendorApproval" EnableEventValidation="false" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <script type="text/javascript">
        function Clickheretoprint(theid) {
            var disp_setting = "toolbar=yes,location=no,directories=yes,menubar=yes,";
            disp_setting += "scrollbars=yes,width=750, height=600, left=100, top=25";
            var content_vlue = document.getElementById(theid).innerHTML;

            var docprint = window.open("", "", disp_setting);
            docprint.document.open();
            docprint.document.write('<html><head><title>CHAI Zimbabwe</title>');
            docprint.document.write('</head><body onLoad="self.print()"><center>');
            docprint.document.write(content_vlue);
            docprint.document.write('</center></body></html>');
            docprint.document.close();
            docprint.focus();
        }
    </script>
    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Vendor Requests</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-3">
                                <asp:Label ID="lblSrchRequestNo" runat="server" Text="Request No" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtSrchRequestNo" runat="server" Visible="true"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchRequestDate" runat="server" Text="Request Date" CssClass="label"></asp:Label>
                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="txtSrchRequestDate" CssClass="form-control datepicker" data-dateformat="mm/dd/yy" runat="server" Visible="true"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchProgressStatus" runat="server" Text="Status" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlSrchProgressStatus" runat="server">
                                    </asp:DropDownList><i></i>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="btnPop" runat="server" />
                        <asp:Button ID="btnPop2" runat="server" />
                        <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-primary" OnClick="btnFind_Click"></asp:Button>
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>
        <div class="table-responsive" style="overflow: auto;">
            <asp:GridView ID="grvSupplierList"
                runat="server" AutoGenerateColumns="False" DataKeyNames="Id" OnRowCommand="grvSupplierList_RowCommand"
                OnRowDataBound="grvSupplierList_RowDataBound" OnSelectedIndexChanged="grvSupplierList_SelectedIndexChanged"
                AllowPaging="True" OnPageIndexChanging="grvSupplierList_PageIndexChanging"
                CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" PageSize="30">
                <RowStyle CssClass="rowstyle" />
                <Columns>
                    <asp:BoundField DataField="RequestNo" HeaderText="Vendor No" SortExpression="RequestNo" />
                    <asp:BoundField DataField="AppUser.FullName" HeaderText="Requester" SortExpression="AppUser.FullName" />
                    <asp:BoundField DataField="VendorLegalEntityName" HeaderText="Vendor Legal Entity Name" SortExpression="VendorLegalEntityName" />
                    <asp:TemplateField HeaderText="Request Date">
                        <ItemTemplate>
                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestDate", "{0:dd/MM/yyyy}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="BusinessAddressLine1" HeaderText="Business Address" SortExpression="BusinessAddressLine1" />
                    <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" SortExpression="PhoneNumber" />
                    <asp:BoundField DataField="TechnicalCapacity" HeaderText="Technical Capacity" SortExpression="TechnicalCapacity" />
                    <asp:ButtonField ButtonType="Button" CommandName="ViewItem" Text="View Attachments" />
                    <asp:CommandField ButtonType="Button" SelectText="Process Request" ShowSelectButton="True" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button runat="server" ID="btnStatus" Text="" BorderStyle="None" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle CssClass="FooterStyle" />
                <HeaderStyle CssClass="headerstyle" />
                <PagerStyle CssClass="PagerStyle" />
                <RowStyle CssClass="rowstyle" />
            </asp:GridView>
        </div>
        <div>
            <asp:Button runat="server" ID="btnInProgress" Text="" BorderStyle="None" BackColor="#FFFF6C" />
            <b>In Progress</b><br />
            <asp:Button runat="server" ID="btnComplete" Text="" BorderStyle="None" BackColor="#FF7251" />
            <b>Completed</b>

        </div>
        <br />
    </div>
    <asp:Panel ID="pnlApproval" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Process Vendor Request</h4>
                </div>
                <div class="modal-body no-padding">
                    <div class="smart-form">
                        <fieldset>
                            <div class="row">
                                <section class="col col-6">
                                    <asp:Panel ID="pnlSupplierType" runat="server" Visible="false">
                                        <asp:Label ID="lblSupplierType" runat="server" Text="Supplier Type" CssClass="label"></asp:Label>
                                        <label class="select">
                                            <asp:DropDownList ID="ddlSupplierType" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                                <asp:ListItem Value="0">-Supplier Type-</asp:ListItem>
                                            </asp:DropDownList><i></i>
                                            <asp:RequiredFieldValidator ID="rfvSupplierType" Enabled="false" runat="server" CssClass="validator" ControlToValidate="ddlSupplierType" ErrorMessage="Supplier Type Required" ValidationGroup="save" InitialValue="0"></asp:RequiredFieldValidator>
                                        </label>
                                    </asp:Panel>
                                </section>
                                <section class="col col-6">
                                    <asp:Label ID="lblApprovalStatus" runat="server" Text="Approval Status" CssClass="label"></asp:Label>
                                    <label class="select">
                                        <asp:DropDownList ID="ddlApprovalStatus" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlApprovalStatus_SelectedIndexChanged">
                                        </asp:DropDownList><i></i>
                                        <asp:RequiredFieldValidator ID="RfvApprovalStatus" runat="server" CssClass="validator" ValidationGroup="save" ErrorMessage="Approval Status Required" InitialValue="0" ControlToValidate="ddlApprovalStatus"></asp:RequiredFieldValidator>
                                    </label>
                                </section>
                            </div>
                            <div class="row">
                                <section class="col col-4">
                                    <asp:Label ID="lblRejectedReason" runat="server" Text="Rejected Reason" Visible="false" CssClass="label"></asp:Label>
                                    <label class="input">
                                        <asp:TextBox ID="txtRejectedReason" Visible="false" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvRejectedReason" runat="server" Enabled="false" CssClass="validator" ValidationGroup="save" ErrorMessage="Must Enter Rejection Reason" ControlToValidate="txtRejectedReason"></asp:RequiredFieldValidator>
                                    </label>
                                </section>
                            </div>
                        </fieldset>
                        <footer>
                            <asp:Button ID="btnApprove" runat="server" ValidationGroup="save" Text="Save" OnClick="btnApprove_Click" Enabled="false" CssClass="btn btn-primary"></asp:Button>
                            <asp:Button ID="btnCancelPopup" runat="server" Text="Close" CssClass="btn btn-primary" OnClick="btnCancelPopup_Click"></asp:Button>
                            <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-primary" Enabled="false" OnClientClick="javascript:Clickheretoprint('divprint')"></asp:Button>
                        </footer>
                    </div>
                </div>
            </div>
        </div>
        <!-- /.modal-content -->
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" BackgroundCssClass="modalBackground"
        Enabled="True" PopupControlID="pnlApproval" TargetControlID="btnPop"
        ID="pnlApproval_ModalPopupExtender">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlDetail" runat="server">
        <div class="modal-body no-padding">
            <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
                <header>
                    <span class="widget-icon"><i class="fa fa-edit"></i></span>
                    <h2>Vendor Attachments</h2>
                </header>
                <div>
                    <div class="jarviswidget-editbox"></div>
                    <div class="widget-body no-padding">
                        <div class="smart-form">
                            <asp:GridView ID="grvAttachments"
                                runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active">
                                <RowStyle CssClass="rowstyle" />
                                <Columns>
                                    <asp:BoundField DataField="FilePath" HeaderText="File Name" SortExpression="FilePath" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="downloadFile"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" Text="Delete" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="deleteFile" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle CssClass="FooterStyle" />
                                <HeaderStyle CssClass="headerstyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <RowStyle CssClass="rowstyle" />
                            </asp:GridView>
                            <footer>
                                <asp:Button ID="btnCancelPopup2" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" OnClick="btnCancelPopup2_Click"></asp:Button>
                            </footer>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" BackgroundCssClass="modalBackground"
        Enabled="True" TargetControlID="btnPop2" PopupControlID="pnlDetail" CancelControlID="btnCancelPopup2"
        ID="pnlDetail_ModalPopupExtender">
    </asp:ModalPopupExtender>
    <div id="divprint" style="display: none;">
        <fieldset>
            <table style="width: 100%;">
                <tr>
                    <td style="font-size: large; text-align: center;">
                        <img src="../img/CHAI%20Logo.png" width="100" height="100" />
                        <br />
                        <strong>CHAI ZIMBABWE
                                <br />
                            VENDOR REQUEST FORM</strong>
                    </td>
                </tr>
            </table>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 25%;">Request No</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblRequestNo" runat="server"></asp:Label></td>
                    <td style="width: 25%;">Request Date</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblRequestDate" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 25%;">Vendor Legal Entity Name</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblVendorLegalName" runat="server"></asp:Label>
                    </td>
                    <td style="width: 25%;">Tax No</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblTaxNo" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 25%;">Business Address Line One</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblBusinessAdd1" runat="server"></asp:Label>
                    </td>
                    <td style="width: 25%;">Contact Name</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblContactName" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 25%;">Phone Number</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblPhoneNo" runat="server"></asp:Label>
                    </td>
                    <td style="width: 25%;">Cell Number</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblCellNo" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 25%;">Fax Number</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblFaxNo" runat="server"></asp:Label></td>
                    <td style="width: 25%;">Website</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblWebsite" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 25%;">Email
                    </td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblEmail" runat="server"></asp:Label>
                    </td>
                    <td style="width: 25%;">Trade Ref</td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblTradeRef" runat="server"></asp:Label></td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grvStatuses" OnRowDataBound="grvStatuses_RowDataBound"
                runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                CssClass="table table-striped table-bordered table-hover">
                <RowStyle CssClass="rowstyle" />
                <Columns>
                    <asp:TemplateField HeaderText="Date">
                        <ItemTemplate>
                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Date", "{0:dd/MM/yyyy}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Name" />
                    <asp:BoundField DataField="AssignedBy" HeaderText="Assignee Approver" SortExpression="AssignedBy" />
                    <asp:BoundField HeaderText="Approval Status" DataField="ApprovalStatus" />
                </Columns>
                <FooterStyle CssClass="FooterStyle" />
                <HeaderStyle CssClass="headerstyle" />
                <PagerStyle CssClass="PagerStyle" />
                <RowStyle CssClass="rowstyle" />
            </asp:GridView>
            <br />
            <table style="width: 100%;">
                <tr>
                    <td></td>
                    <td>Signature</td>
                    <td></td>
                    <td></td>
                    <td style="text-align: right;">Recieved By </td>
                </tr>
                <tr>
                    <td></td>
                    <td>___________________</td>
                    <td></td>
                    <td></td>
                    <td style="text-align: right;">___________________</td>
                </tr>
            </table>
        </fieldset>
    </div>

</asp:Content>
