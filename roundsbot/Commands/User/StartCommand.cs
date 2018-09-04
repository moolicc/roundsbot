namespace roundsbot.Commands.User
{
    class StartCommand : UserCommand
    {
        public override string Name => "start";
        public override string Usage => "start";
        public override string Description => "Starts running rounds.";

        public override void Execute(Discord discord, params string[] args)
        {
            RoundService.Instance.StartRounds();
        }
    }
}
