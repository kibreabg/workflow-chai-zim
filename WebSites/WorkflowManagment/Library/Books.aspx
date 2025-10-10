<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Books.aspx.cs" 
    Inherits="Chai.WorkflowManagment.Modules.Library.Views.BookList"
    Title="Default" MasterPageFile="~/Shared/ModuleMaster.master" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>

<asp:Content ID="content" ContentPlaceHolderID="DefaultContent" runat="Server">
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Book List</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-3">
                                <asp:Label ID="lblSrchAuthors" runat="server" Text="Author" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlSrchAuthors" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="Id">
                                        <asp:ListItem Value="">Select Author</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchGenres" runat="server" Text="Genre" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlSrchGenres" runat="server" AppendDataBoundItems="True" DataTextField="Name" DataValueField="Id">
                                        <asp:ListItem Value="">Select Genre</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchTitle" runat="server" Text="Title" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:TextBox ID="txtSrchTitle" runat="server" CssClass="form-control" placeholder="Enter Keyword"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-primary" OnClick="BtnFind_Click"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>

        <asp:GridView ID="GrvBookList" runat="server" AutoGenerateColumns="False" CellPadding="3"
            DataKeyNames="Id" EnableModelValidation="True" ForeColor="#333333" GridLines="Horizontal"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active"
            AlternatingRowStyle-CssClass="" OnRowDataBound="GrvBookList_RowDataBound" Width="100%"
            Style="text-align: left" AllowPaging="True" OnPageIndexChanging="GrvBookList_PageIndexChanging"
            OnSelectedIndexChanged="GrvBookList_SelectedIndexChanged" OnRowCommand="GrvBookList_RowCommand"
            Visible="True" PageSize="20">
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="ISBN" HeaderText="ISBN" />
                <asp:BoundField DataField="PublishedYear" HeaderText="Published Year" />
                <asp:BoundField DataField="CopiesAvailable" HeaderText="Copies Available" />
                <asp:BoundField DataField="Author.Name" HeaderText="Author" />
                <asp:BoundField DataField="Genre.Name" HeaderText="Genre" />
                <asp:CommandField ShowSelectButton="True" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button runat="server" CommandName="Loan" Text="Loan"
                            CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-success btn-sm" />
                        <asp:Button runat="server" CommandName="Reserve" Text="Reserve"
                            CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-info btn-sm" />
                        <asp:Button runat="server" CommandName="Review" Text="Review"
                            CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-warning btn-sm" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle CssClass="rowstyle" />
            <PagerStyle CssClass="paginate_button active" HorizontalAlign="Center" />
        </asp:GridView>
    </div>
</asp:Content>
