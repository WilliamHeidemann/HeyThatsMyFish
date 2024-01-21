using System;
using System.Collections.Generic;
using Logic;

namespace Systems
{
    public class TurnManager
    {
        private readonly Queue<Team> _teamsQueue;

        public TurnManager(int playerCount)
        {
            _teamsQueue = new Queue<Team>();
            _teamsQueue.Enqueue(Team.Red);
            _teamsQueue.Enqueue(Team.Blue);
            if (playerCount > 2) _teamsQueue.Enqueue(Team.Green);
            if (playerCount > 3) _teamsQueue.Enqueue(Team.Yellow);
        }

        public void NextTeam()
        {
            var previous = _teamsQueue.Dequeue();
            _teamsQueue.Enqueue(previous);
        }

        public Team TeamToMove() => _teamsQueue.Peek();
    }
}
