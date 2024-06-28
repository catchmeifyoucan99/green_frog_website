<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="CKFinder.Settings.ConfigFile" %>
<%@ Import Namespace="CKFinder.Settings" %>
<script runat="server">

    /**
	 * This function must check the user session to be sure that he/she is
	 * authorized to upload and access files using CKFinder.
	 */
    // Đây là chỗ kiểm tra quyền trước khi cho hiện Ckfinder
    public override bool CheckAuthentication()
    {
        return true;
    }

    /**
	 * All configuration settings must be defined here.
	 */
    public override void SetConfig()
    {
        // Paste your license name and key here. If left blank, CKFinder will
        // be fully functional, in Demo Mode.

        // Đây là licent 
        LicenseName = "Phan Quoc Trieu";
        LicenseKey = "59EG7AEF631AX6GJMDYRQ3M5R85KJMPG";


        // Đây là thiết lập đường dẫn lưu file
        BaseUrl = "/DATA";

        // The phisical directory in the server where the file will end up. If
        // blank, CKFinder attempts to resolve BaseUrl.
        BaseDir = HttpContext.Current.Server.MapPath("/DATA");


        // Optional: enable extra plugins (remember to copy .dll files first).
        Plugins = new string[] {
			// "CKFinder.Plugins.FileEditor, CKFinder_FileEditor",
			// "CKFinder.Plugins.ImageResize, CKFinder_ImageResize",
			// "CKFinder.Plugins.Watermark, CKFinder_Watermark"
		};
        // Thiết lập các loại ảnh được tạo ra thêm khi mình upload ảnh lên

        PluginSettings = new Hashtable();
        PluginSettings.Add("ImageResize_smallThumb", "90x90");
        PluginSettings.Add("ImageResize_mediumThumb", "120x120");
        PluginSettings.Add("ImageResize_largeThumb", "180x180");
        // Name of the watermark image in plugins/watermark folder
        PluginSettings.Add("Watermark_source", "logo.gif");
        PluginSettings.Add("Watermark_marginRight", "5");
        PluginSettings.Add("Watermark_marginBottom", "5");
        PluginSettings.Add("Watermark_quality", "90");
        PluginSettings.Add("Watermark_transparency", "80");

        // Thumbnail settings.
        // "Url" is used to reach the thumbnails with the browser, while "Dir"
        // points to the physical location of the thumbnail files in the server.

        // Đây là đường dẫn ảnh nhỏ - sử dụng trong quá trình hiển thị
        //Thumbnails.Url = BaseUrl + "/thumbs/";
        //if ( BaseDir != "" ) {
        //    Thumbnails.Dir = BaseDir + "/thumbs/";
        //}
        //Thumbnails.Enabled = true;
        //Thumbnails.DirectAccess = false;
        //Thumbnails.MaxWidth = 100;
        //Thumbnails.MaxHeight = 100;
        //Thumbnails.Quality = 80;

        // Set the maximum size of uploaded images. If an uploaded image is
        // larger, it gets scaled down proportionally. Set to 0 to disable this
        // feature.
        Images.MaxWidth = 1600;
        Images.MaxHeight = 1200;
        Images.Quality = 80;

        // Indicates that the file size (MaxSize) for images must be checked only
        // after scaling them. Otherwise, it is checked right after uploading.
        CheckSizeAfterScaling = true;

        // Increases the security on an IIS web server.
        // If enabled, CKFinder will disallow creating folders and uploading files whose names contain characters
        // that are not safe under an IIS 6.0 web server.
        DisallowUnsafeCharacters = true;

        // If CheckDoubleExtension is enabled, each part of the file name after a dot is
        // checked, not only the last part. In this way, uploading foo.php.rar would be
        // denied, because "php" is on the denied extensions list.
        // This option is used only if ForceSingleExtension is set to false.
        CheckDoubleExtension = true;

        // Due to security issues with Apache modules, it is recommended to leave the
        // following setting enabled. It can be safely disabled on IIS.
        ForceSingleExtension = true;

        // For security, HTML is allowed in the first Kb of data for files having the
        // following extensions only.
        HtmlExtensions = new string[] { "html", "htm", "xml", "js" };

        // Folders to not display in CKFinder, no matter their location. No
        // paths are accepted, only the folder name.
        // The * and ? wildcards are accepted.
        // By default folders starting with a dot character are disallowed.
        HideFolders = new string[] { ".*", "CVS" };

        // Files to not display in CKFinder, no matter their location. No
        // paths are accepted, only the file name, including extension.
        // The * and ? wildcards are accepted.
        HideFiles = new string[] { ".*" };

        // Perform additional checks for image files.
        SecureImageUploads = true;

        // The session variable name that CKFinder must use to retrieve the
        // "role" of the current user. The "role" is optional and can be used
        // in the "AccessControl" settings (bellow in this file).
        RoleSessionVar = "CKFinder_UserRole";

        // ACL (Access Control) settings. Used to restrict access or features
        // to specific folders.
        // Several "AccessControl.Add()" calls can be made, which return a
        // single ACL setting object to be configured. All properties settings
        // are optional in that object.
        // Subfolders inherit their default settings from their parents' definitions.
        //
        //	- The "Role" property accepts the special "*" value, which means
        //	  "everybody".
        //	- The "ResourceType" attribute accepts the special value "*", which
        //	  means "all resource types".
        AccessControl acl = AccessControl.Add();
        acl.Role = "*";
        acl.ResourceType = "*";
        acl.Folder = "/";

        acl.FolderView = true;
        acl.FolderCreate = true;
        acl.FolderRename = true;
        acl.FolderDelete = true;

        acl.FileView = true;
        acl.FileUpload = true;
        acl.FileRename = true;
        acl.FileDelete = true;

        // Resource Type settings.
        // A resource type is nothing more than a way to group files under
        // different paths, each one having different configuration settings.
        // Each resource type name must be unique.
        // When loading CKFinder, the "type" querystring parameter can be used
        // to display a specific type only. If "type" is omitted in the URL,
        // the "DefaultResourceTypes" settings is used (may contain the
        // resource type names separated by a comma). If left empty, all types
        // are loaded.

        // ==============================================================================
        // ATTENTION: Flash files with `swf' extension, just like HTML files, can be used
        // to execute JavaScript code and to e.g. perform an XSS attack. Grant permission
        // to upload `.swf` files only if you understand and can accept this risk.
        // ==============================================================================

        // Thiết lập các loại file có thể tải lên server

        DefaultResourceTypes = "";

        ResourceType type;
        // Loại 1: HÌnh ảnh, cho vào thư mục Images (Thư mục này tự được tạo khi chạy ckfinder lần đầu)

        type = ResourceType.Add("Images");
        type.Url = BaseUrl + "/Images/";
        type.Dir = BaseDir == "" ? "" : BaseDir + "/Images/";
        type.MaxSize = 0;
        //Các định dạng file có thể tải vào thư mục Images
        type.AllowedExtensions = new string[] { "bmp", "gif", "jpeg", "jpg", "png" };
        type.DeniedExtensions = new string[] { };

        // Thư mục 2: Lưu tất cả các loại file
        ResourceType type2;
        type2 = ResourceType.Add("FILES");
        type2.Url = BaseUrl + "/FILES/";
        type2.Dir = BaseDir == "" ? "" : BaseDir + "/FILES/";
        type2.MaxSize = 0;
        // Để string rỗng => file gì tải lên cũng được
        type2.AllowedExtensions = new string[] { };
        type2.DeniedExtensions = new string[] { };

    }

</script>
