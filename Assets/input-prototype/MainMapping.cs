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
	public ButtonInputControl @sprint { get { return (ButtonInputControl)this[7]; } }
	public Vector3InputControl @moveCamera { get { return (Vector3InputControl)this[8]; } }
	public AxisInputControl @moveCameraX { get { return (AxisInputControl)this[9]; } }
	public AxisInputControl @moveCameraY { get { return (AxisInputControl)this[10]; } }
	public AxisInputControl @moveCameraZ { get { return (AxisInputControl)this[11]; } }
}
