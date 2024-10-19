namespace PandaKidsServer.Common;

public class BasicType
{

    public class Pair<TK, TV>
    {
        public TK Key { set; get; }
        public TV Val { set; get; }

        public Pair(TK k, TV v)
        {
            Key = k;
            Val = v;
        }
    }

    public class Paths
    {
        public string AbsPath;
        public string RefPath;

        public Paths(string absPath, string refPath)
        {
            AbsPath = absPath;
            RefPath = refPath;
        }

        public override string ToString()
        {
            return "Abs path: " + AbsPath + ", Ref path: " + RefPath;
        }
    }

}