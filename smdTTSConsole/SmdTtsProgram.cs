using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;

namespace smdTTSConsole
{
    // Before you get started make sure you've added your own Azure Speech service KEY and a valid REGION to the appsettings.json file 
    class SmdTtsProgram
    {
        static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string input)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{input}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                default:
                    break;
            }
        }


        async static Task Main(string[] args)
        {
            // Build configuration using the appsettings.json file
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Store the SpeechKey and SpeechRegion from the appsettings.json file
            string speechKey = configuration["SpeechKey"];
            string speechRegion = configuration["SpeechRegion"];

            // Set up SpeechConfig using the the stored values for SpeechKey and SpeechRegion
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

            // Set your desired language and voice - see: https://aka.ms/speech/voices/neural
            speechConfig.SpeechSynthesisVoiceName = "en-CA-LiamNeural";

            // If this variable has a value of y then the app will run again
            string choice = "y";

            while (choice == "y")
            {

                // Get user input and synthesize to default speaker
                using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
                {
                    // Prompt the user input
                    Console.WriteLine("Enter the text you want to hear...");
                    Console.Write("> ");

                    // Store the user input
                    string input = Console.ReadLine() ?? "Input was not entered correctly. Try again later.";

                    // Synthesize the speech and store the result
                    var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(input);

                    // Provide an appropriate message based on the result of the synthesis
                    OutputSpeechSynthesisResult(speechSynthesisResult, input);
                }

                // FIN
                Console.WriteLine("Press Y and then enter to continue...");
                choice = Console.ReadLine();
            }
        }
    }
}
