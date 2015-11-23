<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="frmPurchaseApproval.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Approval.Views.frmPurchaseApproval" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Purchase Approval</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-3">
                                <asp:Label ID="lblRequestNosearch" runat="server" Text="Request No" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtRequestNosearch" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblRequestDatesearch" runat="server" Text="Request Date" CssClass="label"></asp:Label>

                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="txtRequestDatesearch" CssClass="form-control datepicker" data-dateformat="mm/dd/yy" runat="server" Visible="true"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="label"></asp:Label>

                                <label class="select">
                                    <asp:DropDownList ID="ddlProgressStatus" runat="server"></asp:DropDownList><i></i>

                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="btnPop" runat="server" />
                         <asp:Button ID="btnPop2" runat="server" />
                        <asp:Button ID="btnFind" runat="server" Text="Find" OnClick="btnFind_Click" CssClass="btn btn-primary"></asp:Button>
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>

        <asp:GridView ID="grvPurchaseRequestList"
            runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
            OnRowDataBound="grvPurchaseRequestList_RowDataBound" OnRowDeleting="grvPurchaseRequestList_RowDeleting"
            OnSelectedIndexChanged="grvPurchaseRequestList_SelectedIndexChanged" AllowPaging="True" OnPageIndexChanging="grvPurchaseRequestList_PageIndexChanging"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" OnRowCommand="grvPurchaseRequestList_RowCommand">
            <RowStyle CssClass="rowstyle" />
            <Columns>
                <asp:BoundField DataField="RequestNo" HeaderText="Request No" SortExpression="RequestNo" />
                <asp:BoundField HeaderText="Requester" />
                <asp:BoundField DataField="RequestedDate" HeaderText="Request Date" SortExpression="RequestedDate" />
                <asp:BoundField DataField="Requireddateofdelivery" HeaderText="Required Date of Delivery" SortExpression="Requireddateofdelivery" />
                <asp:BoundField DataField="SuggestedSupplier" HeaderText="Suggested Supplier" SortExpression="SuggestedSupplier" />
                <asp:BoundField DataField="Neededfor" HeaderText="Needed for" SortExpression="Neededfor" />
                <asp:BoundField DataField="TotalPrice" HeaderText="Total Price" SortExpression="TotalPrice" />
                <asp:ButtonField ButtonType="Button" CommandName="ViewItem" Text="View Item Detail" />
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

        </asp:GridView>
        <div>
            <asp:Button runat="server" ID="btnInProgress" Text="" BorderStyle="None"  BackColor="#FFFF6C"/>  <B>In Progress</B><br />
            <asp:Button runat="server" ID="btnComplete" Text="" BorderStyle="None" BackColor="#FF7251"/>  <B>Completed</B><br />
            <asp:Button runat="server" ID="btnAuthorized" Text="" BorderStyle="None" BackColor="#112552"/>  <B>Authorized</B>

        </div>
        <br />
        <br />

        
       

    </div>
    <asp:Panel ID="pnlDetail" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                </div>
                <div class="modal-body no-padding">
                    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
                        <header>
                            <span class="widget-icon"><i class="fa fa-edit"></i></span>
                            <h2>Requested Item Detail</h2>
                        </header>
                        <div>
                            <div class="jarviswidget-editbox"></div>
                            <div class="widget-body no-padding">
                                <div class="smart-form">

                                    <asp:DataGrid ID="dgPurchaseRequestDetail" runat="server" AlternatingRowStyle-CssClass="" AutoGenerateColumns="False" CellPadding="0" CssClass="table table-striped table-bordered table-hover" DataKeyField="Id" GridLines="None" OnItemDataBound="dgPurchaseRequestDetail_ItemDataBound" PagerStyle-CssClass="paginate_button active" ShowFooter="True">
                                        <Columns>
                                            <asp:TemplateColumn HeaderText="Account Description">


                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "ItemAccount.AccountName")%>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Account Code">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "AccountCode")%>
                                                </ItemTemplate>


                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Qty">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "Qty")%>
                                                </ItemTemplate>


                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Price per unit">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "Priceperunit")%>
                                                </ItemTemplate>


                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Estimated Cost">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "EstimatedCost")%>
                                                </ItemTemplate>


                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Project">


                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container.DataItem, "Project.ProjectCode")%>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>


                                        </Columns>
                                        <PagerStyle CssClass="paginate_button active" HorizontalAlign="Center" />
                                    </asp:DataGrid>


                                    <footer>

                                        <asp:Button ID="btnCancelPopup2" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" OnClick="btnCancelPopup2_Click"></asp:Button>

                                    </footer>
                                </div>
                            </div>
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
    <asp:Panel ID="pnlApproval" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                </div>
                <div class="modal-body no-padding">
                    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
                        <header>
                            <span class="widget-icon"><i class="fa fa-edit"></i></span>
                            <h2>Process Request</h2>
                        </header>
                        <div>
                            <div class="jarviswidget-editbox"></div>
                            <div class="widget-body no-padding">
                                <div class="smart-form">
                                    <fieldset>

                                        <div class="row">
                                            <asp:Panel ID="pnlInfo" runat="server" Visible="false">
            <div class="alert alert-info fade in">
                
                <i class="fa-fw fa fa-info"></i>
                <strong>Info!</strong> Please perform Bid Analysis before Approving Bid.</div>
        </asp:Panel>
                                            <section class="col col-6">
                                                <asp:Label ID="lblApprovalStatus" runat="server" Text="Approval Status" CssClass="label"></asp:Label>

                                                <label class="select">
                                                    <asp:DropDownList ID="ddlApprovalStatus" runat="server" OnSelectedIndexChanged="ddlApprovalStatus_SelectedIndexChanged" AutoPostBack="True">
                                                    </asp:DropDownList><i></i>
                                                    <asp:RequiredFieldValidator ID="RfvApprovalStatus" CssClass="validator" runat="server" ErrorMessage="Approval Status Required" InitialValue="0" ControlToValidate="ddlApprovalStatus" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </label>
                                            </section>
                                            <section class="col col-6">
                                                <asp:Label ID="lblRejectedReason" runat="server" Text="Rejected Reason" Visible="false" CssClass="label"></asp:Label>

                                                <label class="textarea">
                                                    <asp:TextBox ID="txtRejectedReason" runat="server" Visible="false" TextMode="MultiLine"></asp:TextBox>
                                                </label>
                                            </section>
                                            <asp:LinkButton ID="lnkbidanalysis" runat="server" Text="Prepare Bid Analysis" OnClick="lnkbidanalysis_Click"></asp:LinkButton>
                                        </div>
                                        <div class="row">
                                            <div class="row">
                                            <section class="col col-12">
                                                <asp:Label ID="lblAttachments" runat="server" Text="Attachments" CssClass="label"></asp:Label>
                                                <asp:GridView ID="grvAttachments"
                                                    runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                                    CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active">
                                                    <RowStyle CssClass="rowstyle" />
                                                    <Columns>
                                                        <asp:BoundField DataField="FilePath" HeaderText="File Name" SortExpression="FilePath" />
                                                         <asp:TemplateField>
                                                     <ItemTemplate>
                                                       <asp:LinkButton ID="lnkDownload" Text = "Download" CommandArgument = '<%# Eval("FilePath") %>' runat="server" OnClick = "DownloadFile"></asp:LinkButton>
                                                     </ItemTemplate>
                                                   </asp:TemplateField>
                                                    </Columns>
                                                    <FooterStyle CssClass="FooterStyle" />
                                                    <HeaderStyle CssClass="headerstyle" />
                                                    <PagerStyle CssClass="PagerStyle" />
                                                    <RowStyle CssClass="rowstyle" />
                                                </asp:GridView>
                                            </section>
                                        </div>
                                        </div>
                                    </fieldset>
                                    <footer>
                                        <asp:Button ID="btnApprove" runat="server" Text="Save" OnClick="btnApprove_Click" Enabled="true" CssClass="btn btn-primary" ValidationGroup="Save"></asp:Button>
                                        <asp:Button ID="btnCancelPopup" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" OnClick="btnCancelPopup_Click"></asp:Button>
                                        <asp:Button ID="btnPurchaseOrder" runat="server" Text="Purchase Order" CssClass="btn btn-primary" Visible="false" OnClick="btnPurchaseOrder_Click"></asp:Button>


                                    </footer>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>







            </div>
        </div>
        <!-- /.modal-content -->
    </asp:Panel>
     <asp:ModalPopupExtender runat="server" BackgroundCssClass="modalBackground"
        Enabled="True" TargetControlID="btnPop" PopupControlID="pnlApproval" CancelControlID="btnCancelPopup"
        ID="pnlApproval_ModalPopupExtender">
    </asp:ModalPopupExtender>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="menuContent" runat="Server">
</asp:Content>

