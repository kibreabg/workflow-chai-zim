<%@ Page Title="Vendor Request" Language="C#" MasterPageFile="~/Shared/ModuleMaster.master" AutoEventWireup="true" CodeFile="frmVendorRequest.aspx.cs" Inherits="Chai.WorkflowManagment.Modules.Setting.Views.frmVendorRequest" %>

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
    <div class="jarviswidget" id="wid-id-8" data-widget-editbutton="false" data-widget-custombutton="false">
        <header>
            <span class="widget-icon"><i class="fa fa-edit"></i></span>
            <h2>Vendor Request</h2>
        </header>
        <div>
            <div class="jarviswidget-editbox"></div>
            <div class="widget-body no-padding">
                <div class="smart-form">
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Request Date</label>
                                <label class="input">
                                    <i class="icon-append fa fa-calendar"></i>
                                    <asp:TextBox ID="txtRequestDate" ReadOnly="true" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">TaxNo #</label>
                                <label class="input">
                                    <asp:TextBox ID="txtTaxNo" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Vendor Legal Entity Name</label>
                                <label class="input">
                                    <asp:TextBox ID="txtVendorLegalName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvVendorLegalName" runat="server" ErrorMessage="Please Enter Vendor Legal Entity Name" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtVendorLegalName"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Doing-Business-As Name (Trade Name)</label>
                                <label class="input">
                                    <asp:TextBox ID="txtTradeName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvTradeName" runat="server" ErrorMessage="Please Enter Trade Name" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtTradeName"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Vendor Registration Certificate (Where Applicable)</label>
                                <label class="input">
                                    <asp:TextBox ID="txtCertificate" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <header style="background: #e6e6fa;">Business Address</header>
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Address Line 1</label>
                                <label class="input">
                                    <asp:TextBox ID="txtAddressLine1" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvAddress1" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtAddressLine1"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Address Line 2</label>
                                <label class="input">
                                    <asp:TextBox ID="txtAddressLine2" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvAddress2" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtAddressLine2"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">City</label>
                                <label class="input">
                                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvCity" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtCity"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Postal Code</label>
                                <label class="input">
                                    <asp:TextBox ID="txtBusinessPostalCode" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvBusPostal" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtBusinessPostalCode"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">State / Province</label>
                                <label class="input">
                                    <asp:TextBox ID="txtStateProvince" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvState" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtStateProvince"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Country</label>
                                <label class="input">
                                    <asp:TextBox ID="txtCountry" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvCountry" runat="server" ErrorMessage="Address fields are mandatory" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtCountry"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <header style="background: #e6e6fa;">Postal Address (If different than Business Address)</header>
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Address Line 1</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalAddress1" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Address Line 2</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalAddress2" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">City</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalCity" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Postal Code</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalPostalCode" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">State / Province</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalState" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Country</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPostalCountry" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <header style="background: #e6e6fa;">Contact Information</header>
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Contact Name</label>
                                <label class="input">
                                    <asp:TextBox ID="txtContactName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvContactName" runat="server" ErrorMessage="Please Enter Contact Name" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtContactName"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Position</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPosition" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Email Address</label>
                                <label class="input">
                                    <asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvEmail" runat="server" ErrorMessage="Please Enter Email Address" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtEmailAddress"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Website</label>
                                <label class="input">
                                    <asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Phone Number</label>
                                <label class="input">
                                    <asp:TextBox ID="txtPhoneNumber" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator
                                        ID="rfvPhoneNum" runat="server" ErrorMessage="Please Enter Phone Number" Display="Dynamic"
                                        CssClass="validator" ValidationGroup="saveMain" InitialValue=""
                                        SetFocusOnError="true" ControlToValidate="txtPhoneNumber"></asp:RequiredFieldValidator>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Cell Number</label>
                                <label class="input">
                                    <asp:TextBox ID="txtCellNumber" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Fax Number</label>
                                <label class="input">
                                    <asp:TextBox ID="txtFaxNumber" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                    </fieldset>
                    <fieldset>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Briefly explain your technical capacity</label>
                                <label class="textarea">
                                    <asp:TextBox ID="txtTechnicalCapacity" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </label>
                            </section>
                            <section class="col col-6">
                                <label class="label">Trade Reference 1 (Name, Address and Contact Person Details)</label>
                                <label class="textarea">
                                    <asp:TextBox ID="txtTradeRef1" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </label>
                            </section>
                        </div>
                        <div class="row">
                            <section class="col col-6">
                                <label class="label">Trade Reference 2 (Name, Address and Contact Person Details)</label>
                                <label class="textarea">
                                    <asp:TextBox ID="txtTradeRef2" TextMode="MultiLine" runat="server"></asp:TextBox>
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
                                            <a href="#iss2" data-toggle="tab">Attach Reciept</a>
                                        </li>
                                    </ul>
                                    <div class="tab-content padding-10">
                                        <div class="tab-pane active" id="iss2">
                                            <div class="alert alert-info fade in" style="margin-bottom: 20px; margin-top: 0; border-width: 0; border-left-width: 5px !important; padding: 10px; border-radius: 0; -webkit-border-radius: 0; -moz-border-radius: 0;">
                                                <button class="close" data-dismiss="alert">
                                                    ×
                                                </button>
                                                <i class="fa-fw fa fa-info"></i>
                                                <strong>Info!</strong> Please attach the following documents:<br /><br />
                                                &emsp;&emsp;<strong>1.</strong> Tax clearance certificate (if available)<br />
                                                &emsp;&emsp;<strong>2.</strong> VAT registration certificate (if available)<br />
                                                &emsp;&emsp;<strong>3.</strong> Copy of Company profile (if available)<br />
                                                &emsp;&emsp;<strong>4.</strong> Copy of Certificate of Incorporation
                                            </div>
                                            <fieldset>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="label">Attach Reciepts</label>
                                                        <asp:FileUpload ID="fuReciept" runat="server" />
                                                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
                                                    </section>
                                                </div>
                                            </fieldset>
                                            <asp:GridView ID="grvAttachments"
                                                runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                                CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active">
                                                <RowStyle CssClass="rowstyle" />
                                                <Columns>
                                                    <asp:BoundField DataField="FilePath" HeaderText="File Name" SortExpression="FilePath" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkDownload" Text="Download" CssClass="btn btn-primary" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkDelete" Text="Delete" CssClass="btn btn-primary" CommandArgument='<%# Eval("FilePath") %>' runat="server" OnClick="DeleteFile" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <FooterStyle CssClass="FooterStyle" />
                                                <HeaderStyle CssClass="headerstyle" />
                                                <PagerStyle CssClass="PagerStyle" />
                                                <RowStyle CssClass="rowstyle" />
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <!-- end widget content -->

                    </div>
                    <footer>
                        <asp:Button ID="btnSave" runat="server" Text="Request" class="btn btn-primary" ValidationGroup="saveMain" OnClick="btnSave_Click"></asp:Button>
                        <a data-toggle="modal" runat="server" id="searchLink" href="#searchModal" class="btn btn-primary"><i class="fa fa-circle-arrow-up fa-lg"></i>Search</a>
                    </footer>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="searchModal" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="myModalLabel">Search Vendor Requests</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="txtSrchRequestNo">Voucher No.</label>
                                <asp:TextBox ID="txtSrchRequestNo" CssClass="form-control" ToolTip="Voucher No" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="txtSrchRequestDate">Request Date</label>
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
                                <asp:Button Text="Close" ID="btnCancelSearch" runat="server" class="btn btn-primary"></asp:Button>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <div class="well well-sm well-primary">
                                    <asp:GridView ID="grvVendorRequestList"
                                        runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                        OnSelectedIndexChanged="grvVendorRequestList_SelectedIndexChanged"
                                        AllowPaging="True" OnPageIndexChanging="grvVendorRequestList_PageIndexChanging"
                                        CssClass="table table-striped table-bordered table-hover" PagerStyle-CssClass="paginate_button active" PageSize="5" OnRowDataBound="grvVendorRequestList_RowDataBound">
                                        <RowStyle CssClass="rowstyle" />
                                        <Columns>
                                            <asp:BoundField DataField="RequestNo" HeaderText="Request No" SortExpression="RequestNo" />
                                            <asp:TemplateField HeaderText="Request Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestDate", "{0:dd/MM/yyyy}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="VendorLegalEntityName" HeaderText="Vendor Legal Entity Name" SortExpression="VendorLegalEntityName" />
                                            <asp:BoundField DataField="ContactName" HeaderText="Contact Name" SortExpression="ContactName" />
                                            <asp:BoundField DataField="CurrentStatus" HeaderText="Status" SortExpression="CurrentStatus" />
                                            <asp:CommandField ShowSelectButton="True" />
                                        </Columns>
                                        <FooterStyle CssClass="FooterStyle" />
                                        <HeaderStyle CssClass="headerstyle" />
                                        <PagerStyle CssClass="PagerStyle" />
                                        <RowStyle CssClass="rowstyle" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>
