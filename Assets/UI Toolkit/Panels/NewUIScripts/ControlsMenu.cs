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
        gLabel = root.Q("Column2").Q<Label>("GrappleMLabel");
        mLabel = root.Q("Column2").Q<Label>("MovementLabel");
        jLabel = root.Q("Column2").Q<Label>("JumpSpaceLabel");
        sLabel = root.Q("Column2").Q<Label>("ResetRLabel");

        gLabel.RegisterCallback<ClickEvent>((evt) => {
            NewOptions.instance.BindKey(NewOptions.instance.grappleRebind);
            Debug.Log("Triggered grapple rebinding"); });
        //mLabel.RegisterCallback<ClickEvent>((evt) => {
        //    TutorialStartViewPresenter.instance.BindKey(TutorialStartViewPresenter.instance.movementRebind);
        //    Debug.Log("Triggered movement rebinding");
        //});
        jLabel.RegisterCallback<ClickEvent>((evt) => {
            NewOptions.instance.BindKey(NewOptions.instance.jumpRebind);
            Debug.Log("Triggered jump rebinding");
        });
        sLabel.RegisterCallback<ClickEvent>((evt) => {
            NewOptions.instance.BindKey(NewOptions.instance.suicideRebind);
            Debug.Log("Triggered suicide rebinding");
        });
    }
}
