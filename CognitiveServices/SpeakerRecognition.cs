using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;


public class Program 
{
    static async Task Main(string[] args)
    {
        // replace with your own subscription key 
        string subscriptionKey = "44386b4c0af2432980426cbb59c20de2";
        string region = "westus";
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
    
    }
}


public static async Task VerificationEnroll(SpeechConfig config, Dictionary<string, string> profileMapping)
{
    using (var client = new VoiceProfileClient(config))
    using (var profile = await client.CreateProfileAsync(VoiceProfileType.TextDependentVerification, "en-us"))
    {
        Console.WriteLine($"Created a profile {profile.Id} for text dependent verification.");
        string[] trainingFiles = new string[]
        {
            @"MyVoiceIsMyPassportVerifyMe01.wav",
            @"MyVoiceIsMyPassportVerifyMe02.wav",
            @"MyVoiceIsMyPassportVerifyMe03.wav"
        };

        foreach (var trainingFile in trainingFiles)
                    {
                        // Create audio input for enrollment from audio file. Replace with your own audio files.
                        using (var audioInput = AudioConfig.FromWavFileInput(trainingFile))
                        {
                            var result = await client.EnrollProfileAsync(profile, audioInput);
                            if (result.Reason == ResultReason.EnrollingVoiceProfile)
                            {
                                Console.WriteLine($"Enrolling profile id {profile.Id}.");
                            }
                            else if (result.Reason == ResultReason.EnrolledVoiceProfile)
                            {
                                Console.WriteLine($"Enrolled profile id {profile.Id}.");
                                await VerifySpeakerAsync(config, profile);
                            }
                            else if (result.Reason == ResultReason.Canceled)
                            {
                                var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
                                Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode}");
                                Console.WriteLine($"CANCELED {profile.Id}: ErrorDetails={cancellation.ErrorDetails}");
                            }

                            Console.WriteLine($"Number of enrollment audios accepted for {profile.Id} is {result.EnrollmentsCount}.");
                            Console.WriteLine($"Number of enrollment audios needed to complete { profile.Id} is {result.RemainingEnrollmentsCount}.");


                            client.DeleteProfileAsync(profile);
                        }
                    }

            
    }


}

public static async Task SpeakerVerify(SpeechConfig config, VoiceProfile profile, Dictionary<string, string> profileMapping)
{
    var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromDefaultMicrophoneInput());
    var model = SpeakerVerificationModel.FromProfile(profile);

    Console.WriteLine("Speak the passphrase to verify: \"My voice is my passport, please verify me.\"");
    var result = await speakerRecognizer.RecognizeOnceAsync(model);
    Console.WriteLine($"Verified voice profile for speaker {profileMapping[result.ProfileId]}, score is {result.Score}");
    if (result.Reason == ResultReason.RecognizedSpeaker)
    {
        Console.WriteLine($"Verified voice profile {result.ProfileId}, score is {result.Score}");
    }
    else if (result.Reason == ResultReason.Canceled)
    {
        var cancellation = SpeakerRecognitionCancellationDetails.FromResult(result);
        Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode}");
        Console.WriteLine($"CANCELED {profile.Id}: ErrorDetails={cancellation.ErrorDetails}");
    }
}

