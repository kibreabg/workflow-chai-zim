<%@ Page Title="Sole Vendor Request Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmSoleVendorRequest.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Request.Views.frmSoleVendorRequest" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="content" ContentPlaceHolderID="DefaultContent" runat="Server">
    <script src="../js/libs/jquery-2.0.2.min.js"></script>
    <script type="text/javascript">
        function showSearch() {
            $(document).ready(function () {
                $('#searchModal').modal('show');
            });
        }
    </script>

    <div class="jarviswidget jarviswidget-sortable">
        <header role="heading">
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Sole Vendor Request</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <%--<section class="col col-6">
                                <label class="label">Request Number</label>
                                <label class="input">
                                    <asp:TextBox ID="txtRequestNo" runat="server" ReadOnly="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvtxtRequestNo" runat="server" ErrorMessage="Request number is required" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="save" SetFocusOnError="true" ControlToValidate="txtRequestNo"></asp:RequiredFieldValidator>
                                </label>
                            </section>--%>
                            <section class="col col-6">
                                <label class="label">Request Date</label>
                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="txtRequestDate" ReadOnly="true" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Proposed Purchased Price</label>
                                <label class="input">
                                    <asp:TextBox ID="txtProposedPurchasedPrice" runat="server"></asp:TextBox>
                                    <cc1:FilteredTextBoxExtender runat="server" Enabled="True" TargetControlID="txtProposedPurchasedPrice" ID="FilteredTextBoxExtender2" FilterType="Numbers"></cc1:FilteredTextBoxExtender>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Proposed Supplier</label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlSupplier" runat="server" DataValueField="Id" DataTextField="SupplierName" AppendDataBoundItems="True">
                                        <asp:ListItem Value="">--Select Supplier--</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                                <asp:RequiredFieldValidator ID="rfvPayee" runat="server" ControlToValidate="ddlSupplier" CssClass="validator" Display="Dynamic" ErrorMessage="Supplier Required" SetFocusOnError="true" ValidationGroup="save"></asp:RequiredFieldValidator>
                            </section>
                            <section class="col col-6">
                                <label class="label">Contact Person & Number</label>
                                <label class="input">
                                    <asp:TextBox ID="txtContactPersonNumber" runat="server"></asp:TextBox>
                                </label>
                                <asp:RequiredFieldValidator ID="Rfvneededfor" runat="server" CssClass="validator" ControlToValidate="txtContactPersonNumber" ErrorMessage="Enter Contact Person and Number" InitialValue="" SetFocusOnError="True" ValidationGroup="save"></asp:RequiredFieldValidator>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Project</label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlProject" runat="server" DataValueField="Id" DataTextField="ProjectCode" Enabled="false">
                                    </asp:DropDownList><i></i>
                                </label>
                                <asp:RequiredFieldValidator
                                    ID="rfvddlProject" runat="server" ErrorMessage="Project is required" Display="Dynamic"
                                    CssClass="validator" ValidationGroup="save" InitialValue="0"
                                    SetFocusOnError="true" ControlToValidate="ddlProject"></asp:RequiredFieldValidator>
                            </section>
                            <section class="col col-6">
                                <label class="label">Grant</label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlGrant" runat="server" DataValueField="Id" DataTextField="GrantCode" Enabled="false">
                                    </asp:DropDownList><i></i>
                                </label>
                                <asp:RequiredFieldValidator
                                    ID="rfvGrant" runat="server" ErrorMessage="Grant is required" Display="Dynamic"
                                    CssClass="validator" ValidationGroup="saveMain" InitialValue="0"
                                    SetFocusOnError="true" ControlToValidate="ddlGrant"></asp:RequiredFieldValidator>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Sole Source Justification PreparedBy</label>
                                <label class="input">
                                    <asp:TextBox ID="txtSoleSource" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Sole Vendor Justification Type</label>
                                <label class="select">
                                    <asp:DropDownList runat="server" ID="ddlSoleVendorJustification">
                                        <asp:ListItem Value="">--Select Sole Vendor Justification--</asp:ListItem>
                                        <asp:ListItem>One Supplier</asp:ListItem>
                                        <asp:ListItem>One-of-a-kind</asp:ListItem>
                                        <asp:ListItem>Compatibility</asp:ListItem>
                                        <asp:ListItem>Replacement Part</asp:ListItem>
                                        <asp:ListItem>Research Continuity</asp:ListItem>
                                        <asp:ListItem>CHAI standards</asp:ListItem>
                                        <asp:ListItem>Unique design</asp:ListItem>
                                        <asp:ListItem>Other</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                                <asp:RequiredFieldValidator ID="rfvSoleJust" runat="server" ControlToValidate="ddlSoleVendorJustification" CssClass="validator" InitialValue="" Display="Dynamic" ErrorMessage="Enter Sole Vendor Justification" SetFocusOnError="true" ValidationGroup="save"></asp:RequiredFieldValidator>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Commodity / Service Required</label>
                                <label class="input">
                                    <asp:TextBox ID="txtItems" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Comment</label>
                                <label class="input">
                                    <asp:TextBox ID="txtComment" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
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
                                            <a href="#iss1" data-toggle="tab">Sole Vendor Item Detail</a>
                                        </li>
                                        <li class="">
                                            <a href="#iss2" data-toggle="tab">Attachments</a>
                                        </li>
                                    </ul>
                                    <div class="tab-content padding-10">
                                        <div class="tab-pane active" id="iss1">
                                            <asp:DataGrid ID="dgItemDetail" runat="server" AlternatingRowStyle-CssClass="" AutoGenerateColumns="False" CellPadding="0"
                                                CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" DataKeyField="Id"
                                                GridLines="None" OnItemDataBound="dgItemDetail_ItemDataBound" ShowFooter="True" OnDeleteCommand="dgItemDetail_DeleteCommand" OnItemCommand="dgItemDetail_ItemCommand" OnPageIndexChanged="dgItemDetail_PageIndexChanged" OnSelectedIndexChanged="dgItemDetail_SelectedIndexChanged" OnUpdateCommand="dgItemDetail_UpdateCommand">
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="Requested Items">
                                                        <EditItemTemplate>
                                                            <asp:DropDownList ID="ddlItemAcc" runat="server" CssClass="form-control"
                                                                AppendDataBoundItems="True" DataTextField="AccountName" DataValueField="Id"
                                                                AutoPostBack="True">
                                                                <asp:ListItem Value="0">Select Item Account</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="RfvItemAcc" runat="server" CssClass="validator"
                                                                ControlToValidate="ddlItemAcc" ErrorMessage="Item Account Required"
                                                                InitialValue="0" SetFocusOnError="True" ValidationGroup="proedit">*</asp:RequiredFieldValidator>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:DropDownList ID="ddlFItemAcc" runat="server" CssClass="form-control"
                                                                AppendDataBoundItems="True" DataTextField="AccountName" DataValueField="Id"
                                                                EnableViewState="true" AutoPostBack="True">
                                                                <asp:ListItem Value="0">Select Item Account</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="RfvFItemAcc" runat="server" CssClass="validator"
                                                                ControlToValidate="ddlFItemAcc" Display="Dynamic"
                                                                ErrorMessage="Item Account Required" InitialValue="0" SetFocusOnError="True"
                                                                ValidationGroup="proadd"></asp:RequiredFieldValidator>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "ItemAccount.AccountName")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Item Description">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "ItemDescription")%>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "ItemDescription")%>'></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RfvDescription" runat="server" CssClass="validator" ControlToValidate="txtDescription" ErrorMessage="Item Description Required" ValidationGroup="proedit">*</asp:RequiredFieldValidator>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtFDescription" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RfvFDescription" runat="server" CssClass="validator" ControlToValidate="txtFDescription" ErrorMessage="Item Description Required" ValidationGroup="proadd">*</asp:RequiredFieldValidator>
                                                        </FooterTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Qty">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "Qty")%>
                                                            <asp:HiddenField ID="hfqty" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Qty")%>'></asp:HiddenField>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtEdtQty" Enabled="false" runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Quantity")%>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtQty" runat="server" Enabled="true" CssClass="form-control"></asp:TextBox>

                                                        </FooterTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Unit Cost">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "UnitCost")%>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtEdtUnitCost" runat="server" CssClass="form-control" AutoPostBack="true" Text='<%# DataBinder.Eval(Container.DataItem, "UnitPrice")%>' OnTextChanged="txtEdtUnitCost_TextChanged"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtUnitCost" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtUnitCost_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Total Cost">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "TotalCost")%>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtEdtTotalCost" runat="server" Enabled="false" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "TotalPrice")%>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtTotalCost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                        </FooterTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Actions">
                                                        <EditItemTemplate>
                                                            <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" ValidationGroup="proedit" CssClass="btn btn-xs btn-default"><i class="fa fa-save"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default"><i class="fa fa-times"></i></asp:LinkButton>
                                                        </EditItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:LinkButton ID="lnkAddNew" runat="server" CommandName="AddNew" ValidationGroup="proadd" CssClass="btn btn-sm btn-success"><i class="fa fa-save"></i></asp:LinkButton>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-xs btn-default"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default" OnClientClick="javascript:return confirm('Are you sure you want to delete this entry?');"><i class="fa fa-times"></i></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                                <PagerStyle CssClass="paginate_button active" HorizontalAlign="Center" />
                                            </asp:DataGrid>
                                        </div>

                                        <div class="tab-pane" id="iss2">
                                            <fieldset>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="label">Attachments</label>
                                                        <asp:FileUpload ID="fuReciept" runat="server" />
                                                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
                                                    </section>
                                                </div>

                                                <asp:GridView ID="grvAttachments"
                                                    runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                                    CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active">
                                                    <RowStyle CssClass="rowstyle" />
                                                    <Columns>
                                                        <asp:BoundField DataField="FilePath" HeaderText="File Name" SortExpression="FilePath" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDelete" Text="Delete" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="DeleteFile" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <FooterStyle CssClass="FooterStyle" />
                                                    <HeaderStyle CssClass="headerstyle" />
                                                    <PagerStyle CssClass="PagerStyle" />
                                                    <RowStyle CssClass="rowstyle" />
                                                </asp:GridView>
                                            </fieldset>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <footer>
                        <asp:Button ID="btnSave" runat="server" Text="Request" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="save" CssClass="btn btn-primary"></asp:Button>
                        <%--<asp:Button ID="btnSearch" data-toggle="modal" href="#myModal" Text="Search" CssClass="btn btn-primary"></asp:Button>--%>
                        <a data-toggle="modal" runat="server" id="searchLink" href="#searchModal" class="btn btn-primary"><i class="fa fa-circle-arrow-up fa-lg"></i>Search</a>
                        <asp:Button ID="btnDelete" runat="server" CausesValidation="False" CssClass="btn btn-primary" Text="Delete" OnClick="btnDelete_Click"></asp:Button>
                        <cc1:ConfirmButtonExtender ID="btnDelete_ConfirmButtonExtender" runat="server"
                            ConfirmText="Are you sure you want to delete this record?" Enabled="True" TargetControlID="btnDelete">
                        </cc1:ConfirmButtonExtender>
                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-primary" OnClick="btnCancel_Click" Text="New" />
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>
    </div>

    <%--<cc1:ModalPopupExtender runat="server" DynamicServicePath="" PopupControlID="myModal"
        Enabled="True" TargetControlID="btnSearch" CancelControlID="btnCancelSearch" BackgroundCssClass="modalBackground"
        ID="pnlSearch_ModalPopupExtender">
    </cc1:ModalPopupExtender>--%>
    <%--<asp:Panel ID="pnlSearch" runat="server">--%>

    <%-- </asp:Panel>--%>

    <div class="modal fade" id="searchModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="myModalLabel">Search Vehicle Requests</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="txtSrchRequestNo">Request Number</label>
                                <asp:TextBox ID="txtSrchRequestNo" CssClass="form-control" ToolTip="Request No" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="txtSrchRequestDate">Requested Date</label>
                                <label class="input" style="position: relative; display: block; font-weight: 400;">
                                    <i class="icon-append fa fa-calendar" style="position: absolute; top: 5px; width: 22px; height: 22px; font-size: 14px; line-height: 22px; text-align: center; right: 5px; padding-left: 3px; border-left-width: 1px; border-left-style: solid; color: #A2A2A2;"></i>
                                    <asp:TextBox ID="txtSrchRequestDate" CssClass="form-control datepicker"
                                        data-dateformat="mm/dd/yy" ToolTip="Request Date" runat="server"></asp:TextBox>
                                </label>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="text-align: right;">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Button ID="btnFind" runat="server" OnClick="btnFind_Click" Text="Find" CssClass="btn btn-primary"></asp:Button>
                                <asp:Button ID="btnCancelSearch" Text="Close" runat="server" class="btn btn-primary"></asp:Button>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <div class="well well-sm well-primary">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
    <asp:Panel ID="pnlWarning" Visible="false" Style="position: absolute; top: 55px; left: 108px;" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                </div>
                <div class="modal-body no-padding">
                    <div class="jarviswidget" data-widget-editbutton="false" data-widget-custombutton="false">
                        <div>
                            <div class="jarviswidget-editbox"></div>
                            <div class="widget-body no-padding">
                                <div class="smart-form">
                                    <div class="alert alert-block alert-danger">
                                        <h4 class="alert-heading">Warning</h4>
                                        <p>
                                            The current Request has no Approval Settings defined. Please contact your administrator
                                        </p>
                                    </div>
                                    <footer>
                                        <asp:Button ID="btnCancelPopup" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancelPopup_Click"></asp:Button>
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

</asp:Content>
