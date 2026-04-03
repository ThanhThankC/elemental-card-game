public interface ITargetableController : ICardController
{
    void OnFieldCardClickedAsTarget(Card targetCard);
    void CancelTargeting();
}
