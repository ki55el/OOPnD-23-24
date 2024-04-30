namespace SpaceBattle.Service;

[DataContract(Name = "MessageContract")]
public class MessageContract
{
    [DataMember(Name = "Type", Order = 1)]
    public required string Type { get; set; }

    [DataMember(Name = "GameID", Order = 2)]
    public required int GameID { get; set; }

    [DataMember(Name = "GameItemID", Order = 3)]
    public required string GameItemID { get; set; }

    [DataMember(Name = "Properties", Order = 4)]
    public required IDictionary<string, object> Properties { get; set; }
}
