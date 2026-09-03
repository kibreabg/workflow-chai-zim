<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserLogin.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Shell.Views.UserLogin"
    Title="UserLogin" MasterPageFile="~/Shared/LogInMaster.master" %>

<asp:Content ID="content1" ContentPlaceHolderID="DefaultContent" runat="Server">
    <script src="../js/libs/jquery-2.0.2.min.js"></script>
    <style>
        .loading-container {
            position: relative;
            width: 100%;
            height: 100%;
        }

        .loading-overlay {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.7);
            z-index: 1000;
            display: none;
            align-items: center;
            justify-content: center;
        }

            .loading-overlay .jarviswidget-loader {
                font-size: 2em;
                color: #333;
                display: block;
            }

        .jarviswidget {
            position: relative;
        }

        .loading-container.loading .loading-overlay {
            display: flex;
        }
    </style>
    <script type="text/javascript">
        function showLoading() {
            var $container = $('.loading-container');
            $container.addClass('loading');
        }

        function hideLoading() {
            var $container = $('.loading-container');
            $container.removeClass('loading');
        }
    </script>

    <header>
        <span class="widget-icon"></span>
        <h1><span id="logo">
            <img src="img/CHAILogo.png" alt="SmartAdmin" />
        </span>Welcome To CHAI Zimbabwe - Workflow System Version 2.0 </h1>
    </header>
    <div class="col-xs-12 col-sm-12 col-md-7 col-lg-8 hidden-xs hidden-sm">
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6"></div>
        </div>
    </div>
    <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
        <div class="well no-padding">
            <div class="loading-container">
                <div class="loading-overlay">
                    <span class="jarviswidget-loader"><i class="fa fa-spinner fa-spin"></i></span>
                </div>
                <header>Sign In</header>
                <fieldset>
                    <asp:Label ID="lblLoginError" runat="server" CssClass="label" EnableViewState="False"></asp:Label>
                    <asp:Label ID="lblForgotPassword" runat="server" CssClass="label" EnableViewState="False" ForeColor="Red"></asp:Label>
                    <section>
                        <label class="label">User Name</label>
                        <label class="input">
                            <i class="icon-append fa fa-user"></i>
                            <asp:TextBox class="inputText" ID="txtUsername" runat="server"></asp:TextBox>
                            <b class="tooltip tooltip-top-right"><i class="fa fa-user txt-color-teal"></i>Please enter username</b></label>
                    </section>
                    <section>
                        <label class="label">Password</label>
                        <label class="input">
                            <i class="icon-append fa fa-lock"></i>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                            <b class="tooltip tooltip-top-right"><i class="fa fa-lock txt-color-teal"></i>Enter your password</b>
                        </label>
                    </section>
                    <section>
                        <asp:CheckBox ID="chkPersistLogin" runat="server" Text="" Font-Size="Small" Style="font-size: x-small" />
                        Stay signed in
                    </section>
                    <section>
                        <asp:LinkButton ID="lnkForgotPassword" runat="server" OnClick="lnkForgotPassword_Click">Forgot Password</asp:LinkButton>
                    </section>
                </fieldset>
                <footer>
                    <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Sign in" class="btn btn-primary" OnClientClick="showLoading();"></asp:Button>
                </footer>
            </div>
        </div>
    </div>
</asp:Content>

