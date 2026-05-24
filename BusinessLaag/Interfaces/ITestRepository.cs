using BusinessLaag.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Interfaces
{
    public interface ITestRepository
    {
        void OpslaanTest(string testCode, List<Vraag> vragen);

        Test GetTestByCode(string testCode);

        List<Test> GetAlleTests();
    }
}
