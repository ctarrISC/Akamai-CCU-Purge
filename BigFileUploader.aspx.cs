#region Imported Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI.Widgets;
using System.Configuration;
using Telerik.Web.UI;

#endregion

namespace HostSite.Web.UI.Admin
{
    #region Big File Uploader

    public partial class BigFileUploader : System.Web.UI.Page
    {
        #region UX Variables

        protected string uxFieldId;

        #endregion

        #region Page Load
		protected void Page_Load(object sender, EventArgs e)
        {
            uxFieldId = (Request.QueryString["fieldId"] != null) ? Request.QueryString["fieldId"] : "";

            FileExplorer1.Configuration.ContentProviderTypeName = typeof(CustomFileBrowserProviderWithFilter).AssemblyQualifiedName;
            if (ConfigurationManager.AppSettings["BigFileUploaderPaths"] != null)
            {
                FileExplorer1.Configuration.ViewPaths = ConfigurationManager.AppSettings["BigFileUploaderPaths"].Split(',');
                FileExplorer1.Configuration.UploadPaths = ConfigurationManager.AppSettings["BigFileUploaderPaths"].Split(',');
                FileExplorer1.Configuration.DeletePaths = ConfigurationManager.AppSettings["BigFileUploaderPaths"].Split(',');
            }

            if (ConfigurationManager.AppSettings["BigFileUploaderPaths"] != null)
            {
                int maxUploadSize = 104857600;
                if (Int32.TryParse(ConfigurationManager.AppSettings["BigFileUploaderMaxUploadFileSize"], out maxUploadSize))
                {
                    FileExplorer1.Configuration.MaxUploadFileSize = maxUploadSize;
                }
                else
                {
                    FileExplorer1.Configuration.MaxUploadFileSize = 104857600;
                }
            }
        }

        #endregion

        protected void RadFileExplorer1_ItemCommand(object sender, RadFileExplorerEventArgs e)
        {

            if (e.Command.Equals("UploadFile"))
            {
                RadProgressContext context = RadProgressContext.Current;
                context.SecondaryTotal = "100";
                for (int i = 1; i < 100; i++)
                {
					// A very time consumming task
                    context.SecondaryValue = i.ToString();
                    context.SecondaryPercent = i.ToString();
                    context.CurrentOperationText = "Doing step " + i.ToString();

                    if (!Response.IsClientConnected)
                    {
                        //Cancel button was clicked or the browser was closed, so stop processing
                        break;
                    }
                    // simulate a long time performing the current step
                    System.Threading.Thread.Sleep(100);
                }
                
                try 
                {
                    //create the purgeResult object
                    HostSite.Web.UI.ServiceReference1.PurgeResult r = new HostSite.Web.UI.ServiceReference1.PurgeResult();
                    HostSite.Web.UI.ServiceReference1.PurgeApiClient a;

                    a = new HostSite.Web.UI.ServiceReference1.PurgeApiClient();
                    string[] options = new string[4];
                    string[] uri = new string[1];

                    //get the login credential and other data for Akamai from the web.config
                    options[0] = ConfigurationManager.AppSettings["AkamaiCCUEmail"].ToString();
                    options[1] = ConfigurationManager.AppSettings["AkamaiCCUAction"].ToString();
                    options[2] = ConfigurationManager.AppSettings["AkamaiCCUType"].ToString();
                    options[3] = ConfigurationManager.AppSettings["AkamaiCCUDomain"].ToString();

                    //build the URL to the file that was uploaded
                    uri[0] = "http://www.yourdomain.com" + e.Path;
                    
                    //request the content purge
                    r = a.purgeRequest(ConfigurationManager.AppSettings["AkamaiCCUUserName"].ToString(), ConfigurationManager.AppSettings["AkamaiCCUPassword"].ToString(), "", options, uri);
                    
                    //log the activity
                    Web.Core.Context.Current.LogMessage("Akamai CCU WebService Message: " + r.resultCode + " " + r.resultMsg + " " + r.sessionID + " " + uri[0].ToString());
                    
                }
                catch (Exception ex)
                {
                    //something didn't work so let's report the issue
                    Web.Core.Context.Current.LogMessage("Error Akamai CCU WebService Message: ", ex);
                }
            }
        }
    }

    #endregion

    #region Content Provider

    public class CustomFileBrowserProviderWithFilter : FileSystemContentProvider
    {
        public CustomFileBrowserProviderWithFilter(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag)
            : base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
        }

        public override DirectoryItem ResolveDirectory(string path)
        {
            DirectoryItem originalFolder = base.ResolveDirectory(path);
            FileItem[] originalFiles = originalFolder.Files;
            List<FileItem> filteredFiles = new List<FileItem>();

            // Filter the files
            foreach (FileItem originalFile in originalFiles)
            {
                if (!this.IsFiltered(originalFile.Name))
                {
                    filteredFiles.Add(originalFile);
                }
            }

            DirectoryItem newFolder = new DirectoryItem(originalFolder.Name, originalFolder.Location, originalFolder.FullPath, originalFolder.Tag, originalFolder.Permissions, filteredFiles.ToArray(), originalFolder.Directories);

            return newFolder;
        }

        public override DirectoryItem ResolveRootDirectoryAsTree(string path)
        {
            DirectoryItem originalFolder = base.ResolveRootDirectoryAsTree(path);
            DirectoryItem[] originalDirectories = originalFolder.Directories;
            List<DirectoryItem> filteredDirectories = new List<DirectoryItem>();

            // Filter the folders
            foreach (DirectoryItem originalDir in originalDirectories)
            {
                if (!this.IsFiltered(originalDir.Name))
                {
                    filteredDirectories.Add(originalDir);
                }
            }
            DirectoryItem newFolder = new DirectoryItem(originalFolder.Name, originalFolder.Location, originalFolder.FullPath, originalFolder.Tag, originalFolder.Permissions, originalFolder.Files, filteredDirectories.ToArray());

            return newFolder;
        }

        private bool IsFiltered(string name)
        {
            if (name.ToLower().EndsWith(".svn") || name.ToLower().Contains("_svn"))
            {
                return true;
            }

            // else
            return false;
        }
    }

    #endregion
}