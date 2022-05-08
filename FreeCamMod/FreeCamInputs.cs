using System.Collections.Generic;
using CAMOWA.AccessHelpers;

namespace FCM
{
    public static class FreeCamInputs
    {
        public static readonly Axis MoveFoward = new Axis(InputChannels.moveZ);
        public static readonly Axis MoveRight = new Axis(InputChannels.moveX);
        public static readonly Axis MoveUp = new Axis(InputChannels.moveUp);
        public static readonly Axis MoveDown = new Axis(InputChannels.moveDown);

        public static readonly Axis RodarNaVertical = new Axis(InputChannels.pitch);
        public static readonly Axis RolarOuRodarNaHorizontal = new Axis(InputChannels.yaw);
        public static readonly Button TrocarRodarHorizontalPorRolar = new Button(InputChannels.swapRollAndYaw);

        public static readonly Button PararTempo = new Button(InputChannels.cancel);
        public static readonly Button AcelerarCamera = new Button(InputChannels.jump);
        public static readonly Button MudarAcelercaoDaCamera = new Button(InputChannels.flashlight);

        private static HashSet<InputCommand> freeCamInputs;

        public static void InnitFreeCamInputs()
        {
            freeCamInputs = new HashSet<InputCommand>
            {
                MoveFoward,
                MoveRight,
                MoveDown,
                MoveUp,

                RodarNaVertical,
                RolarOuRodarNaHorizontal,
                TrocarRodarHorizontalPorRolar,

                PararTempo,
                AcelerarCamera,
                MudarAcelercaoDaCamera,

                ReferenceFrameInput.targetReferenceFrame
            };

            GlobalMessenger.AddListener("EnterFreeCamMode", OnEnterFreeCamMode);
            GlobalMessenger.AddListener("ExitFreeCamMode", OnExitFreeCamMode);
        }

        private static HashSet<InputCommand>  lastLastInput;
        private static void OnEnterFreeCamMode()
        {
            if (OWInputHelper.UsingTelescope())
                lastLastInput = OWInputHelper.LastInputs();

            OWInputHelper.LastInputs() = new HashSet<InputCommand>(OWInputHelper.ActiveInputs());
            OWInputHelper.ActiveInputs() = new HashSet<InputCommand>(freeCamInputs);
        }
        private static void OnExitFreeCamMode()
        {
            OWInputHelper.ActiveInputs() = OWInputHelper.LastInputs();
            if (OWInputHelper.UsingTelescope())
                OWInputHelper.LastInputs() = lastLastInput;
        }
    }
}
