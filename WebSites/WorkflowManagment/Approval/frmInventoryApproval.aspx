<%@ Page Title="Inventory Request Approval Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmInventoryApproval.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Approval.Views.frmInventoryApproval" EnableEventValidation="false" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>


<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <script src="../js/libs/jquery-2.0.2.min.js"></script>
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

        function showApprovalModal() {
            $(document).ready(function () {
                $('#approvalModal').modal('show');
            });
        }
        function showDetailModal() {
            $(document).ready(function () {
                $('#detailModal').modal('show');
            });
        }

    </script>
    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Inventory Requests</h2>
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
                                    <asp:TextBox ID="txtSrchRequestNo" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchRequestDate" runat="server" Text="Request Date" CssClass="label"></asp:Label>
                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="txtSrchRequestDate" CssClass="form-control datepicker" data-dateformat="mm/dd/yy" runat="server"></asp:TextBox>
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
                        <asp:Button ID="btnpop" runat="server" />
                        <asp:Button ID="btnPop2" runat="server" />
                        <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-primary" OnClick="btnFind_Click"></asp:Button>
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>

                </div>
            </div>
            <div class="table-responsive" style="overflow: auto;">
                <asp:GridView ID="grvInventoryRequestList"
                    runat="server" AutoGenerateColumns="False" DataKeyNames="Id" OnRowCommand="grvInventoryRequestList_RowCommand"
                    OnRowDataBound="grvInventoryRequestList_RowDataBound" OnSelectedIndexChanged="grvInventoryRequestList_SelectedIndexChanged"
                    AllowPaging="True" OnPageIndexChanging="grvInventoryRequestList_PageIndexChanging"
                    CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" PageSize="30">
                    <RowStyle CssClass="rowstyle" />
                    <Columns>
                        <asp:BoundField DataField="RequestNo" HeaderText="Request No" SortExpression="RequestNo" />
                        <asp:BoundField DataField="Requester" HeaderText="Requester" SortExpression="Requester" />
                        <asp:TemplateField HeaderText="Request Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestedDate", "{0:dd/MM/yyyy}")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Required Date of Delivery">
                            <ItemTemplate>
                                <asp:Label ID="lblDateDelivery" runat="server" Text='<%# Eval("Requireddateofdelivery", "{0:dd/MM/yyyy}")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DeliverTo" HeaderText="Deliver To" SortExpression="DeliverTo" />
                        <asp:BoundField DataField="SpecialNeed" HeaderText="Special Need" SortExpression="SpecialNeed" />
                        <asp:BoundField DataField="PurposeOfRequest" HeaderText="Purpose of Request" SortExpression="PurposeOfRequest" />

                        <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn btn-default" CommandName="ViewItem" Text="View Item Detail" />
                        <asp:CommandField ButtonType="Button" ControlStyle-CssClass="btn btn-default" SelectText="Process Request" ShowSelectButton="True" />
                    </Columns>
                    <FooterStyle CssClass="FooterStyle" />
                    <HeaderStyle CssClass="headerstyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <RowStyle CssClass="rowstyle" />
                </asp:GridView>
            </div>
        </div>
        <div>
            <asp:Button runat="server" ID="btnInProgress" Text="" Enabled="false" BorderStyle="None" BackColor="#FFFF6C" />
            <b>In Progress</b><br />
            <asp:Button runat="server" ID="btnComplete" Text="" Enabled="false" BorderStyle="None" BackColor="#FF7251" />
            <b>Completed</b>
        </div>
        <br />
    </div>
    <div class="modal fade" id="approvalModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        &times;</button>
                    <h4 class="modal-title">Process Inventory Request</h4>
                </div>
                <div class="modal-body">
                    <div class="jarviswidget-editbox"></div>
                    <div class="widget-body no-padding">
                        <div class="smart-form">
                            <fieldset>
                                <div class="row">
                                    <section class="col col-6">
                                        <asp:Label ID="lblApprovalStatus" runat="server" Text="Approval Status" CssClass="label"></asp:Label>
                                        <label class="select">
                                            <asp:DropDownList ID="ddlApprovalStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlApprovalStatus_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select Status</asp:ListItem>
                                            </asp:DropDownList><i></i>
                                            <asp:RequiredFieldValidator ID="RfvApprovalStatus" runat="server" ValidationGroup="save" ErrorMessage="Approval Status Required" InitialValue="0" ControlToValidate="ddlApprovalStatus"></asp:RequiredFieldValidator>
                                        </label>
                                    </section>
                                    <section class="col col-6">
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
                                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-default" Enabled="false" OnClientClick="javascript:Clickheretoprint('divprint')"></asp:Button>
                            </footer>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="detailModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        &times;</button>
                    <h4 class="modal-title">Inventory Request Details</h4>
                </div>
                <div class="modal-body">
                    <div class="jarviswidget-editbox"></div>
                    <div class="widget-body no-padding">
                        <div class="tab-content">
                            <div class="tab-pane active" id="hr2">
                                <ul class="nav nav-tabs">
                                    <li class="active">
                                        <a href="#iss1" data-toggle="tab">Details</a>
                                    </li>
                                </ul>
                                <div class="tab-content padding-10">
                                    <div class="tab-pane active" id="iss1">
                                        <div class="smart-form">
                                            <div style="overflow-x: auto;">
                                                <asp:GridView ID="grvInventoryRequestDetails"
                                                    runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                                    CssClass="table table-striped table-bordered table-hover">
                                                    <RowStyle CssClass="rowstyle" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Inventory.ItemName" HeaderText="Item Name" SortExpression="Inventory.ItemName" />
                                                        <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" />
                                                    </Columns>
                                                    <FooterStyle CssClass="FooterStyle" />
                                                    <HeaderStyle CssClass="headerstyle" />
                                                    <PagerStyle CssClass="PagerStyle" />
                                                    <RowStyle CssClass="rowstyle" />
                                                </asp:GridView>
                                            </div>
                                            <footer>
                                                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                                            </footer>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="divprint" style="display: none; text-align: center;">
        <table style="width: 100%;">
            <tr>
                <td style="font-size: large; text-align: center;">
                    <img src="../img/CHAI%20Logo.png" width="70" height="70" />
                    <br />
                    <strong>CHAI ZIMBABWE
                            <br />
                        INVENTORY REQUEST FORM</strong></td>
            </tr>
        </table>
        <table style="width: 75%;">
            <tr>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblRequestNo" runat="server" Text="Request No:"></asp:Label>
                    </strong></td>
                <td style="width: 25%;">
                    <asp:Label ID="lblRequestNoResult" runat="server"></asp:Label>
                </td>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblRequestedDate" runat="server" Text="Requested Date:"></asp:Label>
                    </strong>
                </td>
                <td style="width: 25%;">
                    <asp:Label ID="lblRequestedDateResult" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 25%;"><strong>
                    <asp:Label ID="lblRequester" runat="server" Text="Requester:"></asp:Label>
                </strong></td>
                <td style="width: 25%;">
                    <asp:Label ID="lblRequesterResult" runat="server"></asp:Label></td>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblSuggestedSupplier" runat="server" Text="Suggested Supplier:"></asp:Label>
                    </strong>
                </td>
                <td style="width: 25%;">
                    <asp:Label ID="lblSuggestedSupplierResult" runat="server"></asp:Label>
                </td>
                <td style="width: 25%;">&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblSpecialNeed" runat="server" Text="Special Need:"></asp:Label>
                    </strong></td>
                <td style="width: 25%;">
                    <asp:Label ID="lblSpecialNeedResult" runat="server"></asp:Label>
                </td>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblDeliverTo" runat="server" Text="Deliver To:"></asp:Label>
                    </strong>
                </td>
                <td style="width: 25%;">
                    <asp:Label ID="lblDelivertoResult" runat="server"></asp:Label>
                </td>
            </tr>

            <tr>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblPaymentMeth" runat="server" Text="Payment Method:"></asp:Label>
                    </strong></td>
                <td style="width: 25%;">
                    <asp:Label ID="lblPayMethRes" runat="server"></asp:Label>
                </td>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblReqDate" runat="server" Text="Required Date Of Delivery:"></asp:Label>
                    </strong>
                </td>
                <td style="width: 25%;">
                    <asp:Label ID="lblReqDateResult" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 25%;">
                    <strong>
                        <asp:Label ID="lblSpec" runat="server" Text="Purpose of Request/Activity:"></asp:Label>
                    </strong></td>
                <td style="width: 25%;">
                    <asp:Label ID="lblSpecRes" runat="server"></asp:Label>
                </td>

            </tr>
        </table>
        <br />
        <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            CssClass="table table-striped table-bordered table-hover">
            <RowStyle CssClass="rowstyle" />
            <Columns>
                <asp:BoundField DataField="Inventory.ItemName" HeaderText="Item Name" SortExpression="Inventory.ItemName" />
                <asp:BoundField DataField="Qty" HeaderText="Quantity" SortExpression="Qty" />
            </Columns>
            <FooterStyle CssClass="FooterStyle" />
            <HeaderStyle CssClass="headerstyle" />
            <PagerStyle CssClass="PagerStyle" />
            <RowStyle CssClass="rowstyle" />
        </asp:GridView>
        <br />
        <br />
        <asp:GridView ID="grvStatuses" CellPadding="5" CellSpacing="3" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            CssClass="table table-striped table-bordered table-hover" OnRowDataBound="grvStatuses_RowDataBound">
            <RowStyle CssClass="rowstyle" />
            <Columns>
                <asp:TemplateField HeaderText="Date">
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("ApprovalDate", "{0:dd/MM/yyyy}")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="Approver" />
                <asp:BoundField DataField="AssignedBy" HeaderText="Assignee Approver" SortExpression="AssignedBy" />
                <asp:BoundField DataField="ApprovalStatus" HeaderText="Approval Status" SortExpression="ApprovalStatus" />

            </Columns>
            <FooterStyle CssClass="FooterStyle" />
            <HeaderStyle CssClass="headerstyle" />
            <PagerStyle CssClass="PagerStyle" />
            <RowStyle CssClass="rowstyle" />
        </asp:GridView>
    </div>

</asp:Content>
