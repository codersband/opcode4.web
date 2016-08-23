using System.Web;
using System.Web.Optimization;

namespace opcode4.web
{
    public class CssRewriteUrlVirtual : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }
}
