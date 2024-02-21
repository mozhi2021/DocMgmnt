using DocMgmnt.Models;

namespace DocMgmnt.Interface
{
    public interface IDocumentHandler
    {
        public Task<string> UploadDocumentAsync(DocItem item);

        public Task<string> MoveDocumentAsync(string file);

        public Task<string> MoveAllDocumentAsync();

        public Task<string> GeneratePreSignedUploadUrl(string objectkey);
    }
}
