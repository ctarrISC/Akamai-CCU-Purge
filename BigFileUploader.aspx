<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BigFileUploader.aspx.cs" 
    Inherits="Digitaria.Web.Isc.UI.Admin.BigFileUploader" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Big File Uploader</title>
    <style type="text/css">
        body
        {
            font-family: arial,sans-serif;
            font-size: 9pt;
        }
        .my_clip_button
        {
            width: 300px;
            text-align: center;
            border: 1px solid black;
            background-color: #ccc;
            padding: 10px;
            cursor: default;
            font-size: 9pt;
        }
        .my_clip_button.hover
        {
            background-color: #eee;
        }
        .my_clip_button.active
        {
            background-color: #aaa;
        }
    </style>
<telerik:RadCodeBlock ID="codeBlock1" runat="server">
<script type="text/javascript">
//<![CDATA[
function OnClientItemSelected(sender, args)
{
var fileSrc = args.get_path();
}
//]]>
</script>
</telerik:RadCodeBlock>

</head>
<body style="background-color: White;margin:0px;padding:0px;">
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="Server">
    </telerik:RadScriptManager>
    
    <!--Script was here //-->
    
    <telerik:RadFormDecorator runat="server" ID="RadFormDecorator1" DecoratedControls="All" />
    <table cellspacing="4">
        <tr>
            <td style="vertical-align: top;">
                <telerik:RadFileExplorer runat="server" ID="FileExplorer1" Width="850px" Height="500px"
                    EnableOpenFile="true" DisplayUpFolderItem="true" AllowPaging="true" EnableCreateNewFolder="true"
                    OnClientItemSelected="OnClientItemSelected" Skin="Office2007" OnClientLoad="OnClientLoad"
                    OnItemCommand="RadFileExplorer1_ItemCommand" />
                <br />
                <div>
                    <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
                    <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Skin="Vista" DisplayCancelButton="true" />
                </div>
            </td>
        </tr>
    </table>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

		<script type="text/javascript">
		    function OnClientLoad(oExplorer, args) {
		        windowManager = oExplorer.get_windowManager();
		        windowManager.add_show(uploadWindowShown);
		    }

		    function uploadWindowShown(oWindow, args) {
		        if (oWindow.get_title() == "Upload") {// The upload window
		            oWindow.center();
		            oWindow.set_height(550);
    	            
		            // Find the upload button in the Upload dialog
		            var uploadBtn = $get("<%= FileExplorer1.ClientID %>_btnUpload");

		            uploadBtn.onclick = function(e) {
		                this.style.display = "none";
		            };

		            var progressArea = $find("<%= RadProgressArea1.ClientID %>");

		            // Use the button to find the parent node
		            uploadBtn.parentNode.insertBefore(progressArea.get_element(), uploadBtn);
		        }
		    }    
		</script>&nbsp;</telerik:RadScriptBlock>    
    </form>
</body>
</html>
