using System;
using System.Threading.Tasks;
using DollaryDooMiner.Utils;

namespace DollaryDooMiner
{
    internal class ConsoleApplication
    {
        private readonly IMiner _miner;
        public ConsoleApplication(IMiner miner)
        {
            _miner = miner;
        }

        public async Task Run()
        {
            Console.WriteLine("Mining Dollarydoos!");
            try
            {
                //TODO: Make this mine dollarydoos constantly. Atm it'll stop as soon as it solves one.
                await MineADollaryDoo();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private async Task MineADollaryDoo()
        {
            var lastKnownProof = await _miner.GetLastProofInChain();

            var attemptedProof = 1;
            while (true)
            {
                var successResult = _miner.GuessDollaryDo(lastKnownProof, attemptedProof);
                if (!successResult)
                {
                    attemptedProof += 1;
                    continue;
                }
                break;
            }

            LogSuccess($"Valid proof found @ {attemptedProof}");
            await _miner.SubmitProof(attemptedProof);
        }

        private static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
