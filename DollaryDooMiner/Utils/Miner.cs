using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DollaryDooMiner.Requests;
using Newtonsoft.Json;

namespace DollaryDooMiner.Utils
{
    public interface IMiner
    {
        Task<int> GetLastProofInChain();
        bool GuessDollaryDo(int lastKnownProof, int attemptedProof);
        Task SubmitProof(int proof);
    }

    public class Miner : IMiner
    {
        public async Task<int> GetLastProofInChain()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://dollarydoo.potatodeveloper.com/chain.json");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<List<DollaryDooChainResponse>>(jsonString);
            if (results.Count <= 0) throw new Exception("Failed to find anything in the chain.");

            var orderedResults = results.OrderBy(x => x.index);
            return orderedResults.Last().proof;
        }

        public bool GuessDollaryDo(int lastKnownProof, int attemptedProof)
        {
            var validProof = IsValidProof(lastKnownProof, attemptedProof, ConfigurationManager.AppSettings.Get("StringToSolve"));
            return validProof;
        }

        public async Task SubmitProof(int proof)
        {
            var request = new DollaryDooSolveRequest
            {
                from = ConfigurationManager.AppSettings.Get("DollaryDooAddress"),
                proof = proof,
            };
            using var client = new HttpClient();
            var jsonRequest = JsonConvert.SerializeObject(request);
            var response = await client.PostAsync("https://dollarydooapi.potatodeveloper.com/solve", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        private static bool IsValidProof(int lastKnownProof, int proof, string stringToFind)
        {
            var concatProofs = $"{lastKnownProof}{proof}";
            var guess = ComputeSha256Hash(concatProofs);
            var guessLength = guess.Length;
            var halfWay = int.Parse((guessLength / 2).ToString("N0"));
            var shouldBeInFirstHalf = lastKnownProof % 2 == 0;
            var firstHalf = guess.Substring(0, halfWay);
            var secondHalf = guess.Substring(halfWay);
            if (shouldBeInFirstHalf)
                return firstHalf.IndexOf(stringToFind, StringComparison.Ordinal) >= 0;
            return secondHalf.IndexOf(stringToFind, StringComparison.Ordinal) >= 0;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
