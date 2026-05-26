using BusinessLaag.Model;

namespace BusinessLaag.Interfaces
{
    public interface IScoreRepository
    {

        void SlaScoreOp(string studentNaam, int testId, int score);
        List<TestResultaat> GetAlleScores();
    }
}
