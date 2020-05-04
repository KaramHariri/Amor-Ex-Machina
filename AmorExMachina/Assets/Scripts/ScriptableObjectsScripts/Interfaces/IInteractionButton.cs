using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionButton
{
    void NotifyToShowInteractionButton(InteractionButtons buttonToShow);
    void NotifyToHideInteractionButton(InteractionButtons buttonToHide);
}
