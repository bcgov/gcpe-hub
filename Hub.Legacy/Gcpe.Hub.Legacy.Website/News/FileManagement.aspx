<%@ Page Title="" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="FileManagement.aspx.cs" Inherits="Gcpe.Hub.News.FileManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
    <style>
        table, tr, td {
            padding: 10px;
        }
        .uploadfileblock table tr, 
        .uploadfileblock table tr td {
            padding:0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <script>
        function OnFileDeleteSwitch(link) {
            deleted = (link.text != "Cancel");
            file = $(link).parent().parent().find('td:nth-child(2) span');

            if (deleted) {
                file.css("text-decoration", "line-through");
                link.text = "Cancel";

            } else {
                file.css("text-decoration", "");
                link.text = "Delete";
            }

            $(link).parent().find("input[type=hidden]").val(deleted);

        }
        function SaveFiles() {
            if (document.getElementById("fileUpload").files != null
                && document.getElementById("fileUpload").files.length > 0) {
                startUpload('<%= ResolveUrl("~/News/MediaAssetManagement.asmx") %>', 'fileUpload', 1048576, 'uploadProgress', 'uploadStatusMessage', '', '', 'uploadkey', 'uploadPath', 'hdnFilesButton');
            } else {
                $("#hdnFilesButton").click();
            }
        }
        function UploadFiles(event) {
            event.preventDefault(); 
            startUpload('<%= ResolveUrl("~/News/MediaAssetManagement.asmx") %>', 'fileUpload', 1048576, 'uploadProgress', 'uploadStatusMessage', '', '', 'uploadkey', 'uploadPath', 'btnRefresh');
        }
    </script>
    <h1>File Management</h1>
    <br />

    <asp:Panel runat="server" ID="pnlErrors" CssClass="section-error" Visible="false">
        <h2>Sorry, but there was an error with your submission.</h2>
        <asp:Repeater runat="server" ID="rptErrors">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate><li><%# Container.DataItem %></li></ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <div class="section edit">
        <h2>Upload Files</h2>
        <div class="helper"></div>
        <div class="uploadfileblock">
            <table>
                <tr>
                    <td style="width:30px;"></td>
                    <td style="width:600px; ">
                       <input type="file" name="fileUpload" id="fileUpload" class="fileInput" multiple="multiple" size="60" />
                        <asp:Button ID="btnUpload" CssClass="primary" OnClientClick="UploadFiles(event);return false;" Text="Upload" runat="server" style="display:inline-block;" />
                        <asp:Button ID="btnRefresh" runat="server" Text="Save" CssClass="primary" OnClientClick="location.reload(); return false;" Style="visibility: hidden; display: none;" ClientIDMode="Static" />
                        <input type="hidden" value="staticFiles" id="uploadkey" name="uploadkey" />
                        <input type="hidden" value="" id="uploadPath" name="uploadPath" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <progress id="uploadProgress" value="0" max="100" style="width: 90%; height: 10px" hidden="hidden" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="left" >
                       <span id="uploadStatusMessage" class="message"></span>
                    </td>
                </tr>
            </table>
        </div>
        <br />
    </div>


    <div class="section edit">     
        <h2>Uploaded Files</h2>
        
        <div class="filelistblock">
            
            <table>
                 <asp:Repeater ID="rptAssetList" DataSource="<%# GetStaticFiles() %>" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td></td>
                            <td><asp:Label ID="file" runat="server" Text="<%# Container.DataItem.ToString() %>" /></td>
                            <td>
                                <asp:HyperLink runat="server" NavigateUrl='<%# Gcpe.Hub.Configuration.App.Settings.NewsHostUri + "files/" + Container.DataItem.ToString() %>' Text="View file" />
                            </td>

                            <td style="text-align: center; padding: 3px;">
                                <asp:HyperLink ID="btnDeleteAsset" runat="server" CssClass="switch" NavigateUrl="#" onclick='OnFileDeleteSwitch(this); return false;' Text="Delete"></asp:HyperLink>
                                <asp:HiddenField ID="valDeleted" Value="" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td></td>
                    <td align="left">
                        <asp:Button ID="btnDelete" CssClass="primary" OnClientClick="SaveFiles();return false;" Text="Save" runat="server"/>
                        <asp:Button ID="hdnFilesButton" runat="server" Text="Save" CssClass="primary" OnClick="btnSaveFiles_Click" Style="visibility: hidden; display: none;" ClientIDMode="Static" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr id="NoFileMessage" runat="server" visible="false">
                    <td></td>
                    <td align="left">No file is uploaded.</td>
                    <td></td>
                </tr>
            </table>
        </div>
        <br />
    </div>

</asp:Content>
