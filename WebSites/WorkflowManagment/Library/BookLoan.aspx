<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BookLoan.aspx.cs" 
    Inherits="Chai.WorkflowManagment.Modules.Library.Views.BookLoan"
    Title="Book Loan" MasterPageFile="~/Shared/ModuleMaster.master" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>
<asp:Content ID="Content" ContentPlaceHolderID="DefaultContent" runat="Server">
    <div class="jarviswidget" id="WidIdLoan" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-book"></i></span>
            <h2>Book Loan</h2>
        </header>
        <div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="LblTitle" runat="server" Text="Title:" CssClass="label"></asp:Label>
                                <asp:Label ID="LblBookTitle" runat="server" CssClass="form-control-static"></asp:Label>
                            </section>
                            <section class="col col-6">
                                <asp:Label ID="LblAuthor" runat="server" Text="Author:" CssClass="label"></asp:Label>
                                <asp:Label ID="LblBookAuthor" runat="server" CssClass="form-control-static"></asp:Label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="LblISBN" runat="server" Text="ISBN:" CssClass="label"></asp:Label>
                                <asp:Label ID="LblBookISBN" runat="server" CssClass="form-control-static"></asp:Label>
                            </section>
                            <section class="col col-6">
                                <asp:Label ID="LblCopies" runat="server" Text="Copies Available:" CssClass="label"></asp:Label>
                                <asp:Label ID="LblBookCopies" runat="server" CssClass="form-control-static"></asp:Label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <asp:Label ID="LblLoanDate" runat="server" Text="Loan Date:" CssClass="label"></asp:Label>
                                <asp:TextBox ID="TxtLoanDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </section>
                            <section class="col col-6">
                                <asp:Label ID="LblDueDate" runat="server" Text="Due Date:" CssClass="label"></asp:Label>
                                <asp:TextBox ID="TxtDueDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </section>
                        </div>
                    </fieldset>
                    <footer>
                        <asp:Button ID="BtnSubmitLoan" runat="server" Text="Submit Loan" CssClass="btn btn-primary" OnClick="BtnSubmitLoan_Click" />
                        <asp:Button ID="BtnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" PostBackUrl="Books.aspx" />
                    </footer>
                    <asp:Label ID="LblMessage" runat="server" CssClass="text-danger"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
