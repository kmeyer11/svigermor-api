namespace SvigermorApi.Core
{
    public interface ITranslationService
    {
        Task<TranslationResponse> Translate(TranslationRequest request);
    }
}