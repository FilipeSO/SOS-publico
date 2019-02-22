using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS
{
    //public class Editor
    //{
    //    public string id { get; set; }
    //    public string title { get; set; }
    //    public string email { get; set; }
    //    public string sip { get; set; }
    //    public string picture { get; set; }
    //}

    public class Row
    {
        public string ID { get; set; }
        public string PermMask { get; set; }
        public string FSObjType { get; set; }
        public string HTML_x0020_File_x0020_Type { get; set; }
        public string FileLeafRef { get; set; }
        //public string Created_x0020_Date.ifnew { get; set; }
        public string FileRef { get; set; }
        public string File_x0020_Type { get; set; }
        //public string File_x0020_Type.mapapp { get; set; }
        //public string HTML_x0020_File_x0020_Type.File_x0020_Type.mapcon { get; set; }
        //public string HTML_x0020_File_x0020_Type.File_x0020_Type.mapico { get; set; }
        //public string serverurl.progid { get; set; }
        public string ServerRedirectedEmbedUrl { get; set; }
        //public string File_x0020_Type.progid { get; set; }
        //public string File_x0020_Type.url { get; set; }
        public string ContentTypeId { get; set; }
        //public string CheckoutUser { get; set; }
        public string CheckedOutUserId { get; set; }
        public string IsCheckedoutToLocal { get; set; }
        public string MpoAssunto { get; set; }
        public string Modified { get; set; }
        //public string Modified.FriendlyDisplay { get; set; }
        //public IList<Editor> Editor { get; set; }
        public string File_x0020_Size { get; set; }
    }

    public class ModelDiagramasONS
    {
        public IList<Row> Row { get; set; }
        public int FirstRow { get; set; }
        public int LastRow { get; set; }
        public string NextHref { get; set; }
        public string FilterLink { get; set; }
        public string ForceNoHierarchy { get; set; }
        public string HierarchyHasIndention { get; set; }
    }
}
