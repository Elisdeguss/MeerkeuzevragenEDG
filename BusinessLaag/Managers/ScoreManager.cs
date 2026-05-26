using BusinessLaag.Exceptions;
using BusinessLaag.Interfaces;
using BusinessLaag.Model;

namespace BusinessLaag.Managers
{
    public class ScoreManager
    {
        private readonly IScoreRepository _scoreRepo;

        public ScoreManager(IScoreRepository scoreRepo)
        {
            _scoreRepo = scoreRepo;
        }

        
        public void SlaScoreOp(string studentNaam, int testId, int score)
        {
            if (string.IsNullOrWhiteSpace(studentNaam))
                throw new VraagExceptions("Studentnaam mag niet leeg zijn.");

            if (testId <= 0)
                throw new VraagExceptions("Ongeldig test-ID.");

            if (score < 0)
                throw new VraagExceptions("Score kan niet negatief zijn.");

            _scoreRepo.SlaScoreOp(studentNaam, testId, score);
        }

        
        public List<TestResultaat> GetAlleScores()
        {
            return _scoreRepo.GetAlleScores();
        }
    }
}