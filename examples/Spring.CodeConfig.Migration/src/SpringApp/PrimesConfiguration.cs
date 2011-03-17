using System;
using System.Configuration;
using Primes;
using Spring.Context.Attributes;

namespace SpringApp
{
    [Configuration]
    public class PrimesConfiguration
    {
        [Definition]
        public virtual ConsoleReport ConsoleReport()
        {
            ConsoleReport report = new ConsoleReport(OutputFormatter(), PrimeGenerator());

            report.MaxNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaximumNumber"));
            return report;
        }

        [Definition]
        public virtual IOutputFormatter OutputFormatter()
        {
            return new OutputFormatter();
        }

        [Definition]
        public virtual IPrimeGenerator PrimeGenerator()
        {
            return new PrimeGenerator(new PrimeEvaluationEngine());
        }
    }
}