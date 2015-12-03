<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ShellDefault"
    MasterPageFile="~/Shared/DefaultMaster.master" %>

<%@ MasterType TypeName="Chai.WorkflowManagment.Modules.Shell.BaseMaster" %>
<asp:Content ID="content1" ContentPlaceHolderID="DefaultContent" runat="Server">
    <div id="content">
        <div class="row">
            <div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
                <h1 class="page-title txt-color-blueDark"><i class="fa-fw fa fa-home"></i>Dashboard</h1>
            </div>
            <div class="col-xs-12 col-sm-5 col-md-5 col-lg-8" runat="server" visible="false" id="reimbersmentstatuses">

                <ul id="sparks" class="">
                    <li class="sparks-info">
                        <h5>Not Retired</h5>
                    </li>
                    <li class="sparks-info">
                        <h5>Cash Payment <span class="txt-color-blue">
                            <asp:Label ID="lblCashPaymentreimbersment" runat="server" Text=""></asp:Label></span></h5>

                    </li>

                    <li class="sparks-info">
                        <h5>Cost Sharing Payment <span class="txt-color-greenDark">
                            <asp:Label ID="lblCostPaymentreimbersment" runat="server" Text=""></asp:Label></span></h5>

                    </li>
                </ul>
            </div>
        </div>

        <div class="row">

            <article id="A1" class="col-sm-12 col-md-12 col-lg-6 sortable-grid ui-sortable">
                <div class="jarviswidget jarviswidget-color-blue jarviswidget-sortable" id="wid-id-4" data-widget-editbutton="false" data-widget-colorbutton="false" role="widget" style="">

                    <!-- widget options:
								usage: <div class="jarviswidget" id="wid-id-0" data-widget-editbutton="false">

								data-widget-colorbutton="false"
								data-widget-editbutton="false"
								data-widget-togglebutton="false"
								data-widget-deletebutton="false"
								data-widget-fullscreenbutton="false"
								data-widget-custombutton="false"
								data-widget-collapsed="true"
								data-widget-sortable="false"

								-->

                    <header role="heading">
                        <div class="jarviswidget-ctrls" role="menu"><a href="javascript:void(0);" class="button-icon jarviswidget-toggle-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Collapse"><i class="fa fa-minus "></i></a><a href="javascript:void(0);" class="button-icon jarviswidget-fullscreen-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Fullscreen"><i class="fa fa-expand "></i></a><a href="javascript:void(0);" class="button-icon jarviswidget-delete-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Delete"><i class="fa fa-times"></i></a></div>
                        <span class="widget-icon"><i class="fa fa-check txt-color-white"></i></span>
                        <h2>My Requests </h2>
                        <!-- <div class="widget-toolbar">
									add: non-hidden - to disable auto hide

									</div>-->
                        <span class="jarviswidget-loader"><i class="fa fa-refresh fa-spin"></i></span>
                    </header>

                    <!-- widget div-->
                    <div role="content">
                        <!-- widget edit box -->
                        <div class="jarviswidget-editbox">
                            <div>
                                <label>Title:</label>
                                <input type="text">
                            </div>
                        </div>
                        <!-- end widget edit box -->

                        <div class="widget-body no-padding smart-form">
                            <!-- content goes here -->
                            <h5 class="todo-group-title">Requests</h5>
                            <ul id="sortable3" class="todo">
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblLeaveMyRequest" runat="server" Text="Leave Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblLeaveStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblVehicleMyRequest" runat="server" Text="Vehicle Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblVehicleStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblPaymentMyRequest" runat="server" Text="Payment Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblPaymentStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblCostSharingMyRequest" runat="server" Text="Cost Sharing Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblCostSharingStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblTravelAdvanceMyRequest" runat="server" Text="Travel Advance Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblTravelStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblPurchaseMyRequest" runat="server" Text="Purchase Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblPurchaseStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:Label ID="lblBankRequestMyRequest" runat="server" Text="Bank Payment Request" CssClass="label"></asp:Label>-
                                            <asp:Label ID="lblBankRequestStatus" runat="server" Text="No Request"></asp:Label>
                                    </p>
                                </li>
                            </ul>

                            <!-- end content -->
                        </div>

                    </div>
                    <!-- end widget div -->
                </div>
            </article>


            <article id="A2" class="col-sm-12 col-md-12 col-lg-6 sortable-grid ui-sortable">
                <div class="jarviswidget jarviswidget-color-blue jarviswidget-sortable" id="wid-id-3" data-widget-editbutton="false" data-widget-colorbutton="false" role="widget" style="">
                    <!-- widget options:
								usage: <div class="jarviswidget" id="wid-id-0" data-widget-editbutton="false">

								data-widget-colorbutton="false"
								data-widget-editbutton="false"
								data-widget-togglebutton="false"
								data-widget-deletebutton="false"
								data-widget-fullscreenbutton="false"
								data-widget-custombutton="false"
								data-widget-collapsed="true"
								data-widget-sortable="false"
								-->
                    <header role="heading">
                        <div class="jarviswidget-ctrls" role="menu"><a href="javascript:void(0);" class="button-icon jarviswidget-toggle-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Collapse"><i class="fa fa-minus "></i></a><a href="javascript:void(0);" class="button-icon jarviswidget-fullscreen-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Fullscreen"><i class="fa fa-expand "></i></a><a href="javascript:void(0);" class="button-icon jarviswidget-delete-btn" rel="tooltip" title="" data-placement="bottom" data-original-title="Delete"><i class="fa fa-times"></i></a></div>
                        <span class="widget-icon"><i class="fa fa-check txt-color-white"></i></span>
                        <h2>My Tasks</h2>
                        <!-- <div class="widget-toolbar">
									add: non-hidden - to disable auto hide

									</div>-->
                        <span class="jarviswidget-loader"><i class="fa fa-refresh fa-spin"></i></span>
                    </header>

                    <!-- widget div-->
                    <div role="content">
                        <!-- widget edit box -->
                        <div class="jarviswidget-editbox">
                            <div>
                                <label>Title:</label>
                                <input type="text">
                            </div>
                        </div>
                        <!-- end widget edit box -->

                        <div class="widget-body no-padding smart-form">
                            <!-- content goes here -->
                            <h5 class="todo-group-title">Requests to be Approved</h5>
                            <ul id="sortable3" class="todo">
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkLeaveRequest" runat="server" Text="Leave Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblLeaverequests" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkVehicleRequest" runat="server" Text="Vehicle Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblVehicleRequest" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkPaymentRequest" runat="server" Text="Payment Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblPaymentRequest" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkCostSharingRequest" runat="server" Text="Cost Sharing Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblCostSharingRequest" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkPurchaseRequest" runat="server" Text="Purchase Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblpurchaserequest" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkTravelAdvanceRequest" runat="server" Text="Travel Advance Requests" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblTravelAdvanceRequest" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkreviewliquidation" runat="server" Text="Review Liquidation" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblreviewliquidation" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkExpenseLiquidation" runat="server" Text="Expense Liquidation" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblExpenseLiquidation" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                                <li class="">
                                    <span class="handle" style="display: none"></span>
                                    <p>
                                        <asp:LinkButton ID="lnkbankpayment" runat="server" Text="Bank Payment" Enabled="false"></asp:LinkButton>
                                        (<small class="num-of-tasks"><asp:Label ID="lblbankpayment" runat="server" Text=""></asp:Label></small>)
                                    </p>
                                </li>
                            </ul>

                            <!-- end content -->
                        </div>

                    </div>
                    <!-- end widget div -->
                </div>
            </article>
        </div>
    </div>

</asp:Content>
