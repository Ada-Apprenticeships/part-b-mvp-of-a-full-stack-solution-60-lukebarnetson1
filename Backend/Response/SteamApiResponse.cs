using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class GameResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public bool Success { get; set; }
    public GameData Data { get; set; }
}

public class GameData
{
    public string Type { get; set; }
    public string Name { get; set; }
    [JsonProperty("steam_appid")]
    public int SteamAppId { get; set; }
    [JsonProperty("is_free")]
    public bool IsFree { get; set; }
    private string _detailedDescription;
    [JsonProperty("detailed_description")]
    public string DetailedDescription
    {
        get { return _detailedDescription; }
        set { _detailedDescription = RemoveHtmlTags(value); }
    }
    private string _aboutTheGame;
    [JsonProperty("about_the_game")]
    public string AboutTheGame
    {
        get { return _aboutTheGame; }
        set { _aboutTheGame = RemoveHtmlTags(value); }
    }
    private string _shortDescription;
    [JsonProperty("short_description")]
    public string ShortDescription
    {
        get { return _shortDescription; }
        set { _shortDescription = RemoveHtmlTags(value); }
    }
    private string _supportedLanguages;
    [JsonProperty("supported_languages")]
    public string SupportedLanguages
    {
        get { return _supportedLanguages; }
        set { _supportedLanguages = RemoveHtmlTags(value); }
    }

    private Requirements _pcRequirements;
    [JsonProperty("pc_requirements")]
    public Requirements PcRequirements
    {
        get { return _pcRequirements; }
        set
        {
            _pcRequirements = value;
            if (_pcRequirements != null)
            {
                _pcRequirements.Minimum = RemoveHtmlTags(_pcRequirements.Minimum);
                _pcRequirements.Recommended = RemoveHtmlTags(_pcRequirements.Recommended);
            }
        }
    }
    public List<string> Developers { get; set; }
    public List<string> Publishers { get; set; }
    [JsonProperty("price_overview")]
    public PriceOverview PriceOverview { get; set; }
    public Platforms Platforms { get; set; }
    public List<Category> Categories { get; set; }
    public List<Genre> Genres { get; set; }
    [JsonProperty("release_date")]
    public ReleaseDate ReleaseDate { get; set; }
    [JsonProperty("ratings")]
    public Dictionary<string, RatingBoard> Ratings { get; set; }
    public int MostCommonAgeRating { get; set; }

    // for removing html tags
    private string RemoveHtmlTags(string htmlString)
    {
        if (string.IsNullOrEmpty(htmlString))
        {
            return htmlString;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(htmlString);

        var textWithSpaces = new StringBuilder();
        foreach (var textNode in doc.DocumentNode.DescendantsAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
        {
            textWithSpaces.Append(textNode.InnerText);
            textWithSpaces.Append(" ");
        }

        var cleanedText = textWithSpaces.ToString().Replace("\n", " ").Replace("\r", " ");
        return Regex.Replace(cleanedText, @"\s+", " ");
    }
}

public class Requirements
{
    public string Minimum { get; set; }
    public string Recommended { get; set; }
}


public class PriceOverview
{
    public string Currency { get; set; }
    public int Initial { get; set; }
    public int Final { get; set; }
    [JsonProperty("discount_percent")]
    public int DiscountPercent { get; set; }
    [JsonProperty("initial_formatted")]
    public string InitialFormatted { get; set; }
    [JsonProperty("final_formatted")]
    public string FinalFormatted { get; set; }
}

public class PackageGroup
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    [JsonProperty("selection_text")]
    public string SelectionText { get; set; }
    [JsonProperty("save_text")]
    public string SaveText { get; set; }
    [JsonProperty("display_type")]
    public int DisplayType { get; set; }
    [JsonProperty("is_recurring_subscription")]
    public string IsRecurringSubscription { get; set; }
    public List<Sub> Subs { get; set; }
}

public class Sub
{
    [JsonProperty("packageid")]
    public int PackageId { get; set; }
    [JsonProperty("percent_savings_text")]
    public string PercentSavingsText { get; set; }
    [JsonProperty("percent_savings")]
    public int PercentSavings { get; set; }
    [JsonProperty("option_text")]
    public string OptionText { get; set; }
    [JsonProperty("option_description")]
    public string OptionDescription { get; set; }
    [JsonProperty("can_get_free_license")]
    public string CanGetFreeLicense { get; set; }
    [JsonProperty("is_free_license")]
    public bool IsFreeLicense { get; set; }
    [JsonProperty("price_in_cents_with_discount")]
    public int PriceInCentsWithDiscount { get; set; }
}

public class Platforms
{
    public bool Windows { get; set; }
    public bool Mac { get; set; }
    public bool Linux { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Description { get; set; }
}

public class Genre
{
    public string Id { get; set; }
    public string Description { get; set; }
}

public class ReleaseDate
{
    [JsonProperty("coming_soon")]
    public bool ComingSoon { get; set; }
    public string Date { get; set; }
}

public class SupportInfo
{
    public string Url { get; set; }
    public string Email { get; set; }
}

public class ContentDescriptors
{
    public string Notes { get; set; }
}

public class RatingBoard
{
    public string Rating { get; set; }
    public string Descriptors { get; set; }
}