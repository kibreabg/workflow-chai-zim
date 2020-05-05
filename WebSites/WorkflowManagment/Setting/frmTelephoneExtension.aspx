<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmTelephoneExtension.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Setting.Views.frmTelephoneExtension" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>


<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <asp:ValidationSummary ID="NewValidationSummary1" runat="server"
        CssClass="alert alert-danger fade in" DisplayMode="SingleParagraph"
        ValidationGroup="2" ForeColor="" />
    <asp:ValidationSummary ID="EditValidationSummary2" runat="server"
        CssClass="alert alert-danger fade in" DisplayMode="SingleParagraph"
        ValidationGroup="1" ForeColor="" />
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Telephone Extension</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="lblName" runat="server" Text="Name" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox></label>
                            </section>
                            <section class="col col-6">
                                <asp:Label ID="lblExt" runat="server" Text="Extension" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtExtension" runat="server"></asp:TextBox>
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
        <asp:DataGrid ID="dgTelExt" runat="server" AlternatingRowStyle-CssClass="" AutoGenerateColumns="False" CellPadding="0"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" DataKeyField="Id"
            GridLines="None"
            OnCancelCommand="dgTelExt_CancelCommand" OnDeleteCommand="dgTelExt_DeleteCommand" OnEditCommand="dgTelExt_EditCommand"
            OnItemCommand="dgTelExt_ItemCommand" OnItemDataBound="dgTelExt_ItemDataBound" OnUpdateCommand="dgTelExt_UpdateCommand"
            ShowFooter="True">

            <Columns>
                <asp:TemplateColumn HeaderText="Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Name")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Name")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="validator" ControlToValidate="txtName" ErrorMessage="Name Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtFName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="validator" ControlToValidate="txtFName" ErrorMessage="Name Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Extension">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Extension")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtExtension" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Extension")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" CssClass="validator" ControlToValidate="txtExtension" ErrorMessage="Extension Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtFExtension" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorcode" runat="server" CssClass="validator" ControlToValidate="txtFExtension" ErrorMessage="Extension Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Cellphone">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Cellphone")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtCellphone" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Cellphone")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatordonoredt" runat="server" ControlToValidate="txtCellphone" ErrorMessage="Cellphone Required" ValidationGroup="1">*</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtFCellphone" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatordonor" runat="server" ControlToValidate="txtFCellphone" ErrorMessage="Cellphone Required" ValidationGroup="2">*</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Actions">
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" ValidationGroup="1" CssClass="btn btn-xs btn-default"><i class="fa fa-save"></i></asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default"><i class="fa fa-times"></i></asp:LinkButton>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="lnkAddNew" runat="server" CommandName="AddNew" ValidationGroup="2" CssClass="btn btn-sm btn-success"><i class="fa fa-save"></i></asp:LinkButton>
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

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="menuContent" runat="Server">
</asp:Content>

