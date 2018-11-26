using RQ.Model.Serialization.Input;

namespace RQ2.Controller.UI.Grid
{
    public class InputCommandSlotData
    {
        public InputCommand KeyboardInputCommand { get; set; }
        public InputCommand ControllerInputCommand { get; set; }

        public string InputAction;
    }
}
