<%@ Page Title="Book Edit Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true"
    CodeFile="BookEdit.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Library.Views.BookEdit" %>

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
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="true" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Book Request</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Title</label>
                                <label class="input">
                                    <asp:TextBox ID="TxtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="RfvtxtRequestNo" runat="server" ErrorMessage="Title is required" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="save" SetFocusOnError="true" ControlToValidate="TxtTitle"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">ISBN</label>
                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="TxtIsbn" CssClass="form-control" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Author</label>
                                <label class="select">
                                    <asp:DropDownList ID="DdlAuthor" runat="server" DataValueField="Id" 
                                        DataTextField="Name">
                                    </asp:DropDownList><i></i>
                                    <asp:RequiredFieldValidator
                                        ID="RfvDdlAuthor" runat="server" ErrorMessage="Author is required" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue="0"
                                        SetFocusOnError="true" ControlToValidate="DdlAuthor"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Genre</label>
                                <label class="select">
                                    <asp:DropDownList ID="DdlGenre" runat="server" DataValueField="Id" DataTextField="Name">
                                    </asp:DropDownList><i></i>
                                    <asp:RequiredFieldValidator
                                        ID="RfvGrant" runat="server" ErrorMessage="Genre is required" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue="0"
                                        SetFocusOnError="true" ControlToValidate="DdlGenre"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Published Year</label>
                                <label class="input">
                                    <i class="icon-append fa fa-clock-o"></i>
                                    <asp:TextBox ID="TxtPublishedYear" CssClass="form-control timepicker-orient-top" Text="" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Copies Available</label>
                                <label class="input">
                                    <asp:TextBox ID="TxtCopiesAvailable" runat="server"></asp:TextBox>
                                    <cc1:FilteredTextBoxExtender runat="server" Enabled="True" TargetControlID="TxtCopiesAvailable" 
                                        ID="TxtCopiesAvailable_FilteredTextBoxExtender" FilterType="Numbers"></cc1:FilteredTextBoxExtender>
                                </label>
                            </section>
                        </div>
                    </fieldset>

                    <footer>
                        <asp:Button ID="BtnSave" runat="server" Text="Save" CausesValidation="true" ValidationGroup="save"
                            OnClick="BtnSave_Click" CssClass="btn btn-primary"></asp:Button>
                        <asp:Button ID="BtnDelete" runat="server" CausesValidation="False" 
                            OnClick="BtnDelete_Click" CssClass="btn btn-primary" Text="Delete"></asp:Button>

                        <cc1:ConfirmButtonExtender ID="BtnDelete_ConfirmButtonExtender" runat="server"
                            ConfirmText="Are you sure you want to delete this record?" Enabled="True" TargetControlID="BtnDelete">
                        </cc1:ConfirmButtonExtender>
                        <asp:Button ID="BtnNew" runat="server" CssClass="btn btn-primary" OnClick="BtnNew_Click" Text="New" />
                    </footer>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
