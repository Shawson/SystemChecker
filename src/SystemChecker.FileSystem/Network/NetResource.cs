using System.Runtime.InteropServices;
using ResourceDisplaytype = SystemChecker.FileSystem.Network.ResourceDisplaytype;
using ResourceScope = SystemChecker.FileSystem.Network.ResourceScope;
using ResourceType = SystemChecker.FileSystem.Network.ResourceType;

namespace SystemChecker.FileSystem.Network
{
    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }
}