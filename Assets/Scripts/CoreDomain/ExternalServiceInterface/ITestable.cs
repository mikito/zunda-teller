using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ZundaTeller
{
    public struct TestResult
    {
        public bool isSuccess;
        public Exception exception;

        public static TestResult Success => new TestResult() { isSuccess = true };

        public static TestResult CreateAsFail(Exception e)
        {
            return new TestResult() { isSuccess = false, exception = e };
        }
    }

    public interface ITestable
    {
        UniTask<TestResult> TestAsync(CancellationToken cancellationToken);
    }
}