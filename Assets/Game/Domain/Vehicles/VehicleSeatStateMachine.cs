namespace MafiaTopDown.Gameplay.Domain.Vehicles
{
    public sealed class VehicleSeatStateMachine
    {
        public VehicleSeatState State { get; private set; } = VehicleSeatState.OnFoot;

        public void BeginEnter()
        {
            if (State != VehicleSeatState.OnFoot)
            {
                return;
            }

            State = VehicleSeatState.Entering;
        }

        public void ConfirmEntered()
        {
            if (State != VehicleSeatState.Entering)
            {
                return;
            }

            State = VehicleSeatState.Driving;
        }

        public void BeginExit()
        {
            if (State != VehicleSeatState.Driving)
            {
                return;
            }

            State = VehicleSeatState.Exiting;
        }

        public void ConfirmExited()
        {
            if (State != VehicleSeatState.Exiting)
            {
                return;
            }

            State = VehicleSeatState.OnFoot;
        }

        public void Block()
        {
            State = VehicleSeatState.Blocked;
        }

        public void ResetToOnFoot()
        {
            State = VehicleSeatState.OnFoot;
        }
    }
}
