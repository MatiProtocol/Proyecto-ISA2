namespace PharmaGo.WebApi.Models.Out;

public class ChallengeModelResponse
{
    public string Challenge { get; set; }

    public ChallengeModelResponse(string challenge)
    {
        Challenge = challenge;
    }
}