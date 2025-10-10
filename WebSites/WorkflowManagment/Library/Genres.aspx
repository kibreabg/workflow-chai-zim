<%@ Page Title="Genre Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true"
    CodeFile="Genres.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Library.Views.Genres" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="DefaultContent" runat="Server">
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Search Genres</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="LblName" runat="server" Text="Genre Name" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="TxtSrchName" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="BtnFind" runat="server" Text="Find" OnClick="BtnFind_Click" CssClass="btn btn-primary"></asp:Button>
                        <asp:Button ID="BtnClosepage" runat="server" Text="Close" data-dismiss="modal" CssClass="btn btn-primary" PostBackUrl="../Default.aspx"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>

        <asp:DataGrid ID="DgGenre" runat="server" AlternatingRowStyle-CssClass="" AutoGenerateColumns="False" CellPadding="0"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" DataKeyField="Id"
            GridLines="None"
            OnCancelCommand="DgGenre_CancelCommand" OnDeleteCommand="DgGenre_DeleteCommand" OnEditCommand="DgGenre_EditCommand"
            OnItemCommand="DgGenre_ItemCommand" OnItemDataBound="DgGenre_ItemDataBound" OnUpdateCommand="DgGenre_UpdateCommand"
            ShowFooter="True">

            <Columns>
                <asp:TemplateColumn HeaderText="Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Name")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TxtEdtName" runat="server" CssClass="form-control" Text=' <%# DataBinder.Eval(Container.DataItem, "Name")%>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RfvTxtEdtName" runat="server" CssClass="validator"
                            ControlToValidate="TxtEdtName" ErrorMessage="Genre Name is Required" ValidationGroup="edit"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="TxtName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RfvTxtName" runat="server" CssClass="validator"
                            ControlToValidate="TxtName" ErrorMessage="Genre Name is Required" ValidationGroup="save"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Actions">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LnkUpdate" runat="server" CommandName="Update" ValidationGroup="edit" CssClass="btn btn-xs btn-default"><i class="fa fa-save"></i></asp:LinkButton>
                        <asp:LinkButton ID="LnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default"><i class="fa fa-times"></i></asp:LinkButton>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="LnkAddNew" runat="server" CommandName="AddNew" ValidationGroup="save" CssClass="btn btn-sm btn-success"><i class="fa fa-save"></i></asp:LinkButton>
                    </FooterTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-xs btn-default"><i class="fa fa-pencil"></i></asp:LinkButton>
                        <asp:LinkButton ID="LnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-default" OnClientClick="javascript:return confirm('Are you sure you want to delete this entry?');"><i class="fa fa-times"></i></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <PagerStyle CssClass="paginate_button active" HorizontalAlign="Center" />
        </asp:DataGrid>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="menuContent" runat="Server">
</asp:Content>
