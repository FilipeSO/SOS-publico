using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS
{
    //public class ContentTypeId
    //{
    //    public string _ObjectType_ { get; set; }
    //    public string StringValue { get; set; }
    //}

    //public class Author
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public string LookupValue { get; set; }
    //    public string Email { get; set; }
    //}

    //public class Editor
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public string LookupValue { get; set; }
    //    public string Email { get; set; }
    //}

    //public class SortBehavior
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public string LookupValue { get; set; }
    //}

    //public class CheckedOutUserId
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    //public class SyncClientId
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    //public class VirusStatus
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    //public class CheckedOutTitle
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    //public class ParentVersionString
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    //public class ParentLeafName
    //{
    //    public string _ObjectType_ { get; set; }
    //    public int LookupId { get; set; }
    //    public object LookupValue { get; set; }
    //}

    public class ChildItem
    {
        //public string _ObjectType_ { get; set; }
        //public string _ObjectIdentity_ { get; set; }
        //public string _ObjectVersion_ { get; set; }
        //public int FileSystemObjectType { get; set; }
        //public int Id { get; set; }
        //public ContentTypeId ContentTypeId { get; set; }
        //public object _ModerationComments { get; set; }
        //public string FileLeafRef { get; set; }
        //public object Modified_x0020_By { get; set; }
        //public object Created_x0020_By { get; set; }
        //public object File_x0020_Type { get; set; }
        //public object HTML_x0020_File_x0020_Type { get; set; }
        //public object _SourceUrl { get; set; }
        //public object _SharedFileIndex { get; set; }
        //public object Title { get; set; }
        //public object TemplateUrl { get; set; }
        //public object xd_ProgID { get; set; }
        //public object xd_Signature { get; set; }
        //public object MpoSituacao { get; set; }
        public string MpoCodigo { get; set; }
        //public object MpoAssunto { get; set; }
        public string _Revision { get; set; }
        public string MpoAlteradosPelasMops { get; set; }
        public IList<string> MpoMopsLink { get; set; }
        //public object MpoMopsCancelar { get; set; }
        //public object MpoEvento { get; set; }
        //public object MpoVigencia { get; set; }
        //public object MpoVigenciaProgramada { get; set; }
        public string MpoResponsavel { get; set; }
        //public object MpoEcmDocId { get; set; }
        public string MpoAgentesAssinantes { get; set; }
        //public object SharedWithUsers { get; set; }
        //public int Ano { get; set; }
        //public int ID { get; set; }
        //public DateTime Created { get; set; }
        //public Author Author { get; set; }
        //public DateTime Modified { get; set; }
        //public Editor Editor { get; set; }
        //public object _HasCopyDestinations { get; set; }
        //public object _CopySource { get; set; }
        //public int _ModerationStatus { get; set; }
        public string FileRef { get; set; }
        //public string FileDirRef { get; set; }
        //public DateTime Last_x0020_Modified { get; set; }
        //public DateTime Created_x0020_Date { get; set; }
        //public string File_x0020_Size { get; set; }
        //public string FSObjType { get; set; }
        //public SortBehavior SortBehavior { get; set; }
        //public CheckedOutUserId CheckedOutUserId { get; set; }
        //public string IsCheckedoutToLocal { get; set; }
        //public object CheckoutUser { get; set; }
        //public string UniqueId { get; set; }
        //public SyncClientId SyncClientId { get; set; }
        //public string ProgId { get; set; }
        //public string ScopeId { get; set; }
        //public VirusStatus VirusStatus { get; set; }
        //public CheckedOutTitle CheckedOutTitle { get; set; }
        //public string _CheckinComment { get; set; }
        //public string MetaInfo { get; set; }
        //public int _Level { get; set; }
        //public bool _IsCurrentVersion { get; set; }
        //public string ItemChildCount { get; set; }
        //public string FolderChildCount { get; set; }
        //public object AppAuthor { get; set; }
        //public object AppEditor { get; set; }
        //public int owshiddenversion { get; set; }
        //public int _UIVersion { get; set; }
        //public string _UIVersionString { get; set; }
        //public object InstanceID { get; set; }
        //public int Order { get; set; }
        //public string GUID { get; set; }
        //public int WorkflowVersion { get; set; }
        //public object WorkflowInstanceID { get; set; }
        //public ParentVersionString ParentVersionString { get; set; }
        //public ParentLeafName ParentLeafName { get; set; }
        //public string DocConcurrencyNumber { get; set; }
    }

    public class ModelMPO
    {
        public IList<ChildItem> _Child_Items_ { get; set; }
    }

}
