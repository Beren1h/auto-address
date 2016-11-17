public class PredictionContainer
{
    public List<Prediction> Predictions { get; set; }
}

public class Prediction
{
    public string description { get; set; }
}

public class ResultContainer
{
    public List<Result> Results { get; set; }
}

public class Result
{
    public string formatted_address { get; set; }
    public List<AddressComponent> address_components { get; set; }
}

public class AddressComponent
{
    public string short_name { get; set; }
    public string long_name { get; set; }
    public List<string> types { get; set; }
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
    public string zipcode { get; set; }
    public string primary_number { get; set; }
    public string street_predirection { get; set;}
    public string street_name { get; set; }
    public string street_suffix { get; set; }
}

public class Verification
{
    public string delivery_line_1 { get; set; }
    public string last_line { get; set; }
    public VerificationComponent components { get; set; }
}

public class VerificationComponent
{
    public string zipcode { get; set; }
    public string primary_number { get; set; }
    public string street_predirection { get; set;}
    public string street_name { get; set; }
    public string street_suffix { get; set; }
}
