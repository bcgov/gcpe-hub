<%@ Page Language="C#" AutoEventWireup="true" Inherits="RecordHistory" MasterPageFile="~/Contacts/MasterPage/MediaRelations.Master" CodeBehind="RecordHistory.aspx.cs" %>

<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<% bool filterOnEntityId = !string.IsNullOrEmpty(Request.QueryString["guid"]); %>
    <script type="text/javascript">
        var skip = 0;
        var limit = 250;

        function scrollHandler() {
            var scrollTop = $(window).scrollTop();
            if (scrollTop > $(document).height() - $(window).height() - 30) {
                getHistoryRecords();
            }
        }

        function getHistoryRecords() {
            var loadmoreajaxloader = $('div#loadmoreajaxloader');
            if (loadmoreajaxloader.is(':visible')) return;
            loadmoreajaxloader.show();
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                url: "RecordHistory.aspx/GetHistoryRecords",
                data: {
                    "skip": skip,
                    "limit": limit,
                    "sortDescending": <%= Request.QueryString["sortDir"] == "asc" ? "false" : "true" %>,
                    "sort": <%= FormatJsonStringParam(Request, "sort") %>,
                    "guid": <%= FormatJsonStringParam(Request, "guid") %>,
                    "type": <%= FormatJsonStringParam(Request, "type") %>
                },
                dataType: "json",
                success: function (data) {
                    loadmoreajaxloader.hide();
                    if (data.d.length == 0) {
                        $(window).off("scroll", scrollHandler);
                        if (skip != 0) {
                            $('tr.bottom').append($('tr.top').children().clone());
                        }
                        return;
                    }
                    skip += limit;
                    var isEven = true;
                    var tableContent = $('#tableContent');
                    $.each(data.d, function (key, val) {
                        var row = "<td style='max-width: 120px'>" + val.Action;
                        <%if (!filterOnEntityId)
                        { %>
                        row += "</td><td style='max-width: 190px'>" + val.RecordName;
                        <% } %>
                        row += "</td><td>" + val.EventUser;
                        row += "</td><td>" + val.EventData;
                        row += "</td><td>" + val.EventDate + "</td>";
                        tableContent.append("<tr class=" + (isEven ? "'even'>" : "'odd'>") + row + "</tr>");
                        isEven = !isEven;
                    });
                },
                error:function (error) {
                    alert(error.responseJSON.Message);
                    loadmoreajaxloader.hide();
                    limit = 100;
                }
            })
        }
        $(window).scroll(scrollHandler);

        $(document).ready(function () {
            getHistoryRecords();
        });

    </script>

    <%if (filterOnEntityId)
    { %>
    <style type="text/css">
        .noprint, div.bottom-filler-div {
            display: none !important;
        }
        .common-page-960 {
            width: 100% !important;
        }
    </style>
    <% } %>
    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsApprovals" AccessLevel="Read">
        <Content>
            <h1>Record History</h1>

            <table class="common-admin-table top">
                <thead>
                    <tr class="top">
                        <th style="width: 100px">
                            <mr:SortColumnHeader Text="Action" runat="server" />
                        </th>
                        <%if (string.IsNullOrEmpty(Request.QueryString["guid"]))
                        { %>
                        <th>
                            <mr:SortColumnHeader Text="Record Name" runat="server" />
                        </th>
                        <% } %>
                        <th>
                            <mr:SortColumnHeader Text="User" runat="server" />
                        </th>
                        <th>Change</th>
                        <th style="width: 200px">
                            <mr:SortColumnHeader Text="Date" runat="server" />
                        </th>
                    </tr>
                </thead>

                <tbody id="tableContent" />
                </tbody>
                <tfoot>
                    <tr class="bottom">
                    </tr>
                </tfoot>

            </table>
            <div id="loadmoreajaxloader" style="display: none; text-align: center">
                <img src="../images/ajaxloader.gif" />
            </div>
        </Content>
    </mrcl:PermissionContainer>
</asp:Content>
