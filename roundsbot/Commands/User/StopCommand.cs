namespace roundsbot.Commands.User
{
    class StopCommand : UserCommand
    {
        public override string Name => "stop";
        public override string Usage => "stop";
        public override string Description => "Stops any running rounds.";

        public override void Execute(Discord discord, params string[] args)
        {
            RoundService.Instance.StopRounds();
        }
    }
}
