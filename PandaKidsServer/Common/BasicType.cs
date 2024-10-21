namespace PandaKidsServer.Common;

public static class BasicType
{
    public class BasicPair<TK, TV>
    {
        public BasicPair(TK k, TV v) {
            Key = k;
            Val = v;
        }

        public TK Key { set; get; }
        public TV Val { set; get; }
    }

    public class BasicPath
    {
        public string AbsPath;
        public string RefPath;
        public string Name;
        public string? Extra = null;

        public BasicPath(string absPath, string refPath, string name) {
            AbsPath = absPath;
            RefPath = refPath;
            Name = name;
        }

        public override string ToString() {
            return "Abs path: " + AbsPath + ", Ref path: " + RefPath;
        }
    }
}