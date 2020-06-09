<%@ Page Title="Telehone Extension Form" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmCabRequest.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Request.Views.frmCabRequest" %>

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
        function Clickheretoprint(theid) {
            var disp_setting = "toolbar=yes,location=no,directories=yes,menubar=yes,";
            disp_setting += "scrollbars=yes,width=750, height=600, left=100, top=25";
            var content_vlue = document.getElementById(theid).innerHTML;

            var docprint = window.open("", "", disp_setting);
            docprint.document.open();
            docprint.document.write('<html><head><title>CHAI Zimbabwe</title>');
            docprint.document.write('</head><body onLoad="self.print()"><center>');
            docprint.document.write(content_vlue);
            docprint.document.write('</center></body></html>');
            docprint.document.close();
            docprint.focus();
        }
    </script>
    <div class="jarviswidget" id="wid-id-0" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Telephone Extensions</h2>
        </header>
        <div role="content">
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                       <div class="table-responsive" style="overflow: auto;">
                <asp:GridView ID="grvPurchaseRequestList"
                    runat="server" AutoGenerateColumns="false" DataKeyNames="Id" 
                    CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" PageSize="30">
                    <RowStyle CssClass="rowstyle" />
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Extension" HeaderText="Extension" SortExpression="Extension" />
                         <asp:BoundField DataField="Cellphone" HeaderText="Cellphone" SortExpression="Cellphone" />

                       
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button runat="server" ID="btnStatus" Text="" BorderStyle="None" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle CssClass="FooterStyle" />
                    <HeaderStyle CssClass="headerstyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <RowStyle CssClass="rowstyle" />
                </asp:GridView>
            </div>
                    </fieldset>

                  
                </div>
            </div>
        </div>

    </div>
    <%-- Modal --%>

    <%--<asp:Panel ID="pnlSearch" Visible="true" runat="server">--%>
   
    <%--</asp:Panel>--%>
    <%--<cc1:ModalPopupExtender runat="server" Enabled="True" PopupControlID="pnlSearch" CancelControlID="btnCancelSearch"
        TargetControlID="btnSearch" BackgroundCssClass="modalBackground" ID="pnlSearch_ModalPopupExtender">
    </cc1:ModalPopupExtender>--%>
  
  
</asp:Content>

