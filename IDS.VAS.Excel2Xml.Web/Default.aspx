<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server" />
</head>
<body>
  <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
      <Scripts>
        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
      </Scripts>
    </telerik:RadScriptManager>
    <script type="text/javascript">
        //Put your JavaScript code here.
    </script>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <div>

      <telerik:RadFormDecorator RenderMode="Lightweight" ID="FormDecorator1" runat="server" DecoratedControls="all" DecorationZoneID="decorationZone"></telerik:RadFormDecorator>
      <div>
        <div id="decorationZone">
          <fieldset>
            <legend>1. Upload</legend>
            <p>Bitte laden Sie die EXCEL-Datei hoch.</p>
            <telerik:RadAsyncUpload ID="upload_files" runat="server" MultipleFileSelection="Disabled" MaxFileInputsCount="1"></telerik:RadAsyncUpload>
          </fieldset>
          <fieldset>
            <legend>2. Ausführen</legend>
            <p>Klicken Sie abschließend auf den "Ausführen"-Button und warten Sie die Konvertierung ab.</p>
            <telerik:RadButton ID="btn_execute" runat="server" Text="Ausführen" OnClick="btn_execute_Click"></telerik:RadButton><br />
            <asp:Label ID="calc_error" runat="server" Text=""></asp:Label>
          </fieldset>
        </div>
      </div>

    </div>
  </form>
</body>
</html>
