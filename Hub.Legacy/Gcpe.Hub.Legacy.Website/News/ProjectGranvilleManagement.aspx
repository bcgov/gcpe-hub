<%@ Page Title="Project Granville Management" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="ProjectGranvilleManagement.aspx.cs" Inherits="Gcpe.Hub.News.ProjectGranvilleManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
    <style>
        .bc-gov-alertbanner {
          border: 1px solid transparent;
          border-radius: 4px;
          font-weight: 700;
          padding: 15px;
          padding-left: 0;
        }

        .bc-gov-alertbanner p {
          font-size: 18px;
          margin: 0;
          padding-left: 15px;
        }

        .bc-gov-alertbanner-error {
          background-color: #f2dede;
          border-color: #ebccd1;
          color: #a12622
        }

        .bc-gov-alertbanner-error a {
          color: #66512c;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <h1>Project Granville Management</h1>
    <div class="bc-gov-alertbanner bc-gov-alertbanner-error" role="alert" aria-labelledby="warning" aria-describedby="error-desc">
        <p id="error-desc"><i class="fa fa-exclamation-circle" aria-hidden="true"></i> &nbsp; Do not click this button unless you have approval from IGRS.</p>
    </div>
    <br />

    <div class="section">
        <table>
            <tr>
                <td style="width:30px;"></td>
                <td style="width:600px;">
                    <h2 id="enabled_Label" class="live-feed-label" runat="server">Enable Project Granville</h2>
                    <asp:Button ID="save_Button" runat="server" Text="Enable Project Granville" CssClass="primary"  OnClick="btnToggleProjectGranville" onClientClick=" return confirm('Do not click OK unless you have approval from IGRS')" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

