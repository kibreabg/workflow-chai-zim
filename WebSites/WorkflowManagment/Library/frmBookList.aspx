<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmBookList.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Library.Views.BookList"
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
                                <asp:Label ID="lblSrchAuthor" runat="server" Text="Author" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtAuthor" runat="server" Visible="true"></asp:TextBox>
                                </label>
                            </section>
                            <%--<section class="col col-3">
                                <asp:Label ID="lblSrchFullName" runat="server" Text="Full Name" CssClass="label"></asp:Label>
                                <label class="input">
                                    <asp:TextBox ID="txtSrchSrchFullName" runat="server" Visible="true"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblSrchProgram" runat="server" Text="Project" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlSrchSrchProgram" runat="server" AppendDataBoundItems="True" DataTextField="ProgramName" DataValueField="Id">
                                        <asp:ListItem Value="0">Select Program</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                            </section>
                            <section class="col col-3">
                                <asp:Label ID="lblEmpStatus" runat="server" Text="Book Status" CssClass="label"></asp:Label>
                                <label class="select">
                                    <asp:DropDownList ID="ddlEmpStatus" runat="server" AppendDataBoundItems="True">
                                        <asp:ListItem Value="True">Active</asp:ListItem>
                                        <asp:ListItem Value="False">In Active</asp:ListItem>
                                    </asp:DropDownList><i></i>
                                </label>
                            </section>--%>
                        </div>

                    </fieldset>
                    <footer>
                        <asp:Button ID="btnFind" runat="server" Text="Find" CssClass="btn btn-primary" OnClick="BtnFind_Click"></asp:Button>
                        <asp:Button ID="btnClosepage" runat="server" Text="Close" CssClass="btn btn-primary"></asp:Button>
                    </footer>
                </div>
            </div>
        </div>

        <asp:GridView ID="GrvBookList" runat="server" AutoGenerateColumns="False" CellPadding="3"
            DataKeyNames="Id" EnableModelValidation="True" ForeColor="#333333" GridLines="Horizontal"
            CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active"
            AlternatingRowStyle-CssClass="" OnRowDataBound="GrvBookList_RowDataBound" Width="100%"
            Style="text-align: left" AllowPaging="True" OnPageIndexChanging="GrvBookList_PageIndexChanging"
            Visible="True" PageSize="20">
            <Columns>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                <asp:BoundField HeaderText="Program" />
                <asp:BoundField HeaderText="Position" />
                <asp:BoundField HeaderText="Hired Date" />
                <asp:BoundField HeaderText="Status" />
            </Columns>
            <PagerStyle CssClass="paginate_button active" HorizontalAlign="Center" />
        </asp:GridView>
    </div>
</asp:Content>
