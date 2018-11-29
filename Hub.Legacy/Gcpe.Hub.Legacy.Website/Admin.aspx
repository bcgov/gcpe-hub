<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="Gcpe.Hub.Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <table>
         <tr>
             <td><b>Azure Files Container</b></td>
             <td><%= GetAzureFilesContainer() %></td>
         </tr>
         <tr>
             <td><b>Azure Assets Container</b></td>
             <td><%= GetAzureAssetsContainer() %></td>
         </tr>
     </table>
    </div>
    </form>
</body>
</html>
