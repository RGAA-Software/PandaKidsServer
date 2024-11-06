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
    // grades
    public List<string> Grades { set; get; } = [];
    
    public override string ToString() {
        var msg = $"{nameof(SuitType)}: {SuitType}," +
            $" {nameof(SuitName)}: {SuitName}, " +
            $"{nameof(SuitSummary)}: {SuitSummary}, " +
            $"{nameof(SuitAuthor)}: {SuitAuthor},"
            ;
        
        msg += "\nCategories: ";
        foreach (var suitCategory in SuitCategories) {
            msg += suitCategory + ",";
        }

        msg += "\nGrade: ";
        foreach (var grade in Grades) {
            msg += grade + ",";
        }
        
        return msg;
    }
}