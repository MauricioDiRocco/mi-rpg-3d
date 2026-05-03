// Interfaz que implementan todos los objetos interactuables (NPCs, cofres, portales).
// InteractionSystem detecta los más cercanos al player y dispara Interact() con tecla E.
public interface IInteractable
{
    string InteractPrompt { get; }
    void Interact();
    void OnEnterRange();
    void OnExitRange();
}
