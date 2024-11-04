namespace PandaKidsServer.ResManager;

public class PresetBaseInfo
{
    // name
    public string SuitName { set; get; } = "";
    // summary
    public string SuitSummary { set; get; } = "";
    // author
    public string SuitAuthor { set; get; } = "";
    // categories
    public List<string> SuitCategories { set; get; } = [];
    // type
    public string SuitType { set; get; } = "";
    // grade
    public string Grade { set; get; } = "";
    
    public override string ToString() {
        var msg = $"{nameof(SuitType)}: {SuitType}," +
            $" {nameof(SuitName)}: {SuitName}, " +
            $"{nameof(SuitSummary)}: {SuitSummary}, " +
            $"{nameof(SuitAuthor)}: {SuitAuthor}," +
            $"{nameof(Grade)}: {Grade},"
            ;
        msg += "Categories: ";
        foreach (var suitCategory in SuitCategories) {
            msg += suitCategory + ",";
        }
        return msg;
    }
}