using System.Collections.Generic;

namespace CS5410
{
    // Structure for saved high scores
    public class HighScoreStorage
    {
        public HighScoreStorage() { }

        public HighScoreStorage(List<int> highScores)
        {
            this.highScores = highScores;
        }

        public List<int> highScores {get; set; }
    }
}
