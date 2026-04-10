using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContinuousSpellEffect : ISpellEffect
{
    void OnMonsterSummoned(Card monster);

    void OnDeactivate();
}
