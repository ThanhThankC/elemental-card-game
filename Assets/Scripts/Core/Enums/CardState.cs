/// <summary>
/// Card's current state in game.
/// Determines if card can be interacted with.
/// </summary>
public enum CardState
{
    InDeck,
    Drawing,
    InHand,
    //Selected,
    Playing,
    OnField,
    Discarding,
    InGraveyard,
    Destroyed,
}
