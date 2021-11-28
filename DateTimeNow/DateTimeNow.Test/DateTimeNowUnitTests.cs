using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = DateTimeNow.Test.CSharpCodeFixVerifier<
    DateTimeNow.DateTimeNowAnalyzer,
    DateTimeNow.DateTimeNowCodeFixProvider>;

namespace DateTimeNow.Test
{
    [TestClass]
    public class DateTimeNowUnitTest
    {
    
        //Two diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            
            const string test = @"
using System;

class Program
{
    static void Main()
    {
        var a = DateTime.Now;
        var b = DateTime.UtcNow;
        Console.WriteLine();
    }
}
";

            await VerifyCS.VerifyAnalyzerAsync(test,
                VerifyCS.Diagnostic("DateTimeNow").WithSpan(8, 17, 8, 29).WithArguments("Now or UtcNow"),
                VerifyCS.Diagnostic("DateTimeNow").WithSpan(9, 17, 9, 32).WithArguments("Now or UtcNow")
                );
        }

      
    }
}
