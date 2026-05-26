using BusinessLaag.Model;

namespace BusinessLaag.Interfaces
{

    public interface ITestRepository
    {
        void OpslaanTest(string testCode, List<Vraag> vragen);
        Test GetTestByCode(string testCode);
        List<Test> GetAlleTests();
    }
}