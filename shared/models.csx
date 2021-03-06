public class CityState
{
    public string city { get; set; }
    public string state_abbreviation { get; set; }
}

public class ZipCode
{
    public List<CityState> city_states { get; set; }
}

public class SuggestionContainer
{
    public List<Suggestion> Suggestions { get; set; }
}

public class Suggestion
{
    public string text { get; set; }
    public string street_line { get; set; }
    public string city { get; set; }
    public string state { get; set; }
}

public class Verification
{
    public string delivery_line_1 { get; set; }
    public string last_line { get; set; }
    public VerificationComponent components { get; set; }
    public VerificationAnalysis analysis { get; set; }
}

public class VerificationComponent
{
    public string zipcode { get; set; }
    public string primary_number { get; set; }
    public string street_predirection { get; set;}
    public string street_name { get; set; }
    public string street_suffix { get; set; }
    public string state_abbreviation { get; set; }
    public string city_name { get; set; }
    public string secondary_number { get; set; }
    public string secondary_designator { get; set; }
}

public class VerificationAnalysis
{
    public string dpv_match_code { get; set; }
    public string dpv_footnotes { get; set; }
    public string footnotes { get; set; }
}