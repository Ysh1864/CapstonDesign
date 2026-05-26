
public interface IInteractable
{
    void Interact(PlayerMovement player);
}

public interface IPickupable
{
    void OnSwitch(PlayerMovement player);
}
