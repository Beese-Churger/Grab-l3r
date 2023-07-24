using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ControlsMenu
{
    private Button _backButton;

    public Label gLabel;
    public Label mLabel;
    public Label jLabel;
    public Label sLabel;
    public Action BackAction { set => _backButton.clicked += value; }

    public ControlsMenu(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        gLabel = root.Q<Label>("GrappleLabel");
        mLabel = root.Q<Label>("MovementLabel");
        jLabel = root.Q<Label>("Jump");

        gLabel.RegisterCallback<ClickEvent>((evt) => {
            TutorialStartViewPresenter.instance.BindKey(TutorialStartViewPresenter.instance.grappleRebind);
            Debug.Log("Triggered grapple rebinding"); });
        mLabel.RegisterCallback<ClickEvent>((evt) => {
            TutorialStartViewPresenter.instance.BindKey(TutorialStartViewPresenter.instance.movementRebind);
            Debug.Log("Triggered movement rebinding");
        });
        jLabel.RegisterCallback<ClickEvent>((evt) => {
            TutorialStartViewPresenter.instance.BindKey(TutorialStartViewPresenter.instance.jumpRebind);
            Debug.Log("Triggered jump rebinding");
        });
    }
}
