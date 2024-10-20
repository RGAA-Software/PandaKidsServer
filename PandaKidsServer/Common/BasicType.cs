namespace PandaKidsServer.Common;

public class BasicType
{
    public class Pair<TK, TV>
    {
        public Pair(TK k, TV v) {
            Key = k;
            Val = v;
        }

        public TK Key { set; get; }
        public TV Val { set; get; }
    }

    public class Paths
    {
        public string AbsPath;
        public string RefPath;
        public string Name;

        public Paths(string absPath, string refPath, string name) {
            AbsPath = absPath;
            RefPath = refPath;
            Name = name;
        }

        public override string ToString() {
            return "Abs path: " + AbsPath + ", Ref path: " + RefPath;
        }
    }
}