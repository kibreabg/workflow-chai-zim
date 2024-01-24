<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmInventory.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Setting.Views.frmInventory" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>


<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <asp:ValidationSummary ID="vsAddNew" runat="server"
        CssClass="alert alert-danger fade in" DisplayMode="SingleParagraph"
        ValidationGroup="2" ForeColor="" />
    <asp:ValidationSummary ID="vsUpdate" runat="server"
        CssClass="alert alert-danger fade in" DisplayMode="SingleParagraph"
        ValidationGroup="1" ForeColor="" />
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Inventory Items</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="lblItemName" runat="server" Text="Item Name" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtItemName" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="btnFind" runat="server" Text="Find" OnClick="btnFind_Click" CssClass="btn btn-primary"></asp:Button>
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>
        <asp:DataGrid ID="dgInventory" runat="server" AlternatingRowStyle-CssClass="" AutoGenerateColumns="False" CellPadding="0"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" DataKeyField="Id"
            GridLines="None"
            OnCancelCommand="dgInventory_CancelCommand" OnDeleteCommand="dgInventory_DeleteCommand" OnEditCommand="dgInventory_EditCommand"
            OnItemCommand="dgInventory_ItemCommand" OnItemDataBound="dgInventory_ItemDataBound" OnUpdateCommand="dgInventory_UpdateCommand"
            ShowFooter="True">

            <columns>
                <asp:TemplateColumn HeaderText="Item Name">
                    <itemtemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ItemName")%>
                    </itemtemplate>
                    <edititemtemplate>
                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "ItemName")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtItemName" runat="server" ControlToValidate="txtItemName" ErrorMessage="Inventory Item Name Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </edititemtemplate>
                    <footertemplate>
                        <asp:TextBox ID="txtFItemName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtFItemName" runat="server" CssClass="validator" ControlToValidate="txtFItemName" ErrorMessage="Inventory Item Name Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </footertemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Category">
                    <itemtemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Category")%>
                    </itemtemplate>
                    <edititemtemplate>
                        <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Category")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtCategory" runat="server" CssClass="validator" ControlToValidate="txtCategory" ErrorMessage="Category Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </edititemtemplate>
                    <footertemplate>
                        <asp:TextBox ID="txtFCategory" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtFCategory" runat="server" CssClass="validator" ControlToValidate="txtFCategory" ErrorMessage="Category Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </footertemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Unit">
                    <itemtemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Unit")%>
                    </itemtemplate>
                    <edititemtemplate>
                        <asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Unit")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtUnit" runat="server" CssClass="validator" ControlToValidate="txtUnit" ErrorMessage="Unit Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </edititemtemplate>
                    <footertemplate>
                        <asp:TextBox ID="txtFUnit" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtFUnit" runat="server" CssClass="validator" ControlToValidate="txtFUnit" ErrorMessage="Unit Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </footertemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Actions">
                    <edititemtemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" ValidationGroup="1" CssClass="btn btn-xs btn-default"><i class="fa fa-save"></i></asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default"><i class="fa fa-times"></i></asp:LinkButton>
                    </edititemtemplate>
                    <footertemplate>
                        <asp:LinkButton ID="lnkAddNew" runat="server" CommandName="AddNew" ValidationGroup="2" CssClass="btn btn-sm btn-success"><i class="fa fa-save"></i></asp:LinkButton>
                    </footertemplate>
                    <itemtemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-xs btn-default"><i class="fa fa-pencil"></i></asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default" OnClientClick="javascript:return confirm('Are you sure you want to delete this entry?');"><i class="fa fa-times"></i></asp:LinkButton>
                    </itemtemplate>
                </asp:TemplateColumn>
            </columns>
            <pagerstyle cssclass="paginate_button active" horizontalalign="Center" />
        </asp:DataGrid>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="menuContent" runat="Server">
</asp:Content>

