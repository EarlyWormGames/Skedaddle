using UnityEngine;
using UnityEngine.InputNew;

// GENERATED FILE - DO NOT EDIT MANUALLY
public class MainMapping : ActionMapInput {
	public MainMapping (ActionMap actionMap) : base (actionMap) { }
	
	public AxisInputControl @moveX { get { return (AxisInputControl)this[0]; } }
	public AxisInputControl @moveY { get { return (AxisInputControl)this[1]; } }
	public ButtonInputControl @interact { get { return (ButtonInputControl)this[2]; } }
	public ButtonInputControl @upDownButton { get { return (ButtonInputControl)this[3]; } }
	public ButtonInputControl @leftRightButton { get { return (ButtonInputControl)this[4]; } }
	public ButtonInputControl @restart { get { return (ButtonInputControl)this[5]; } }
	public ButtonInputControl @nextAnimal { get { return (ButtonInputControl)this[6]; } }
}
