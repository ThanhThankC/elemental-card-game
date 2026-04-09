public interface ITargetableController : ICardController
{
    void OnTargetSelected(Card targetCard);
    void CancelTargeting();
}
