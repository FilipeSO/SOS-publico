using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS
{
    public class Bookmark
    {
        public string Title { get; set; }
        public string Page { get; set; }
    }
    public class ModelSearchBookmark
    {
        public string MpoCodigo { get; set; }
        public IEnumerable<Bookmark> Bookmarks { get; set; }
    }
}
