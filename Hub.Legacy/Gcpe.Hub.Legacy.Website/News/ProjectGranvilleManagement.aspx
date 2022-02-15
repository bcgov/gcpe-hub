<%@ Page Title="Project Granville Management" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="ProjectGranvilleManagement.aspx.cs" Inherits="Gcpe.Hub.News.ProjectGranvilleManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
    <style>
        .bc-gov-alertbanner {
          border: 1px solid transparent;
          border-radius: 4px;
          font-weight: 700;
          margin-bottom: 20px;
          padding: 15px;
        }

        .bc-gov-alertbanner p {
          font-size: 18px;
          margin: 0;
          padding-left: 35px;
        }

        .bc-gov-alertbanner-warning {
          background-color: #f9f1c6;
          border-color: #faebcc;
          color: #6c4a00;
        }

        .bc-gov-alertbanner-warning a {
          color: #66512c;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <h1>Project Granville Management</h1>
    <br />

    <div class="section">
        <table>
            <tr>
                <td style="width:30px;"></td>
                <td style="width:600px;">
                    <div class="bc-gov-alertbanner bc-gov-alertbanner-warning" role="alert" aria-labelledby="warning" aria-describedby="warning-desc">
                        <p id="warning-desc"><i class="fa fa-exclamation-triangle" aria-hidden="true"></i> &nbsp; Do not click this button unless you have approval from IGRS.</p>
                    </div>
                    <h2 id="enabled_Label" class="live-feed-label" runat="server">Enable Project Granville</h2>
                    <asp:Button ID="save_Button" runat="server" Text="Enable Project Granville" CssClass="primary"  OnClick="btnToggleProjectGranville" onClientClick=" return confirm('Do not click OK unless you have approval from IGRS')" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

