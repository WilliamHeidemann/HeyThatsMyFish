using System;
using System.Collections.Generic;
using Logic;

namespace Systems
{
    public class TurnManager
    {
        private Team _teamToMove;
        public void NextTeam()
        {
            _teamToMove = _teamToMove switch
            {
                Team.Red => Team.Blue,
                Team.Blue => Team.Red,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public Team TeamToMove() => _teamToMove;
    }
}
