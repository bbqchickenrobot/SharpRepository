using System.Collections.Generic;
using NUnit.Framework;
using SharpRepository.Tests.Integration.Data;

namespace SharpRepository.Tests.Integration.TestAttributes
{
    public class ExecuteForAllCompoundKeyRepositoriesAttribute : TestCaseSourceAttribute
    {
        private static IEnumerable<TestCaseData> ForAllCompoundKeyRepositoriesTestCaseData
        {
            get
            {
                return CompoundKeyRepositoryTestCaseDataFactory.Build(RepositoryTypes.CompoundKey);
            }
        }

        public ExecuteForAllCompoundKeyRepositoriesAttribute()
            : base(typeof(ExecuteForAllCompoundKeyRepositoriesAttribute), "ForAllCompoundKeyRepositoriesTestCaseData")
        {
        }
    }
}